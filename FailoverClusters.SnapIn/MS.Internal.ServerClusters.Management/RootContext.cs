using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Microsoft.FailoverClusters.ClusterSnapIn;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.SnapIn;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.FailoverClusters.UI.Common.Services;
using Microsoft.FailoverClusters.UIFramework;
using Microsoft.ManagementConsole;
using MS.Internal.ServerClusters.Wizards;

namespace MS.Internal.ServerClusters.Management;

internal class RootContext : ScopeNodeContextBase, IHasPropertyPages
{
	private class BackgroundClusterConnectArgs
	{
		private ConnectedClusterData connectionData;

		private bool selectNode;

		private bool reportError;

		public ConnectedClusterData ConnectionData => connectionData;

		public bool SelectNode => selectNode;

		public bool ReportError => reportError;

		public BackgroundClusterConnectArgs(ConnectedClusterData connectionData)
			: this(connectionData, selectNode: false, reportError: false)
		{
		}

		public BackgroundClusterConnectArgs(ConnectedClusterData connectionData, bool selectNode, bool reportError)
		{
			this.connectionData = connectionData;
			this.selectNode = selectNode;
			this.reportError = reportError;
		}
	}

	private delegate void OpenComplete(IContext clusterContext, bool selectNode);

	private static RootContext singletonRootContext;

	private readonly ClusterAdministrator snapIn;

	private bool disposed;

	private volatile ActionsPaneItemCollection actions;

	private readonly object addChildLock = new object();

	private Action<ViewModelDataSignal> signalViewModel;

	internal ActionBase ValidateConfigurationAction { get; private set; }

	internal ActionBase CreateClusterAction { get; private set; }

	internal ActionBase ManageClusterAction { get; private set; }

	public override bool ClearActionsOnDeactivateScopeNode => false;

	public override ViewDescriptionCollection ViewDescriptions
	{
		get
		{
			ViewDescriptionCollection viewDescriptionCollection = new ViewDescriptionCollection();
			ViewModelData viewModelData = new ViewModelData
			{
				HelpTopic = HelpTopic,
				ViewCommandsProvider = (IViewCommandsProvider)(object)new ClusterRootViewCommandsProvider(this)
			};
			signalViewModel = viewModelData.ProcessMessage;
			viewDescriptionCollection.Add(new FormViewDescription
			{
				DisplayName = Resources.ManageServerClusters_Text,
				ViewType = typeof(ClusterRootViewAdapter),
				ControlType = typeof(WpfViewHostControl<ClusterRootView, ClusterRootViewAdapter>),
				Tag = viewModelData
			});
			return viewDescriptionCollection;
		}
	}

	public override ActionsPaneItemCollection ActionsPaneItems
	{
		get
		{
			if (isDisposed)
			{
				return new ActionsPaneItemCollection();
			}
			lock (base.ActionsLock)
			{
				if (actions == null)
				{
					actions = CreateActions();
				}
			}
			return actions;
		}
	}

	public PropertyPageCollection PropertyPages
	{
		get
		{
			PropertyPageCollection propertyPageCollection = new PropertyPageCollection();
			ClusterPropertyPage clusterPropertyPage = new ClusterPropertyPage();
			clusterPropertyPage.SetControl(new ToolsPropertiesPage(snapIn.Settings));
			propertyPageCollection.Add(clusterPropertyPage);
			return propertyPageCollection;
		}
	}

	private RootContext(ClusterAdministrator snapIn, ICollection<string> commandLineClusterNames)
		: base(new Guid("{5FE7DF44-C460-447c-8A4E-B9FFFB32DC6A}"), ExpandIconOptions.DoNotShow)
	{
		Exceptions.ThrowIfNull((object)snapIn, "snapIn");
		Exceptions.ThrowIfNull((object)commandLineClusterNames, "commandLineClusterNames");
		base.DisplayName = snapIn.RootContextDisplayName;
		base.ImageIndex = snapIn.RootContentIconIndex;
		this.snapIn = snapIn;
		this.snapIn.SettingsLoaded += OnSettingsLoaded;
		if (Utilities.IsDomainUser())
		{
			ConnectToLocalCluster();
			ConnectToListOfClusters(commandLineClusterNames, selectCluster: true);
		}
		ServiceContainer.Container.Resolve<IClusterDataService>(Array.Empty<object>()).ClustersDataChanged += delegate(object s, ClusterDataChangedEventArgs v)
		{
			if (v.ClusterDataChangedType == ClusterDataChangedType.Removal)
			{
				foreach (ClusterContext item in GetChildContexts().OfType<ClusterContext>())
				{
					Microsoft.FailoverClusters.Framework.Cluster frameworkCluster = item.FrameworkCluster;
					if (frameworkCluster != null && frameworkCluster.CacheId == v.ClusterCacheId)
					{
						item.Close();
					}
				}
			}
		};
		if (WindowsAdminCenterNotificationDialog.ShouldShow())
		{
			new WindowsAdminCenterNotificationDialog().Show();
		}
	}

