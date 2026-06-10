using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Input;
using FailoverClusters.Framework;
using FailoverClusters.SnapIn;
using FailoverClusters.UI.Common;
using FailoverClusters.UIFramework;
using ManagementConsole;
using ManagementConsole.Advanced;

namespace KDDSL.ServerClusters.Management;

internal class DownClusterContext : ScopeNodeContextBase, ICloseable
{
	private const int ServiceWaitTimeout = 10000;

	private const int TimerInterval = 10000;

	private ActionBase startClusterAction;

	private ActionBase connectToClusterAction;

	private readonly object stateActionLockObject = new object();

	private readonly ClusterDatabase clusterDb;

	private readonly ClusterEventsContext eventsContext;

	private readonly List<DownNodeContext> childNodes;

	private volatile ActionsPaneItemCollection actions;

	private readonly object clusterConnectionLock = new object();

	private volatile ClusterContext clusterContext;

	private ForceClusterStartActionPaneItem forceClusterStartActionPaneItem;

	private UICommand forceClusterStartCommand;

	private System.Timers.Timer refreshTimer;

	private readonly object refreshTimerLock = new object();

	public ClusterDatabase ClusterDatabase => clusterDb;

	public ICollection<DownNodeContext> Nodes
	{
		get
		{
			lock (childNodes)
			{
				return new List<DownNodeContext>(childNodes).OrderBy((DownNodeContext ctx) => ctx.DisplayName).ToList();
			}
		}
	}

	public Guid InstanceId => clusterDb.ClusterInstanceId;

	public override ViewDescriptionCollection ViewDescriptions => new ViewDescriptionCollection { Utilities.CreateFormViewDescription(clusterDb.ClusterName, typeof(DownClusterStartPageControl)) };

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

	public bool IsClusterUp => WaitForClusterToAttainQuorum();

	public event EventHandler<ChildDeletedEventArgs> Closed;

	internal event EventHandler ContextRefreshed;

	public event EventHandler ClusterConnectionChanged;

	internal DownClusterContext(ClusterDatabase clusterDb)
		: base(new Guid("{A7A9E337-6A94-4da8-800E-1D352AE71063}"), ExpandIconOptions.DoNotShow)
	{
		childNodes = new List<DownNodeContext>();
		this.clusterDb = clusterDb;
		base.DisplayName = this.clusterDb.FqdnClusterName;
		base.ImageIndex = Icons.ClusterDownIndex;
		Utilities.VerifyUserIsAdminOnNodes(this.clusterDb.NodeNames, this.clusterDb.ClusterName);
		eventsContext = ContextFactory.CreateClusterEventsContext(clusterDb);
		AddChildContext(eventsContext, delayedAdd: true);
		refreshTimer = new System.Timers.Timer();
		refreshTimer.Elapsed += RefreshTimerCallback;
		refreshTimer.Interval = 10000.0;
		lock (childNodes)
		{
			foreach (DownNodeContext item in this.clusterDb.Nodes.Select(ContextFactory.CreateDownNodeContext))
			{
				childNodes.Add(item);
				item.ImageIndexChanged += NodeContextImageIndexChanged;
			}
		}
		refreshTimer.Start();
	}

	private void OnClosed()
	{
		EventHandler<ChildDeletedEventArgs> closed = this.Closed;
		if (closed != null)
		{
			ChildDeletedEventArgs e = new ChildDeletedEventArgs(base.DisplayName);
			closed(this, e);
		}
	}

	private void OnContextRefreshed()
	{
		this.ContextRefreshed?.Invoke(this, null);
	}

	public override void ClearActions()
	{
		lock (base.ActionsLock)
		{
			actions = Utilities.DisposeActions(actions);
		}
		lock (stateActionLockObject)
		{
			startClusterAction = null;
			connectToClusterAction = null;
			forceClusterStartActionPaneItem?.Dispose();
			forceClusterStartActionPaneItem = null;
			forceClusterStartCommand = null;
		}
		base.ClearActions();
	}

	protected override void UpdateStateBasedActions()
	{
		if (startClusterAction == null || forceClusterStartActionPaneItem == null || connectToClusterAction == null)
		{
			return;
		}
		int num = Nodes.Count((DownNodeContext node) => node.ServiceState == WindowsServiceState.StartPending || node.ServiceState == WindowsServiceState.StopPending);
		lock (stateActionLockObject)
		{
			if (num > 0)
			{
				startClusterAction.Enabled = false;
				connectToClusterAction.Enabled = false;
				forceClusterStartActionPaneItem.Enabled = false;
			}
			else
			{
				bool isClusterUp = IsClusterUp;
				ActionBase actionBase = startClusterAction;
				bool enabled = (forceClusterStartActionPaneItem.Enabled = !isClusterUp);
				actionBase.Enabled = enabled;
				connectToClusterAction.Enabled = isClusterUp;
			}
		}
		OnActionsUpdated();
	}

