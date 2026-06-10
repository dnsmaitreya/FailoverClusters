using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using FailoverClusters.UI.Common;
using ManagementConsole;

namespace KDDSL.ServerClusters.Management;

internal class SnapInFormView : FormView
{
	private delegate void UpdateActionsDelegate(IContext context);

	private class SelectionDataInfo
	{
		private StandardVerbs standardVerbs;

		private string displayName;

		private ActionsPaneItemCollection actions;

		private string helpTopic;

		private WritableSharedData sharedData;

		private IContext context;

		public IContext Context
		{
			get
			{
				return context;
			}
			set
			{
				context = value;
			}
		}

		public string DisplayName => displayName;

		public StandardVerbs StandardVerbs => standardVerbs;

		public ActionsPaneItemCollection ActionsPaneItems => actions;

		public string HelpTopic => helpTopic;

		public WritableSharedData SharedData => sharedData;

		public SelectionDataInfo(IContext context)
		{
			this.context = context;
			standardVerbs = context.EnabledStandardVerbs & ~StandardVerbs.Rename;
			if (!(context is IRefreshable))
			{
				standardVerbs &= ~StandardVerbs.Refresh;
			}
			ActionsPaneItemCollection actionsPaneItems = context.ActionsPaneItems;
			actions = ((actionsPaneItems != null) ? actionsPaneItems : null);
			if (actions == null)
			{
				return;
			}
			displayName = context.DisplayName;
			helpTopic = context.HelpTopic;
			sharedData = new WritableSharedData();
			foreach (WritableSharedDataItem sharedDatum in context.SharedData)
			{
				sharedData.Add(sharedDatum);
			}
		}
	}

	private static IContext processUIContext;

	private static SelectionDataInfo selectionDataInfo;

	private static SelectionDataInfo selectionDataInfoPrevious;

	private static AutoResetEvent processNextUIAction;

	private static ManualResetEvent processedNextUIAction;

	private StartPageContainerControl control;

	private bool showingControl;

	private bool isShuttingDown;

	private static int uiActionProducerCounter;

	private IContext contextPrevious;

	private object initializeLock = new object();

	private bool isInitialized;

	private static Thread uiProcessActionsThread;

	private ActionsPaneItemCollection actionPaneItemsCollection;

	private EventHandler<RefreshViewEventArgs> refreshViewEventHandler;

	public bool Shutdown => isShuttingDown;

	private CluAdminScopeNode ViewScopeNode => base.ScopeNode as CluAdminScopeNode;

	static SnapInFormView()
	{
		processUIContext = null;
		selectionDataInfo = null;
		selectionDataInfoPrevious = null;
		processNextUIAction = new AutoResetEvent(initialState: false);
		processedNextUIAction = new ManualResetEvent(initialState: false);
		uiActionProducerCounter = 0;
		InitUIProducerThread();
	}

	public static void ShutdownUIProducer()
	{
		processNextUIAction.Set();
	}

	private static void InitUIProducerThread()
	{
		if (uiProcessActionsThread != null)
		{
			try
			{
				uiProcessActionsThread.Abort();
			}
			catch (Exception caughtException)
			{
				ExceptionHelp.LogException(caughtException, "The UI Action producer failed to call Thread Abort");
			}
		}
		uiProcessActionsThread = new Thread(UIActionsProducer);
		uiProcessActionsThread.Priority = ThreadPriority.AboveNormal;
		uiProcessActionsThread.IsBackground = true;
		uiProcessActionsThread.Name = string.Format(CultureInfo.CurrentCulture, "UI Actions pane process ({0})", uiActionProducerCounter++.ToString(CultureInfo.CurrentCulture));
		uiProcessActionsThread.Start();
	}

	protected override void OnDelete(SyncStatus status)
	{
		Utilities.PerformDelete(GetSelectionData(), this, ClusterAdministrator.NotifyUser, status);
	}

	internal void RemoveResultNode(IContext context)
	{
		control.OnRemoveResultNode(context);
	}

	protected override void OnRefresh(AsyncStatus status)
	{
		try
		{
			control.RefreshSelectedResultNode(status);
		}
		catch (Exception ex)
		{
			ClusterLog.AdminEvents.WriteDataOutOfSyncEvent(status.Title, ExceptionHelp.GetExceptionMessage(ex));
			ExceptionHelp.LogException(ex, "Failed retrieving information for the current view");
		}
	}

