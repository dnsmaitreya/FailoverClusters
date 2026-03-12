using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal class CluAdminScopeNode : ScopeNode, IDisposable
{
	private delegate void StringParamDelegate(string name);

	private delegate void ContextParamDelegate(IContext context);

	private struct ImageIndexInfo
	{
		public int ImageIndex;

		public int SelectedImageIndex;
	}

	private IScopeNodeContext context;

	private bool expanded;

	private object lockObject = new object();

	private bool disposed;

	private bool deleting;

	private static IScopeNodeContext selectedContext;

	private ActionsPaneItemCollection actionPaneItemsCollection;

	private AsyncBatchEnumeration<IScopeNodeContext> batchEnum;

	private AsyncStatus expandStatus;

	private BackgroundOperation<object, string> updateDisplayNameOperation;

	private BackgroundOperation<object, ImageIndexInfo> updateImageIndexItemsOperation;

	private BackgroundOperation<object, object> refreshOperation;

	public INotifyUser NotifyUser
	{
		get
		{
			INotifyUser notifyUser = ClusterAdministrator.NotifyUser;
			return Utilities.GetClusterNotifyUser(context, notifyUser);
		}
	}

	internal IScopeNodeContext Context => context;

	public event EventHandler<RefreshViewEventArgs> RefreshView;

	public event EventHandler ScopeNodeAdded;

	internal CluAdminScopeNode(IScopeNodeContext context)
		: base(context.NodeType, context.ChildrenPossible)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		expanded = false;
		base.Tag = context;
		this.context = context;
		base.DisplayName = context.DisplayName;
		base.ImageIndex = context.ImageIndex;
		base.SelectedImageIndex = context.SelectedImageIndex;
		base.HelpTopic = context.HelpTopic;
		foreach (WritableSharedDataItem sharedDatum in context.SharedData)
		{
			base.SharedData.Add(sharedDatum);
		}
		base.ViewDescriptions = context.ViewDescriptions;
		if (base.ViewDescriptions.Count > 0)
		{
			base.ViewDescriptions.DefaultIndex = 0;
		}
		updateDisplayNameOperation = new BackgroundOperation<object, string>((BackgroundOperationFunction<object, string>)GetDisplayName);
		updateDisplayNameOperation.OperationCompleted += UpdateDisplayName;
		updateDisplayNameOperation.MaximumRetriesOnError = ClusterAdministrator.MaxBackgroundRetries;
		updateImageIndexItemsOperation = new BackgroundOperation<object, ImageIndexInfo>((BackgroundOperationFunction<object, ImageIndexInfo>)GetImageIndex);
		updateImageIndexItemsOperation.OperationCompleted += UpdateImageIndex;
		updateImageIndexItemsOperation.MaximumRetriesOnError = ClusterAdministrator.MaxBackgroundRetries;
		refreshOperation = new BackgroundOperation<object, object>((BackgroundOperationFunction<object, object>)RefreshContext);
		refreshOperation.OperationCompleted += refreshOperation_OperationCompleted;
		base.ActionsActivated += CluAdminScopeNode_ActionsActivated;
		base.ActionsDeactivated += CluAdminScopeNode_ActionsDeactivated;
		context.ActionsUpdated += OnActionsUpdated;
		context.DisplayNameChanged += OnDisplayNameChanged;
		context.ImageIndexChanged += OnImageIndexChanged;
		context.Deleting += Context_Deleting;
		context.ChildAdded += OnChildAdded;
		context.ChildDeleted += OnChildDeleted;
		context.ChildInserted += OnChildInserted;
		context.ContextCleared += ContextContextCleared;
		this.context.AsyncChildEnumerationComplete += OnAsyncChildEnumerationComplete;
		base.EnabledStandardVerbs = context.EnabledStandardVerbs;
		if (!Context.ChildrenPossible)
		{
			batchEnum = new AsyncBatchEnumeration<IScopeNodeContext>(MoveChildrenToUI);
		}
	}

	protected override byte[] OnGetSharedData(WritableSharedDataItem item, SyncStatus status)
	{
		if (context is ClusterContext clusterContext && item.ClipboardFormatId == "CLUSTER_NAME")
		{
			return clusterContext.GetClusterNameBytes();
		}
		return base.OnGetSharedData(item, status);
	}

	private void CluAdminScopeNode_ActionsActivated(object sender, EventArgs e)
	{
		selectedContext = context;
		UpdateActions();
	}

	private void CluAdminScopeNode_ActionsDeactivated(object sender, EventArgs e)
	{
		if (context == selectedContext)
		{
			selectedContext = null;
		}
		if (context.ClearActionsOnDeactivateScopeNode)
		{
			base.ActionsPaneItems.Clear();
		}
	}

	private void UpdateActions()
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		ActionsPaneItem[] actionPaneItems = null;
		ActionsPaneItemCollection referenceToActionPaneItems = null;
		try
		{
			referenceToActionPaneItems = context.ActionsPaneItems;
			ActionsPaneItem[] array = null;
			while (array == null)
			{
				try
				{
					array = referenceToActionPaneItems.ToArray();
				}
				catch (InvalidOperationException)
				{
				}
			}
			actionPaneItems = array;
		}
		catch (Exception ex2)
		{
			ClusterLog.AdminEvents.WriteFailedUpdatePaneActionsEvent(base.DisplayName, ExceptionHelp.GetExceptionMessage(ex2));
			ClusterLog.LogException(ex2, "Failed to update pane actions for {0}", new object[1] { base.DisplayName });
			actionPaneItems = new ActionsPaneItem[0];
		}
		UIThreadHandlerV val = (UIThreadHandlerV)delegate
		{
			base.ActionsPaneItems.Clear();
			base.ActionsPaneItems.AddRange(actionPaneItems);
			actionPaneItemsCollection = referenceToActionPaneItems;
		};
		if (actionPaneItems != null)
		{
			if (SynchronizeInvoke.InvokeRequired)
			{
				SynchronizeInvoke.Invoke((Delegate)(object)val);
			}
			else
			{
				val.Invoke();
			}
		}
	}

	private void Context_Deleting(object sender, DeletingEventArgs e)
	{
		if (e.Stage == DeletingStage.Start)
		{
			deleting = true;
		}
		else if (e.Stage == DeletingStage.Error || e.Stage == DeletingStage.Canceled)
		{
			deleting = false;
			Refresh();
		}
	}

	private void UpdateDisplayName()
	{
		if (!deleting)
		{
			updateDisplayNameOperation.QueueOperation((object)null);
		}
	}

	private string GetDisplayName(BackgroundOperationStatus backgroundStatus, object parameter)
	{
		return context.DisplayName;
	}

	private void UpdateDisplayName(object sender, BackgroundOperationCompletedEventArgs<object, string> e)
	{
		if (!disposed)
		{
			if (e.Success)
			{
				base.DisplayName = e.OperationResult;
			}
			else if (e.Error != null && !ExceptionHelp.IsFirstExceptionFound<ClusterObjectDeletedException>(e.Error))
			{
				ClusterLog.AdminEvents.WriteFailedUpdateImageIndexEvent(base.DisplayName, ExceptionHelp.GetExceptionMessage(e.Error));
				ExceptionHelp.LogException(e.Error, "Failed to update display name {0}", base.DisplayName);
			}
		}
	}

	private void UpdateImageIndex()
	{
		if (!deleting)
		{
			updateImageIndexItemsOperation.QueueOperation((object)null);
		}
	}

	private ImageIndexInfo GetImageIndex(BackgroundOperationStatus backgroundStatus, object parameter)
	{
		ImageIndexInfo result = default(ImageIndexInfo);
		result.ImageIndex = context.ImageIndex;
		result.SelectedImageIndex = context.SelectedImageIndex;
		return result;
	}

	private void UpdateImageIndex(object sender, BackgroundOperationCompletedEventArgs<object, ImageIndexInfo> e)
	{
		if (!disposed)
		{
			if (e.Success)
			{
				ImageIndexInfo operationResult = e.OperationResult;
				base.ImageIndex = operationResult.ImageIndex;
				base.SelectedImageIndex = operationResult.SelectedImageIndex;
			}
			else if (e.Error != null && !ExceptionHelp.IsFirstExceptionFound<ClusterObjectDeletedException>(e.Error))
			{
				ClusterLog.AdminEvents.WriteFailedUpdateImageIndexEvent(base.DisplayName, ExceptionHelp.GetExceptionMessage(e.Error));
				ExceptionHelp.LogException(e.Error, "Failed to update image index for {0}", base.DisplayName);
			}
		}
	}

	private void DeleteChild(string childName)
	{
		lock (lockObject)
		{
			if (batchEnum != null)
			{
				batchEnum.FlushBatchedItems();
			}
			CluAdminScopeNode cluAdminScopeNode = FindChild(childName);
			if (cluAdminScopeNode != null)
			{
				base.Children.Remove(cluAdminScopeNode);
				cluAdminScopeNode.Dispose();
			}
		}
	}

	private void DeleteChild(IContext childContext)
	{
		lock (lockObject)
		{
			if (batchEnum != null)
			{
				batchEnum.FlushBatchedItems();
			}
			CluAdminScopeNode cluAdminScopeNode = FindChild(childContext);
			if (cluAdminScopeNode != null)
			{
				base.Children.Remove(cluAdminScopeNode);
				cluAdminScopeNode.Dispose();
			}
		}
	}

	private void MoveChildrenToUI(ICollection<IScopeNodeContext> items)
	{
		IEnumerable<IScopeNodeContext> contexts = items.Where(delegate(IScopeNodeContext itemContext)
		{
			if (itemContext.Deleted)
			{
				return false;
			}
			if (!itemContext.Initialized)
			{
				itemContext.Initialize();
			}
			return true;
		});
		System.Action action = delegate
		{
			CluAdminScopeNode[] array = contexts.Select((IScopeNodeContext itemContext) => new CluAdminScopeNode(itemContext)).ToArray();
			lock (lockObject)
			{
				ScopeNodeCollection children = base.Children;
				ScopeNode[] items2 = array;
				children.AddRange(items2);
				EventHandler scopeNodeAdded = this.ScopeNodeAdded;
				if (scopeNodeAdded != null)
				{
					CluAdminScopeNode[] array2 = array;
					foreach (CluAdminScopeNode sender in array2)
					{
						scopeNodeAdded(sender, EventArgs.Empty);
					}
				}
			}
		};
		if (SynchronizeInvoke.InvokeRequired)
		{
			SynchronizeInvoke.BeginInvoke((Delegate)action, (object[])null);
		}
		else
		{
			action();
		}
	}

	protected override void OnExpand(AsyncStatus status)
	{
		if (!context.ChildrenPossible)
		{
			status.EnableManualCompletion();
			expandStatus = status;
			Expand();
		}
	}

	private void Expand()
	{
		if (!expanded)
		{
			expanded = true;
			if (!context.ChildrenPossible)
			{
				Background.QueueWorker((WaitCallback)ExpandWorkItem);
			}
		}
	}

	private void ExpandWorkItem(object data)
	{
		try
		{
			Context.EnumerateChildrenAsync();
		}
		catch (Exception ex)
		{
			ClusterLog.AdminEvents.WriteFailedExpandingNodeEvent(base.DisplayName, ExceptionHelp.GetExceptionMessage(ex));
			ExceptionHelp.LogException(ex, "Failed to expand scope node {0}", base.DisplayName);
		}
	}

	private void OnAsyncChildEnumerationComplete(object sender, EventArgs eventArgs)
	{
		if (batchEnum != null)
		{
			batchEnum.FlushBatchedItems();
		}
		if (expandStatus != null)
		{
			expandStatus.Complete(Resources.ExpansionComplete_Text, success: true);
			expandStatus = null;
		}
	}

	protected override void OnAddPropertyPages(PropertyPageCollection pages)
	{
		try
		{
			foreach (PropertyPage propertyPage in ((IHasPropertyPages)context).PropertyPages)
			{
				pages.Add(propertyPage);
			}
		}
		catch (Exception e)
		{
			ErrorPropertyPage.CreateErrorPropertySheet(pages, e, base.DisplayName);
		}
	}

	internal void Remove()
	{
		((CluAdminScopeNode)base.Parent)?.DeleteChild(base.DisplayName);
	}

	private object RefreshContext(BackgroundOperationStatus status, object data)
	{
		GetRefreshableParent(this)?.Refresh();
		return null;
	}

	private IRefreshable GetRefreshableParent(CluAdminScopeNode node)
	{
		if (node == null)
		{
			return null;
		}
		if (!(node.context is IRefreshable))
		{
			return GetRefreshableParent((CluAdminScopeNode)node.Parent);
		}
		return (IRefreshable)node.context;
	}

	private void refreshOperation_OperationCompleted(object sender, BackgroundOperationCompletedEventArgs<object, object> e)
	{
		OnRefreshView(RefreshViewAction.RestoreCursor);
		if (e.Error != null)
		{
			ClusterLog.AdminEvents.WriteFailedRefreshingScopeNodeEvent(base.DisplayName);
			ClusterAdministrator.SetStatusBarProgressMessage(string.Format(CultureInfo.CurrentCulture, Resources.CannotRefreshScopeNode_Text, base.DisplayName));
		}
		else if (e.Success)
		{
			OnRefreshView(RefreshViewAction.RefreshView);
		}
	}

	private bool Contains(IContext childContext, ScopeNodeCollection scopeNodes)
	{
		if (FindChild(childContext) != null)
		{
			return true;
		}
		return false;
	}

	protected override void OnRefresh(AsyncStatus status)
	{
		OnRefreshView(RefreshViewAction.ShowWaitCursor);
		if (!refreshOperation.QueueOperation((object)null))
		{
			OnRefreshView(RefreshViewAction.RestoreCursor);
		}
	}

	protected override void OnRename(string newName, SyncStatus status)
	{
		try
		{
			((IRenameable)context).Rename(NotifyUser, newName, status);
		}
		catch (Exception ex)
		{
			NotifyUser.ShowError(ex, Resources.CannotRename_Text, new object[1] { base.DisplayName });
		}
	}

	protected override void OnDelete(SyncStatus status)
	{
		Utilities.PerformDelete(context, this, NotifyUser, status);
	}

	private void OnRefreshView(RefreshViewAction refreshViewAction)
	{
		this.RefreshView?.Invoke(this, new RefreshViewEventArgs(refreshViewAction));
	}

	private void OnActionsUpdated(object sender, EventArgs e)
	{
		if (ClusterAdministrator.ActiveFormView != null && ClusterAdministrator.ActiveFormView.ScopeNode == this)
		{
			UpdateActions();
		}
	}

	private void OnImageIndexChanged(object sender, EventArgs e)
	{
		UpdateImageIndex();
	}

	private void OnDisplayNameChanged(object sender, EventArgs e)
	{
		UpdateDisplayName();
	}

	private void OnChildAdded(object sender, ChildAddedEventArgs e)
	{
		if (!e.DelayedAdd && base.SnapIn.InvokeRequired)
		{
			base.SnapIn.Invoke(new EventHandler<ChildAddedEventArgs>(OnChildAdded), new object[2] { sender, e });
			return;
		}
		if (e.ChildContext is IScopeNodeContext child)
		{
			AddChild(child, e.DelayedAdd);
			return;
		}
		List<IContext> childContexts = e.ChildContexts;
		if (childContexts != null)
		{
			List<IScopeNodeContext> children = childContexts.ConvertAll((IContext context) => (IScopeNodeContext)context);
			AddChildren(children, e.DelayedAdd);
		}
	}

	private void AddChild(IScopeNodeContext child, bool delayedAdd)
	{
		lock (lockObject)
		{
			if (!expanded)
			{
				return;
			}
			if (Contains(child, base.Children))
			{
				DebugLog.LogError("Duplicate child - {0}", child.DisplayName);
			}
			else if (batchEnum != null)
			{
				batchEnum.AddItem(child);
				if (!delayedAdd)
				{
					batchEnum.FlushBatchedItems();
				}
			}
		}
	}

	private void AddChildren(List<IScopeNodeContext> children, bool delayedAdd)
	{
		lock (lockObject)
		{
			if (!expanded || batchEnum == null)
			{
				return;
			}
			foreach (IScopeNodeContext child in children)
			{
				batchEnum.AddItem(child);
				if (!delayedAdd)
				{
					batchEnum.FlushBatchedItems();
				}
			}
		}
	}

	private void OnChildInserted(object sender, ChildInsertedEventArgs e)
	{
		if (base.SnapIn.InvokeRequired)
		{
			base.SnapIn.Invoke(new EventHandler<ChildInsertedEventArgs>(OnChildInserted), new object[2] { sender, e });
		}
		else if (e.ChildContext is IScopeNodeContext child)
		{
			InsertChild(child, e.Index);
		}
	}

	private void InsertChild(IScopeNodeContext child, int index)
	{
		lock (lockObject)
		{
			if (expanded)
			{
				if (Contains(child, base.Children))
				{
					DebugLog.LogError("Duplicate child - {0}", child.DisplayName);
				}
				else
				{
					base.Children.Insert(index, new CluAdminScopeNode(child));
				}
			}
		}
	}

	private void OnChildDeleted(object sender, ChildDeletedEventArgs e)
	{
		if (e != null)
		{
			if (base.SnapIn.InvokeRequired)
			{
				base.SnapIn.Invoke(new ContextParamDelegate(DeleteChild), new object[1] { e.ChildContext });
			}
			else
			{
				DeleteChild(e.ChildContext);
			}
		}
	}

	private void ContextContextCleared(object sender, EventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		UIThreadHandlerV val = (UIThreadHandlerV)delegate
		{
			ClearChildren();
		};
		if (SynchronizeInvoke.InvokeRequired)
		{
			SynchronizeInvoke.Invoke((Delegate)(object)val);
		}
		else
		{
			val.Invoke();
		}
	}

	public void Dispose()
	{
		disposed = true;
		base.ActionsActivated -= CluAdminScopeNode_ActionsActivated;
		base.ActionsDeactivated -= CluAdminScopeNode_ActionsDeactivated;
		context.ActionsUpdated -= OnActionsUpdated;
		context.DisplayNameChanged -= OnDisplayNameChanged;
		context.ImageIndexChanged -= OnImageIndexChanged;
		context.ChildAdded -= OnChildAdded;
		context.ChildDeleted -= OnChildDeleted;
		context.ChildInserted -= OnChildInserted;
		context.ContextCleared -= ContextContextCleared;
		context.AsyncChildEnumerationComplete -= OnAsyncChildEnumerationComplete;
		context = null;
		base.Tag = null;
		updateDisplayNameOperation.CancelOperations();
		updateImageIndexItemsOperation.CancelOperations();
		if (batchEnum != null)
		{
			batchEnum.Dispose();
		}
		ClearChildren();
		GC.SuppressFinalize(this);
	}

	private void ClearChildren()
	{
		lock (lockObject)
		{
			foreach (CluAdminScopeNode child in base.Children)
			{
				child.Dispose();
			}
			base.Children.Clear();
		}
	}

	internal CluAdminScopeNode FindChild(string childName)
	{
		CluAdminScopeNode result = null;
		lock (lockObject)
		{
			foreach (CluAdminScopeNode child in base.Children)
			{
				if (string.Compare(child.DisplayName, childName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					result = child;
					break;
				}
			}
		}
		return result;
	}

	internal CluAdminScopeNode FindChild(IContext childContext)
	{
		CluAdminScopeNode result = null;
		lock (lockObject)
		{
			foreach (CluAdminScopeNode child in base.Children)
			{
				if (child.context == childContext || child.DisplayName == childContext.DisplayName)
				{
					result = child;
					break;
				}
			}
		}
		return result;
	}

	internal virtual CluAdminScopeNode FindChildWithExpand(string childName, INotifyUser notifyUser)
	{
		if (string.IsNullOrEmpty(childName))
		{
			return null;
		}
		if (!expanded)
		{
			Expand();
		}
		else
		{
			CluAdminScopeNode cluAdminScopeNode = FindChild(childName);
			if (cluAdminScopeNode != null)
			{
				return cluAdminScopeNode;
			}
		}
		CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.Expanding_Text, Resources.ExpandingChildren_Text);
		using (cluadminWaitDialog)
		{
			return cluadminWaitDialog.ShowDialog(notifyUser, BackgroundFindChild, childName);
		}
	}

	private CluAdminScopeNode BackgroundFindChild(CluadminWaitDialog waitDialog, string childName)
	{
		CluAdminScopeNode cluAdminScopeNode = null;
		int num = 0;
		while (cluAdminScopeNode == null && num < 5)
		{
			waitDialog.ThrowIfCanceled();
			cluAdminScopeNode = FindChild(childName);
			if (cluAdminScopeNode == null)
			{
				Thread.Sleep(new TimeSpan(0, 0, 1));
			}
			num++;
		}
		return cluAdminScopeNode;
	}

	internal void Refresh()
	{
		OnRefresh(null);
	}

	internal void ReportError(View view, Exception e, string message)
	{
		ExceptionHelp.LogException(e, Resources.UnexpectedError_Text);
		ErrorMessageViewData tag = new ErrorMessageViewData(e, message);
		FormViewDescription item = Utilities.CreateFormViewDescription(Resources.Error_Text, typeof(ErrorStartPageControl), tag);
		base.ViewDescriptions.Clear();
		base.ViewDescriptions.Add(item);
		base.ViewDescriptions.DefaultIndex = 0;
		view.SelectScopeNode(this);
	}

	internal void RestoreForm(View view)
	{
		base.ViewDescriptions = context.ViewDescriptions;
		base.ViewDescriptions.DefaultIndex = 0;
		view.SelectScopeNode(this);
	}
}