	private void RefreshTimerCallback(object sender, EventArgs args)
	{
		lock (refreshTimerLock)
		{
			if (refreshTimer == null)
			{
				return;
			}
			refreshTimer.Stop();
		}
		try
		{
			Refresh();
		}
		finally
		{
			OnContextRefreshed();
			lock (refreshTimerLock)
			{
				if (refreshTimer != null)
				{
					refreshTimer.Start();
				}
			}
		}
	}

	private ActionsPaneItemCollection CreateActions()
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Expected O, but got Unknown
		//IL_00eb: Expected O, but got Unknown
		ActionsPaneItemCollection actionsPaneItemCollection = new ActionsPaneItemCollection();
		lock (stateActionLockObject)
		{
			connectToClusterAction = ActionFactory.CreateAction(Resources.Connect_Text, Resources.Connect_Description_Text, Icons.ClusterIndex, ConnectToCluster);
			connectToClusterAction.Enabled = false;
			startClusterAction = ActionFactory.CreateAction(Resources.StartCluster_Text, Resources.StartCluster_Description_Text, Icons.StartClusterIndex, StartCluster);
			startClusterAction.Enabled = true;
			UICommand val = new UICommand("ForceClusterStart", (UICommandId)13, (Action<object>)delegate
			{
			}, (Predicate<object>)((object x) => false));
			((ClusterCommand)val).Text = ActionFactory.GenerateDisplayName(Resources.ForceClusterStart_Text);
			((ClusterCommand)val).Description = ActionFactory.GenerateDisplayName(Resources.ForceClusterStart_Description_Text);
			forceClusterStartCommand = val;
			forceClusterStartActionPaneItem = new ForceClusterStartActionPaneItem((ICommand)forceClusterStartCommand, this);
		}
		actionsPaneItemCollection.AddRange(new ActionsPaneItem[9]
		{
			SharedActions.CreateValidationAction(CommandResources.ValidateConfigurationAction_Text, OnValidate),
			SharedActions.CreateViewValidationReportAction(OnViewValidationReport),
			new ActionSeparator(),
			connectToClusterAction,
			new ActionSeparator(),
			startClusterAction,
			forceClusterStartActionPaneItem.Action,
			new ActionSeparator(),
			SharedActions.CreateCloseConnectionAction(CloseConnection)
		});
		return actionsPaneItemCollection;
	}

	private void CloseConnection(object sender, SnapinActionEventArgs e)
	{
		ClearClusterConnection();
		OnClosed();
	}

	private static void OnValidate(object sender, SnapinActionEventArgs e)
	{
		SharedActions.PerformValidationAction(ActionData.GetNotifyUserFromSender(sender));
	}

	private void OnViewValidationReport(object sender, SnapinActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		SharedActions.PerformViewValidationReportAction(clusterDb.NodeNames, notifyUserFromSender, e.Action.DisplayName);
	}

	private void ConnectToCluster(object sender, SnapinActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		AttemptToConnect(sender, notifyUserFromSender);
	}

	private void AttemptToConnect(object sender, INotifyUser notifyUser)
	{
		ConnectedClusterData connectionData = ConnectedClusterData.CreateDownClusterData(clusterDb);
		IContext clusterConnection = GetClusterConnection(notifyUser);
		IContext context = clusterConnection ?? ClusterConnectionFactory.ConnectToCluster(connectionData, notifyUser, ConnectionType.LiveClusterOnly);
		if (context != null)
		{
			SwitchContexts(sender, context);
		}
	}

	private ClusterContext GetClusterConnection(INotifyUser notifyUser)
	{
		CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.ClusterConnection_Prepare_Text, Resources.ClusterConnection_Prepare_Text);
		using (cluadminWaitDialog)
		{
			return cluadminWaitDialog.ShowDialog<object, ClusterContext>(notifyUser, GetClusterConnection, null);
		}
	}

	private ClusterContext GetClusterConnection(CluadminWaitDialog waitDialog, object data)
	{
		ClusterContext result = null;
		lock (clusterConnectionLock)
		{
			if (clusterContext != null)
			{
				result = clusterContext;
				clusterContext.Cluster.ConnectionChanged -= InternalClusterConnectionChanged;
				clusterContext = null;
			}
		}
		return result;
	}

	private void SwitchContexts(object sender, IContext context)
	{
		ScopeNode scopeNodeFromSender = GetScopeNodeFromSender(sender);
		if (typeof(SnapIn).IsAssignableFrom(scopeNodeFromSender.SnapIn.GetType()))
		{
			AddChildContext(((SnapIn)scopeNodeFromSender.SnapIn).RootNode, context);
		}
		else if (typeof(NamespaceExtension).IsAssignableFrom(scopeNodeFromSender.SnapIn.GetType()))
		{
			AddChildContext(((NamespaceExtension)scopeNodeFromSender.SnapIn).PrimaryNode.Children[0], context);
		}
		else
		{
			DebugLog.LogCritical(string.Format(CultureInfo.CurrentCulture, "SnapIn failed to switch context since the context from where it is executed in a unknown instance type '{0}'", scopeNodeFromSender.SnapIn.GetType()));
		}
	}

	private void AddChildContext(ScopeNode rootNode, IContext context)
	{
		RootContext obj = (RootContext)rootNode.Tag;
		OnClosed();
		obj.AddChild(context, selectNode: true);
	}

	private void StartCluster(object sender, SnapinActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		CluadminWaitDialog cluadminWaitDialog = e.CreateWaitDialog(Resources.StartingCluster_Text);
		AggregateException aggregateException = null;
		using (cluadminWaitDialog)
		{
			cluadminWaitDialog.ShowDialog(notifyUserFromSender, delegate(CluadminWaitDialog waitDialogCallback)
			{
				try
				{
					PerformStartCluster(waitDialogCallback);
				}
				catch (AggregateException ex)
				{
					aggregateException = ex;
				}
			});
			if (cluadminWaitDialog.IsCanceled)
			{
				return;
			}
		}
		if (aggregateException == null)
		{
			return;
		}
		foreach (Exception innerException in aggregateException.InnerExceptions)
		{
			ClusterLog.LogException(innerException, "There was an error starting the cluster");
		}
		if (aggregateException.InnerExceptions.Count > 0)
		{
			ClusterDialogException.ShowTaskDialogAsync(aggregateException.InnerExceptions[0]);
		}
	}

	private void PerformStartCluster(CluadminWaitDialog waitDialog)
	{
		ConcurrentBag<Exception> exceptions = new ConcurrentBag<Exception>();
		Parallel.ForEach(clusterDb.NodeNames, delegate(string nodeName)
		{
			try
			{
				if (NetworkHelper.CanPing(nodeName))
				{
					using (ServiceController serviceController = new ServiceController("Clussvc", nodeName))
					{
						serviceController.Refresh();
						if (serviceController.Status == ServiceControllerStatus.Stopped)
						{
							waitDialog.StatusText = Extensions.FormatCurrentCulture(Resources.StartingServiceOnNode_Text, (object)nodeName);
							serviceController.Start();
						}
						else
						{
							DebugLog.LogInfo("Down cluster page: Not starting clussvc on node {0} because the service state is {1}.", nodeName, serviceController.Status.ToString());
						}
						return;
					}
				}
			}
			catch (ArgumentException item)
			{
				exceptions.Add(item);
			}
			catch (Win32Exception item2)
			{
				exceptions.Add(item2);
			}
			catch (InvalidOperationException item3)
			{
				exceptions.Add(item3);
			}
		});
		if (exceptions.Count > 0)
		{
			throw new AggregateException(exceptions);
		}
		DisableStartActions();
	}

	private void DisableStartActions()
	{
		startClusterAction.Enabled = false;
		forceClusterStartActionPaneItem.Enabled = false;
		OnActionsUpdated();
	}

	private void RefreshNodeContexts()
	{
		lock (childNodes)
		{
			Parallel.ForEach(childNodes, delegate(DownNodeContext downNode)
			{
				downNode.Refresh();
			});
		}
	}

	internal void ForceClusterStart(object sender, ActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		if (DialogResult.Yes != notifyUserFromSender.ShowWindowsCodePackDialog(Resources.ForceClusterStart_Text, Resources.ForceClusterStart_ConfirmationDialogHeader, Resources.ForceClusterStartConfirmation_Text))
		{
			return;
		}
		using CluadminWaitDialog cluadminWaitDialog = e.CreateWaitDialog(Resources.ForcingClusterStart_Text);
		cluadminWaitDialog.DisplayDelay = TimeSpan.Zero;
		cluadminWaitDialog.CylonTime = TimeSpan.FromMinutes(5.0);
		cluadminWaitDialog.ShowDialog(notifyUserFromSender, PerformForceClusterStart);
		_ = cluadminWaitDialog.IsCanceled;
	}

	private void PerformForceClusterStart(CluadminWaitDialog waitDialog)
	{
		Parallel.ForEach(clusterDb.NodeNames, delegate(string nodeName)
		{
			try
			{
				waitDialog.StatusText = Extensions.FormatCurrentCulture(Resources.StoppingNode_Text, (object)nodeName);
				ClusterNode.StopClusterService(nodeName);
				ClusterNode.WaitForStoppingClusterService(nodeName, 10000);
			}
			catch (ApplicationException caughtException)
			{
				ExceptionHelp.LogException(caughtException, "Error stopping node {0}", nodeName);
			}
			catch (ClusterBaseException caughtException2)
			{
				ExceptionHelp.LogException(caughtException2, "Error stopping node {0}", nodeName);
			}
		});
		waitDialog.SetStatusText(Resources.ForcingClusterStart_Text);
		Cluster.ForceClusterStart(clusterDb.NodeNames);
		DisableStartActions();
		WaitForClusterToAttainQuorum();
	}

	public override void Refresh()
	{
		RefreshNodeContexts();
		UpdateStateBasedActions();
		if (connectToClusterAction != null && connectToClusterAction.Enabled)
		{
			ConnectToCluster();
		}
	}

	private void ConnectToCluster()
	{
		IContext context = ClusterConnectionFactory.ConnectToCluster(ConnectedClusterData.CreateDownClusterData(clusterDb), ConnectionType.LiveClusterOnly);
		lock (clusterConnectionLock)
		{
			clusterContext = (ClusterContext)context;
		}
		clusterContext.Cluster.ConnectionChanged += InternalClusterConnectionChanged;
		OnClusterConnectionChanged();
	}

	private bool WaitForClusterToAttainQuorum()
	{
		return Cluster.WaitForClusterToAttainQuorum(Nodes.Select((DownNodeContext downNode) => downNode.DisplayName).ToList(), 0);
	}

	public override void Dispose()
	{
		if (isDisposed)
		{
			return;
		}
		lock (refreshTimerLock)
		{
			refreshTimer.Stop();
			refreshTimer.Dispose();
			refreshTimer = null;
		}
		ClearActions();
		base.Dispose();
		ClearClusterConnection();
		lock (childNodes)
		{
			foreach (DownNodeContext childNode in childNodes)
			{
				childNode.ImageIndexChanged -= NodeContextImageIndexChanged;
				childNode.Dispose();
			}
			childNodes.Clear();
		}
		GC.SuppressFinalize(this);
	}

	private void InternalClusterConnectionChanged(object sender, ClusterConnectionEventArgs e)
	{
		if (e.ConnectionState != ClusterConnectionState.Disconnected)
		{
			return;
		}
		lock (clusterConnectionLock)
		{
			if (clusterContext != null)
			{
				ClearClusterConnection();
				OnClusterConnectionChanged();
			}
		}
	}

	private void ClearClusterConnection()
	{
		lock (clusterConnectionLock)
		{
			if (clusterContext != null)
			{
				clusterContext.Cluster.ConnectionChanged -= InternalClusterConnectionChanged;
				KDDSL.ServerClusters.Utilities.BackgroundDisposeObject(clusterContext);
				clusterContext = null;
			}
		}
	}

	protected virtual void OnClusterConnectionChanged()
	{
		EventHandler clusterConnectionChanged = this.ClusterConnectionChanged;
		if (clusterConnectionChanged != null)
		{
			IContext context = clusterContext;
			clusterContext = null;
			clusterConnectionChanged(context ?? this, EventArgs.Empty);
		}
	}

	private void NodeContextImageIndexChanged(object sender, EventArgs e)
	{
		Background.QueueWorker((WaitCallback)TestClusterConnection);
	}

	private void TestClusterConnection(object data)
	{
		lock (clusterConnectionLock)
		{
			if (clusterContext == null)
			{
				return;
			}
			try
			{
				clusterContext.Cluster.GetNodeCount();
			}
			catch (Exception caughtException)
			{
				Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(caughtException);
				if (firstException == null || firstException.NativeErrorCode != -2147023174)
				{
					ExceptionHelp.LogException(caughtException, "Error testing cluster connection");
				}
			}
		}
	}
}

