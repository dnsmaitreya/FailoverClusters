using System;
using System.ServiceProcess;
using System.Threading;
using FailoverClusters.UI.Common;
using ManagementConsole;

namespace KDDSL.ServerClusters.Management;

internal class DownNodeContext : ContextBase, IRefreshable
{
	private readonly ClusterDatabaseNode nodeInfo;

	private ServiceController serviceController;

	private ActionsPaneItemCollection actions;

	private ActionBase startAction;

	private ActionBase stopAction;

	private WindowsServiceState? serviceState;

	private readonly object stateActionLockObject = new object();

	public Guid InstanceId => nodeInfo.InstanceId;

	public WindowsServiceState ServiceState => GetServiceState();

	public override ActionsPaneItemCollection ActionsPaneItems
	{
		get
		{
			if (isDisposed)
			{
				return new ActionsPaneItemCollection();
			}
			return actions ?? (actions = CreateActions());
		}
	}

	public event EventHandler ServiceStateChanged;

	internal DownNodeContext(ClusterDatabaseNode nodeInfo)
		: base(new Guid("{DB583C53-931F-4db6-81DB-2605FFAA6A4B}"))
	{
		this.nodeInfo = nodeInfo;
		base.DisplayName = this.nodeInfo.Name;
		actions = CreateActions();
		base.ImageIndex = Icons.NodeDownIndex;
		Background.QueueWorker((WaitCallback)InitialUpdateServiceStateActions);
	}

	private int DeterminImageIndex()
	{
		switch (ServiceState)
		{
		case WindowsServiceState.StartPending:
		case WindowsServiceState.StopPending:
		case WindowsServiceState.ContinuePending:
		case WindowsServiceState.PausePending:
			return Icons.NodePendingIndex;
		case WindowsServiceState.Paused:
			return Icons.NodePausedIndex;
		case WindowsServiceState.Running:
			return Icons.NodeIndex;
		default:
			return Icons.NodeDownIndex;
		}
	}

	private WindowsServiceState GetServiceState()
	{
		WindowsServiceState? windowsServiceState = serviceState;
		if (windowsServiceState.HasValue)
		{
			return windowsServiceState.Value;
		}
		if (NetworkHelper.CanPing(nodeInfo.Name))
		{
			try
			{
				if (serviceController == null)
				{
					serviceController = new ServiceController(ClusterNode.ServiceName, nodeInfo.FqdnName);
				}
				serviceController.Refresh();
				windowsServiceState = (serviceState = (WindowsServiceState)serviceController.Status);
				return windowsServiceState.Value;
			}
			catch (InvalidOperationException)
			{
				if (serviceController != null)
				{
					serviceController.Dispose();
					serviceController = null;
				}
			}
		}
		windowsServiceState = (serviceState = WindowsServiceState.Stopped);
		return windowsServiceState.Value;
	}

	public override void ClearActions()
	{
		actions = Utilities.DisposeActions(actions);
		lock (stateActionLockObject)
		{
			startAction = null;
			stopAction = null;
		}
		base.ClearActions();
	}

	private ActionsPaneItemCollection CreateActions()
	{
		ActionsPaneItemCollection actionsPaneItemCollection = new ActionsPaneItemCollection();
		lock (stateActionLockObject)
		{
			startAction = SharedActions.CreateStartServiceAction(StartService);
			startAction.Enabled = false;
			stopAction = SharedActions.CreateStopServiceAction(StopService);
			stopAction.Enabled = false;
		}
		actionsPaneItemCollection.Add(startAction);
		actionsPaneItemCollection.Add(stopAction);
		return actionsPaneItemCollection;
	}

	private void InitialUpdateServiceStateActions(object data)
	{
		UpdateStateBasedActions();
		OnActionsUpdated();
	}

	protected override void UpdateStateBasedActions()
	{
		lock (stateActionLockObject)
		{
			if (startAction != null)
			{
				startAction.Enabled = ServiceState == WindowsServiceState.Stopped;
			}
			if (stopAction != null)
			{
				stopAction.Enabled = ServiceState == WindowsServiceState.Running;
			}
		}
	}

	private void StartService(object sender, SnapinActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		try
		{
			CluadminWaitDialog cluadminWaitDialog = e.CreateWaitDialog(Resources.StartingNode_Text, base.DisplayName);
			using (cluadminWaitDialog)
			{
				cluadminWaitDialog.ShowDialog(notifyUserFromSender, PerformStartService);
			}
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Error starting service");
			notifyUserFromSender.ShowError(ex, Resources.StartServiceFailed_Text);
		}
	}

	private void PerformStartService(CluadminWaitDialog waitDialog)
	{
		startAction.Enabled = false;
		ClusterNode.StartClusterService(nodeInfo.FqdnName);
	}

	private void StopService(object sender, SnapinActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		try
		{
			CluadminWaitDialog cluadminWaitDialog = e.CreateWaitDialog(Resources.StoppingNode_Text, base.DisplayName);
			using (cluadminWaitDialog)
			{
				cluadminWaitDialog.ShowDialog(notifyUserFromSender, PerformStopService);
			}
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Error stopping service");
			notifyUserFromSender.ShowError(ex, Resources.StopServiceFailed_Text);
		}
	}

	private void PerformStopService(CluadminWaitDialog waitDialog)
	{
		stopAction.Enabled = false;
		ClusterNode.StopClusterService(nodeInfo.FqdnName);
	}

	private void OnServiceStateChanged()
	{
		base.ImageIndex = DeterminImageIndex();
		UpdateStateBasedActions();
		OnActionsUpdated();
		if (this.ServiceStateChanged != null)
		{
			this.ServiceStateChanged(this, EventArgs.Empty);
		}
	}

	public void Refresh()
	{
		WindowsServiceState? windowsServiceState = serviceState;
		serviceState = null;
		if (GetServiceState() != windowsServiceState)
		{
			OnServiceStateChanged();
		}
	}

	public override void Dispose()
	{
		if (!isDisposed)
		{
			ClearActions();
			if (serviceController != null)
			{
				serviceController.Dispose();
				serviceController = null;
			}
			base.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}