	protected override void OnShow()
	{
		if (Global.IsProcessShuttingDown)
		{
			return;
		}
		try
		{
			ClusterAdministrator.ActiveFormView = this;
			if (control == null)
			{
				InitializeInternal();
				if (!((Control)(object)control).Created)
				{
					((Control)(object)control).CreateControl();
					control.Initialize(this);
				}
				control.OnRefreshView(UpdateReason.Refresh);
			}
			showingControl = true;
			((Control)(object)control).Refresh();
			control.OnShow();
		}
		catch (Exception e)
		{
			ViewScopeNode.ReportError(this, e, Resources.UnexpectedError_Text);
		}
	}

	protected override void OnHide()
	{
		if (Global.IsProcessShuttingDown)
		{
			return;
		}
		try
		{
			lock (base.SelectionData)
			{
				base.SelectionData.ActionsPaneItems.Clear();
				base.SelectionData.Clear();
				contextPrevious = null;
				if (selectionDataInfo != null)
				{
					selectionDataInfo.Context = null;
				}
				selectionDataInfo = null;
				selectionDataInfoPrevious = null;
			}
			if (ViewScopeNode.ViewDescriptions.Count < 2 && control != null)
			{
				showingControl = false;
				control.OnHide();
			}
		}
		catch (Exception e)
		{
			ViewScopeNode.ReportError(this, e, Resources.UnexpectedError_Text);
		}
	}