	internal static RootContext CreateInstance(ClusterAdministrator snapIn, ICollection<string> commandLineClusterNames)
	{
		singletonRootContext = new RootContext(snapIn, commandLineClusterNames);
		return singletonRootContext;
	}

	private void ConnectToLocalCluster()
	{
		BackgroundClusterConnectArgs connectArgs = new BackgroundClusterConnectArgs(ConnectedClusterData.CreateNameData(Environment.MachineName));
		QueueBackgroundConnectRequest(connectArgs);
	}

	private void QueueBackgroundConnectRequest(BackgroundClusterConnectArgs connectArgs)
	{
		Background.QueueWorker((WaitCallback)BackgroundConnectToCluster, (object)connectArgs);
	}

	private void ConnectToListOfClusters(ICollection<string> commandLineClusterNames, bool selectCluster)
	{
		foreach (string commandLineClusterName in commandLineClusterNames)
		{
			ConnectedClusterData connectionData = ConnectedClusterData.CreateNameData(commandLineClusterName);
			QueueBackgroundConnectRequest(new BackgroundClusterConnectArgs(connectionData, selectCluster, reportError: true));
			selectCluster = false;
		}
	}

	protected override void UpdateStateBasedActions()
	{
	}

	public override void ClearActions()
	{
		lock (base.ActionsLock)
		{
			actions = Utilities.DisposeActions(actions);
		}
		base.ClearActions();
	}

	private ActionsPaneItemCollection CreateActions()
	{
		ActionsPaneItemCollection actionsPaneItemCollection = new ActionsPaneItemCollection();
		ValidateConfigurationAction = SharedActions.CreateValidationAction(CommandResources.ValidateConfigurationAction_Text, OnValidateConfiguration);
		CreateClusterAction = ActionFactory.CreateAction(CommandResources.CreateClusterAction_Text, Resources.CreateClusterAction_Description_Text, Icons.CreateClusterIndex, OnNewCluster);
		ManageClusterAction = ActionFactory.CreateAction(CommandResources.ManageClusterAction_Text, Resources.ManageClusterAction_Description_Text, Icons.ClusterIndex, OnConnect);
		actionsPaneItemCollection.AddRange(new ActionsPaneItem[3] { ValidateConfigurationAction, CreateClusterAction, ManageClusterAction });
		return actionsPaneItemCollection;
	}

	private void OnConnect(object sender, SnapinActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		DoConnect(notifyUserFromSender);
	}

	internal void DoConnect(INotifyUser notifyUser)
	{
		DoConnect(notifyUser, string.Empty);
	}

	internal void DoConnect(INotifyUser notifyUser, string firstChoice)
	{
		IContext context = null;
		List<string> list = snapIn.Settings.ClusterMRU.ToList();
		if (!string.IsNullOrEmpty(firstChoice))
		{
			list.RemoveAll((string s) => string.Compare(s, firstChoice, StringComparison.OrdinalIgnoreCase) == 0);
			list.Insert(0, firstChoice);
		}
		OpenClusterDlg openClusterDlg = new OpenClusterDlg(list);
		try
		{
			if (notifyUser.ShowDialog((Form)(object)openClusterDlg) == DialogResult.OK)
			{
				string clusterName = openClusterDlg.ClusterName;
				ICollection<string> nodeNames = new List<string>();
				ConnectedClusterData connectedClusterData = snapIn.Settings.ConnectedClusters.FirstOrDefault((ConnectedClusterData data) => string.Compare(clusterName, data.ClusterName, StringComparison.CurrentCultureIgnoreCase) == 0);
				if (connectedClusterData != null)
				{
					nodeNames = connectedClusterData.NodeNames;
				}
				ConnectedClusterData connectionData = ConnectedClusterData.CreateNameData(clusterName, nodeNames);
				try
				{
					context = ClusterConnectionFactory.ConnectToCluster(connectionData, notifyUser, ConnectionType.AnyConnection);
				}
				catch (Exception ex)
				{
					notifyUser.ShowError(ex, Resources.ClusterConnectFailed_Text, new object[1] { clusterName });
				}
				if (context != null)
				{
					AddChild(context, selectNode: true);
				}
				else
				{
					snapIn.Settings.DecrementConnectedCluster(clusterName);
				}
			}
		}
		finally
		{
			((IDisposable)openClusterDlg)?.Dispose();
		}
	}