	protected override void OnAddPropertyPages(PropertyPageCollection propertyPageCollection)
	{
		IContext selectionData = GetSelectionData();
		try
		{
			foreach (PropertyPage propertyPage in ((IHasPropertyPages)selectionData).PropertyPages)
			{
				propertyPageCollection.Add(propertyPage);
			}
			ResourceContext resourceContext = selectionData as ResourceContext;
			if (resourceContext == null)
			{
				return;
			}
			bool addPropertyGrid = false;
			using (CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.VerifyingPropertySheetsExtensions_Text, string.Empty))
			{
				cluadminWaitDialog.ShowDialog(ClusterAdministrator.NotifyUser, delegate
				{
					Cluster cluster = resourceContext.Resource.Cluster;
					StringCollection stringCollection = (StringCollection)cluster.GetResourceType(resourceContext.ResourceType).GetCommonProperties(PropertyCollectionSet.ReadWrite)["AdminExtensions"].Value;
					if (stringCollection.Count > 0)
					{
						if (!SetupAdminExtensions(cluster, stringCollection))
						{
							addPropertyGrid = true;
						}
					}
					else if (!WellKnownResourceType.IsWellKnownResourceType(resourceContext.ResourceType))
					{
						addPropertyGrid = true;
					}
				});
				if (cluadminWaitDialog.IsCanceled)
				{
					return;
				}
			}
			if (addPropertyGrid)
			{
				ClusterPropertyPage clusterPropertyPage = new ClusterPropertyPage();
				clusterPropertyPage.SetControl(new PropertyGridPropertiesPage(resourceContext));
				propertyPageCollection.Add(clusterPropertyPage);
			}
		}
		catch (Exception e)
		{
			ErrorPropertyPage.CreateErrorPropertySheet(propertyPageCollection, e, selectionData.DisplayName);
		}
	}

	private bool SetupAdminExtensions(Cluster cluster, StringCollection adminExtensions)
	{
		bool result = true;
		try
		{
			bool flag = true;
			StringEnumerator enumerator = adminExtensions.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					switch (GetPropertySheetExtensionStatus(enumerator.Current))
					{
					case 2147746132u:
						flag = false;
						goto end_IL_0031;
					case 2147942526u:
						result = false;
						goto end_IL_0031;
					}
					continue;
					end_IL_0031:
					break;
				}
			}
			finally
			{
				if (enumerator is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
			if (!flag)
			{
				ClusterUtilities.InstallExtensionDlls(cluster);
			}
		}
		catch (Exception caughtException)
		{
			result = false;
			ExceptionHelp.LogException(caughtException, "Error setting up admin extensions");
		}
		return result;
	}

	[DllImport("failoverclusters.snapinsupport.dll", CharSet = CharSet.Auto)]
	private static extern uint GetPropertySheetExtensionStatus([MarshalAs(UnmanagedType.LPWStr)] string extensionDllClassId);

	protected override void OnInitialize(AsyncStatus status)
	{
		base.OnInitialize(status);
		InitializeInternal();
	}

	private void InitializeInternal()
	{
		lock (initializeLock)
		{
			if (!isInitialized)
			{
				refreshViewEventHandler = OnRefreshView;
				ViewScopeNode.RefreshView += refreshViewEventHandler;
				control = (StartPageContainerControl)(object)base.Control;
				ClusterAdministrator.SelectScopeNode += OnSelectScopeNode;
				isInitialized = true;
			}
		}
	}

	private void OnSelectScopeNode(object sender, SelectScopeNodeEventArgs e)
	{
		try
		{
			if (base.Control != null && !base.Control.IsDisposed && e.ScopeNode.Parent != null)
			{
				SelectScopeNode(e.ScopeNode);
			}
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Error selecting scope node");
		}
	}

	internal void RefreshView()
	{
		if (showingControl)
		{
			ClusterAdministrator.SetStatusBarProgressMessage(CommandResources.RefreshAction_Text.Replace("&", ""), success: true);
			control.OnRefreshView(UpdateReason.Refresh);
		}
	}

	private void OnRefreshView(object sender, RefreshViewEventArgs e)
	{
		switch (e.RefreshViewAction)
		{
		case RefreshViewAction.RefreshView:
			RefreshView();
			break;
		case RefreshViewAction.ShowWaitCursor:
			control.ShowWaitCursor();
			break;
		case RefreshViewAction.RestoreCursor:
			control.RestoreCursor();
			break;
		}
	}

	protected override void OnShutdown(SyncStatus status)
	{
		try
		{
			base.OnShutdown(status);
			lock (base.SelectionData)
			{
				base.SelectionData.ActionsPaneItems.Clear();
				base.SelectionData.Clear();
			}
			isShuttingDown = true;
			ViewScopeNode.RefreshView -= refreshViewEventHandler;
			ClusterAdministrator.SelectScopeNode -= OnSelectScopeNode;
		}
		catch (Exception e)
		{
			ViewScopeNode.ReportError(this, e, Resources.UnexpectedError_Text);
		}
	}

	public void SetSelectionData(IContext context)
	{
		try
		{
			if (context == null)
			{
				if (actionPaneItemsCollection == null)
				{
					return;
				}
				lock (base.SelectionData)
				{
					base.SelectionData.BeginUpdates();
					try
					{
						base.SelectionData.Clear();
						actionPaneItemsCollection = null;
						contextPrevious = null;
						processUIContext = null;
						return;
					}
					finally
					{
						base.SelectionData.EndUpdates();
					}
				}
			}
			if (SynchronizeInvoke.InvokeRequired)
			{
				SynchronizeInvoke.Invoke((Delegate)new UpdateActionsDelegate(UpdateActions), new object[1] { context });
			}
			else
			{
				UpdateActions(context);
			}
		}
		catch (Exception ex)
		{
			ClusterLog.AdminEvents.WriteFailedUpdatePaneActionsEvent(context.DisplayName, ExceptionHelp.GetExceptionMessage(ex));
			ExceptionHelp.LogException(ex, "Failed to update pane actions for {0}", context.DisplayName);
		}
	}

	private static void UIActionsProducer(object data)
	{
		try
		{
			while (Thread.CurrentThread.ThreadState == ThreadState.Background)
			{
				try
				{
					if (Global.IsProcessShuttingDown)
					{
						break;
					}
					processNextUIAction.WaitOne(-1);
					if (Global.IsProcessShuttingDown || Thread.CurrentThread.ThreadState != ThreadState.Background)
					{
						break;
					}
					processNextUIAction.Reset();
					IContext context = processUIContext;
					if (context != null)
					{
						SelectionDataInfo selectionDataInfo = new SelectionDataInfo(context);
						if (Thread.CurrentThread.ThreadState == ThreadState.Background)
						{
							SnapInFormView.selectionDataInfo = selectionDataInfo;
						}
					}
				}
				catch (ThreadAbortException)
				{
				}
				catch (ClusterObjectDeletedException)
				{
				}
				catch (Exception caughtException)
				{
					IContext context2 = processUIContext;
					if (context2 != null)
					{
						ExceptionHelp.LogException(caughtException, "Failed to update pane actions for {0}", context2.DisplayName);
					}
					else
					{
						ExceptionHelp.LogException(caughtException, "Failed to update pane actions");
					}
				}
				finally
				{
					if (Thread.CurrentThread.ThreadState == ThreadState.Background)
					{
						processedNextUIAction.Set();
					}
				}
			}
		}
		finally
		{
			Global.WriteLineThreadTerminated();
		}
	}

	private void UpdateActions(IContext context)
	{
		bool flag = true;
		try
		{
			processedNextUIAction.Reset();
			processUIContext = context;
			processNextUIAction.Set();
			if (!processedNextUIAction.WaitOne(3000))
			{
				InitUIProducerThread();
				if (contextPrevious != context || selectionDataInfoPrevious == null)
				{
					throw new TimeoutException();
				}
				selectionDataInfo = selectionDataInfoPrevious;
			}
			if (selectionDataInfo != null && selectionDataInfo.ActionsPaneItems != null && actionPaneItemsCollection == selectionDataInfo.ActionsPaneItems)
			{
				return;
			}
			contextPrevious = context;
			selectionDataInfoPrevious = selectionDataInfo;
			lock (base.SelectionData)
			{
				base.SelectionData.BeginUpdates();
				try
				{
					if (selectionDataInfo != null && selectionDataInfo.ActionsPaneItems != null)
					{
						base.SelectionData.ActionsPaneItems.Clear();
						base.SelectionData.EnabledStandardVerbs = selectionDataInfo.StandardVerbs;
						base.SelectionData.DisplayName = selectionDataInfo.DisplayName;
						base.SelectionData.HelpTopic = selectionDataInfo.HelpTopic;
						ActionsPaneItem[] array = null;
						while (array == null)
						{
							try
							{
								array = selectionDataInfo.ActionsPaneItems.ToArray();
							}
							catch (InvalidOperationException)
							{
							}
						}
						while (true)
						{
							try
							{
								base.SelectionData.ActionsPaneItems.AddRange(array);
							}
							catch (Exception caughtException)
							{
								ExceptionHelp.LogException(caughtException, "ActionsPaneItems.AddRange throw an exception.  Stack: {0}", DebugLog.GetStackTrace());
								base.SelectionData.ActionsPaneItems.Clear();
								continue;
							}
							break;
						}
						base.SelectionData.Update(selectionDataInfo.Context, multiSelection: false, new Guid[1] { selectionDataInfo.Context.NodeType }, selectionDataInfo.SharedData);
						actionPaneItemsCollection = selectionDataInfo.ActionsPaneItems;
					}
					else
					{
						base.SelectionData.ActionsPaneItems.Clear();
						base.SelectionData.Clear();
					}
				}
				finally
				{
					base.SelectionData.EndUpdates();
				}
			}
		}
		catch (Exception ex2)
		{
			ClusterLog.AdminEvents.WriteFailedUpdatePaneActionsEvent(context.DisplayName, ExceptionHelp.GetExceptionMessage(ex2));
			ExceptionHelp.LogException(ex2, "Failed to update pane actions for {0}", context.DisplayName);
			flag = false;
		}
		finally
		{
			if (!flag && selectionDataInfo != null)
			{
				base.SelectionData.BeginUpdates();
				try
				{
					base.SelectionData.ActionsPaneItems.Clear();
					base.SelectionData.Clear();
					base.SelectionData.EnabledStandardVerbs = StandardVerbs.None;
					base.SelectionData.DisplayName = context.DisplayName;
					DebugLog.GetStackTrace();
					base.SelectionData.Update(selectionDataInfo.Context, multiSelection: false, new Guid[1] { selectionDataInfo.Context.NodeType }, selectionDataInfo.SharedData);
					actionPaneItemsCollection = null;
				}
				finally
				{
					base.SelectionData.EndUpdates();
				}
			}
		}
	}

	public IContext GetSelectionData()
	{
		try
		{
			lock (base.SelectionData)
			{
				return base.SelectionData.SelectionObject as IContext;
			}
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Exception getting the context menu information...");
			return null;
		}
	}
}