	private void OnNewCluster(object sender, SnapinActionEventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		CreateClusterWizard val = new CreateClusterWizard();
		CreateClusterWizard val2 = val;
		bool wizardTaskCompleted;
		Cluster cluster;
		try
		{
			notifyUserFromSender.ShowDialog((Form)(object)val);
			wizardTaskCompleted = ((ClusterWizardForm)val).WizardTaskCompleted;
			cluster = val.Cluster;
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		if (wizardTaskCompleted)
		{
			AddNewCluster(cluster, notifyUserFromSender, e);
		}
	}

	private void AddNewCluster(Cluster cluster, INotifyUser notifyUser, SnapinActionEventArgs e)
	{
		CluadminWaitDialog cluadminWaitDialog = e.CreateWaitDialog(Resources.ConnectingToCluster_Text, cluster.Name);
		IContext context = null;
		try
		{
			using (cluadminWaitDialog)
			{
				context = cluadminWaitDialog.ShowDialog(notifyUser, CreateClusterContext, cluster);
				if (!cluadminWaitDialog.IsCanceled)
				{
					AddChild(context, selectNode: true);
				}
			}
		}
		finally
		{
			if (context == null)
			{
				MS.Internal.ServerClusters.Utilities.BackgroundDisposeObject(cluster);
			}
		}
	}

	private IContext CreateClusterContext(CluadminWaitDialog waitDialog, Cluster cluster)
	{
		return ContextFactory.CreateContext(cluster);
	}

	private void OnValidateConfiguration(object sender, SnapinActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		Cluster cluster = SharedActions.PerformValidationAction(notifyUserFromSender);
		if (cluster != null)
		{
			AddNewCluster(cluster, notifyUserFromSender, e);
		}
	}

	public override void Refresh()
	{
		signalViewModel.SafeCall(ViewModelDataSignal.Refresh);
	}

	private void RemoveChild(string name)
	{
		IContext context = FindChildContext(name);
		if (context != null)
		{
			RemoveChild(context);
		}
	}

	private void RemoveChild(IContext context)
	{
		string clusterName = context.DisplayName;
		RemoveChildContext(context);
		ServiceContainer.Container.Resolve<IDownClusterDataChangedService>(Array.Empty<object>()).NotifyDownClusterChanged(context.DisplayName, (DownClusterDataChangedType)1);
		MS.Internal.ServerClusters.Utilities.BackgroundDisposeObject(context);
		snapIn.Settings.RemoveConnectedCluster(clusterName);
	}

	public void AddChild(IContext context, bool selectNode)
	{
		Exceptions.ThrowIfNull((object)context, "context");
		if (disposed)
		{
			MS.Internal.ServerClusters.Utilities.BackgroundDisposeObject(context);
			return;
		}
		EventHandler selectNodeAction = null;
		selectNodeAction = delegate
		{
			snapIn.RootNode.ScopeNodeAdded -= selectNodeAction;
			if (selectNode)
			{
				Worker.Start(delegate
				{
					ScopeNode scopeNode = null;
					int num = 200;
					while (scopeNode == null && --num > 0)
					{
						scopeNode = snapIn.RootNode.FindChild(context.DisplayName);
						Thread.Sleep(100);
					}
					Thread.Sleep(1000);
					if (scopeNode != null)
					{
						ClusterAdministrator.RequestScopeNodeSelection(scopeNode);
					}
				});
			}
		};
		if (selectNode)
		{
			snapIn.RootNode.ScopeNodeAdded += selectNodeAction;
		}
		bool flag = true;
		lock (addChildLock)
		{
			IContext context2 = FindChildContext(context.DisplayName);
			if (context2 != null)
			{
				if (context2 is DownClusterContext && context is ClusterContext)
				{
					RemoveChild(context2);
				}
				else
				{
					MS.Internal.ServerClusters.Utilities.BackgroundDisposeObject(context);
					flag = false;
					context = context2;
				}
			}
			if (flag)
			{
				((ICloseable)context).Closed += OnChildClosed;
				AddChildContext(context, delayedAdd: false);
				ClusterContext clusterContext = context as ClusterContext;
				DownClusterContext downClusterContext = context as DownClusterContext;
				if (clusterContext != null)
				{
					if (!clusterContext.IsDisposed)
					{
						Cluster cluster = clusterContext.Cluster;
						if (cluster != null)
						{
							snapIn.Settings.AddConnectedCluster(cluster);
							clusterContext.ReportConnectionChanges = true;
						}
					}
				}
				else
				{
					ClusterDatabase clusterDatabase = downClusterContext.ClusterDatabase;
					ServiceContainer.Container.Resolve<IDownClusterDataChangedService>(Array.Empty<object>()).NotifyDownClusterChanged(clusterDatabase.FqdnClusterName, (DownClusterDataChangedType)0);
					snapIn.Settings.AddConnectedCluster(ConnectedClusterData.CreateDownClusterData(clusterDatabase));
					downClusterContext.ClusterConnectionChanged += DownClusterContext_ClusterConnectionChanged;
				}
			}
			if (selectNode && selectNodeAction != null)
			{
				selectNodeAction(this, EventArgs.Empty);
			}
		}
	}

	private void DownClusterContext_ClusterConnectionChanged(object sender, EventArgs e)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		if (SynchronizeInvoke.InvokeRequired)
		{
			SynchronizeInvoke.BeginInvoke((Delegate)(UIThreadHandlerV)delegate
			{
				DownClusterContext_ClusterConnectionChanged(sender, e);
			}, (object[])null);
		}
		else
		{
			AddChild((IContext)sender, selectNode: true);
		}
	}

	private void OnChildClosed(object sender, ChildDeletedEventArgs e)
	{
		RemoveChild(e.ChildName);
	}

	private void OnSettingsLoaded(object sender, EventArgs e)
	{
		if (!Utilities.IsDomainUser())
		{
			return;
		}
		DebugLog.LogInfo("Loading previously connected Clusters");
		foreach (ConnectedClusterData connectedCluster in snapIn.Settings.ConnectedClusters)
		{
			if (connectedCluster != null)
			{
				bool flag = connectedCluster.NodeNames.Any((string nodeName) => nodeName.StartsWith(Environment.MachineName, StringComparison.OrdinalIgnoreCase));
				if (connectedCluster.Connected && !flag)
				{
					QueueBackgroundConnectRequest(new BackgroundClusterConnectArgs(connectedCluster));
				}
			}
		}
	}

	private void BackgroundConnectToCluster(object data)
	{
		BackgroundClusterConnectArgs backgroundClusterConnectArgs = (BackgroundClusterConnectArgs)data;
		try
		{
			IContext context = null;
			bool flag = true;
			if (backgroundClusterConnectArgs.ConnectionData.ClusterName.Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase))
			{
				NodeClusterState clusterState = ClusterNode.GetClusterState(null);
				if (clusterState != NodeClusterState.Running && clusterState != NodeClusterState.NotRunning)
				{
					flag = false;
				}
			}
			if (flag)
			{
				context = ClusterConnectionFactory.ConnectToCluster(backgroundClusterConnectArgs.ConnectionData, ConnectionType.AnyConnection);
			}
			if (context != null)
			{
				if (!string.Equals(backgroundClusterConnectArgs.ConnectionData.ClusterName, context.DisplayName, StringComparison.OrdinalIgnoreCase))
				{
					snapIn.Settings.RemoveConnectedCluster(backgroundClusterConnectArgs.ConnectionData.ClusterName);
				}
				object[] array = new object[2] { context, backgroundClusterConnectArgs.SelectNode };
				SynchronizeInvoke.BeginInvoke((Delegate)new OpenComplete(AddChild), array);
			}
			else
			{
				snapIn.Settings.DecrementConnectedCluster(backgroundClusterConnectArgs.ConnectionData.ClusterName);
			}
		}
		catch (Exception ex)
		{
			if (backgroundClusterConnectArgs.ReportError)
			{
				ClusterAdministrator.NotifyUser.ShowError(ex, Resources.ClusterConnection_Failed_Text, new object[1] { backgroundClusterConnectArgs.ConnectionData.ClusterName });
			}
			else
			{
				ExceptionHelp.LogException(ex, "Error opening cluster {0} in background", backgroundClusterConnectArgs.ConnectionData.ClusterName);
			}
		}
	}

	public override void Dispose()
	{
		disposed = true;
		foreach (IContext childContext in GetChildContexts())
		{
			MS.Internal.ServerClusters.Utilities.BackgroundDisposeObject(childContext);
		}
		GC.SuppressFinalize(this);
	}
}
