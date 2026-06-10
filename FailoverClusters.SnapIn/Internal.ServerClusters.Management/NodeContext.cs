using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using FailoverClusters.Framework;
using FailoverClusters.SnapIn;
using FailoverClusters.UI.Common;
using FailoverClusters.UI.Controls;
using FailoverClusters.UIFramework;
using ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal class NodeContext : ScopeNodeContextBase, IClusterSpecific, IViewContext
{
	private enum PauseType
	{
		Drain,
		NoDrain
	}

	private volatile ActionsPaneItemCollection actions;

	private bool actionsAreDirty = true;

	private readonly ClusterNode node;

	private readonly ClusterContext clusterContext;

	private ActionGroup pauseActionGroup;

	private ActionGroup resumeActionGroup;

	private ActionBase pauseDrainAction;

	private ActionBase pauseNoDrainAction;

	private ActionBase resumeFailbackAction;

	private ActionBase resumeNoFailbackAction;

	private ActionBase startAction;

	private ActionBase stopAction;

	private readonly object nodeStateActionLockObject = new object();

	private readonly object serviceStateActionLockObject = new object();

	private EvictNodeActionPaneItem evictActionPaneItem;

	private UICommand evictNodeCommand;

	public Cluster Cluster => clusterContext.Cluster;

	public override Guid Id => node.Id;

	public string[] DisplayColumns => new string[4]
	{
		Resources.Name_Text,
		Resources.Status_Text,
		Resources.GroupType_Text,
		Resources.AutoStartColumnHeader
	};

	public string EmptyMessage => Resources.List_NoNodeInformation_Text;

	public bool IsEnumerating => false;

	public override ViewDescriptionCollection ViewDescriptions => new ViewDescriptionCollection();

	public override ActionsPaneItemCollection ActionsPaneItems
	{
		get
		{
			if (node == null || node.IsDeleted || node.IsDeleting || isDisposed)
			{
				return new ActionsPaneItemCollection();
			}
			if (actionsAreDirty)
			{
				lock (base.ActionsLock)
				{
					actions = Utilities.DisposeActions(actions);
					actions = CreateActions();
				}
				actionsAreDirty = false;
			}
			return actions;
		}
	}

	public ClusterNode Node => node;

	public ClusterContext ClusterContext => clusterContext;

	internal NodeContext(ClusterNode node, ClusterContext clusterContext)
		: base(ClusterAdministrator.NodeContextGuid, ExpandIconOptions.Show)
	{
		this.clusterContext = clusterContext;
		this.node = node;
		base.DisplayName = node.Name;
		base.ImageIndex = IconsHelp.GetNodeIconIndex(this.node.State);
		this.node.Deleting += NodeDeleting;
		this.node.StateChanged += OnStateChanged;
		this.node.PropertiesChanged += OnPropertiesChanged;
	}

	public override void Dispose()
	{
		if (!isDisposed)
		{
			base.Dispose();
			ClearActions();
			node.Deleting -= NodeDeleting;
			node.StateChanged -= OnStateChanged;
			node.PropertiesChanged -= OnPropertiesChanged;
			GC.SuppressFinalize(this);
		}
	}

	public override void Refresh()
	{
		base.DisplayName = node.Name;
		base.ImageIndex = IconsHelp.GetNodeIconIndex(node.State);
		UpdateStateBasedActions();
	}

	private void NodeDeleting(object sender, DeletingEventArgs e)
	{
		OnDeleting(e.Stage);
	}

	private void OnPropertiesChanged(object sender, EventArgs e)
	{
		base.DisplayName = node.Name;
		UpdateStateBasedActions();
		OnActionsUpdated();
	}

	private void OnStateChanged(object sender, EventArgs e)
	{
		base.ImageIndex = IconsHelp.GetNodeIconIndex(node.State);
		UpdateStateBasedActions();
		OnActionsUpdated();
	}

	public override void ClearActions()
	{
		lock (base.ActionsLock)
		{
			actions = Utilities.DisposeActions(actions);
		}
		lock (nodeStateActionLockObject)
		{
			evictActionPaneItem?.Dispose();
			evictActionPaneItem = null;
			evictNodeCommand = null;
			resumeActionGroup = Utilities.DisposeActions(resumeActionGroup);
			pauseActionGroup = Utilities.DisposeActions(pauseActionGroup);
			resumeNoFailbackAction = null;
			resumeFailbackAction = null;
			pauseNoDrainAction = null;
			pauseDrainAction = null;
		}
		lock (serviceStateActionLockObject)
		{
			startAction = null;
			stopAction = null;
		}
		actionsAreDirty = true;
		base.ClearActions();
	}

	private void CreateStateActions()
	{
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Expected O, but got Unknown
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Expected O, but got Unknown
		//IL_01af: Expected O, but got Unknown
		lock (nodeStateActionLockObject)
		{
			pauseActionGroup = new ActionGroup(StringExtensions.ReplaceAccelerator(CommandResources.PauseAction_Text), CommandResources.PauseNodeActionDescription_Text, Icons.PauseNodeIndex);
			resumeActionGroup = new ActionGroup(StringExtensions.ReplaceAccelerator(CommandResources.ResumeNodeAction_Text), CommandResources.ResumeNodeActionDescription_Text, Icons.ResumeNodeIndex);
			pauseDrainAction = ActionFactory.CreateAction(StringExtensions.ReplaceAccelerator(CommandResources.PauseNodeDrainAction_Text), CommandResources.PauseNodeDrainActionDescription_Text, Icons.PauseNodeIndex, PauseNodeDrain);
			pauseNoDrainAction = ActionFactory.CreateAction(StringExtensions.ReplaceAccelerator(CommandResources.PauseNodeNoDrainAction_Text), CommandResources.PauseNodeNoDrainActionDescription_Text, Icons.PauseNodeIndex, PauseNodeNoDrain);
			resumeFailbackAction = ActionFactory.CreateAction(StringExtensions.ReplaceAccelerator(CommandResources.ResumeNodeFailbackAction_Text), CommandResources.ResumeNodeFailbackActionDescription_Text, Icons.ResumeNodeIndex, ResumeNodeFailback);
			resumeNoFailbackAction = ActionFactory.CreateAction(StringExtensions.ReplaceAccelerator(CommandResources.ResumeNodeNoFailbackAction_Text), CommandResources.ResumeNodeNoFailbackActionDescription_Text, Icons.ResumeNodeIndex, ResumeNodeNoFailback);
			pauseActionGroup.Items.AddRange(new ActionsPaneItem[2] { pauseDrainAction, pauseNoDrainAction });
			resumeActionGroup.Items.AddRange(new ActionsPaneItem[2] { resumeFailbackAction, resumeNoFailbackAction });
		}
		UICommand val = new UICommand("EvictNode", (UICommandId)12, (Action<object>)delegate
		{
		}, (Predicate<object>)delegate
		{
			NodeState safeNodeState = node.SafeNodeState;
			ClusterNodeDrainStatus drainStatus = node.DrainStatus;
			return (safeNodeState == NodeState.Up || safeNodeState == NodeState.Down) && drainStatus == ClusterNodeDrainStatus.NotInitiated;
		});
		((ClusterCommand)val).Text = CommandResources.EvictNodeAction_Text;
		((ClusterCommand)val).Description = ActionFactory.GenerateDisplayName(Resources.EvictNodeActionDescription_Text);
		evictNodeCommand = val;
		evictActionPaneItem = new EvictNodeActionPaneItem((ICommand)evictNodeCommand, this);
		lock (serviceStateActionLockObject)
		{
			startAction = SharedActions.CreateStartServiceAction(OnStartService);
			startAction.Enabled = false;
			stopAction = SharedActions.CreateStopServiceAction(OnStopService);
			stopAction.Enabled = false;
		}
		UpdateStateBasedActions();
		Background.QueueWorker((WaitCallback)InitialServiceStateActionUpdate);
	}

	private void InitialServiceStateActionUpdate(object data)
	{
		UpdateStateBasedActions();
		OnActionsUpdated();
	}

	private ActionsPaneItemCollection CreateActions()
	{
		ActionGroup actionGroup = new ActionGroup(StringExtensions.ReplaceAccelerator(CommandResources.MoreActions), Resources.MoreActionsActionsGroupDescription_Text, Icons.BlueArrowIndex);
		CreateStateActions();
		actionGroup.Items.AddRange(new ActionsPaneItem[4]
		{
			startAction,
			stopAction,
			new ActionSeparator(),
			evictActionPaneItem.Action
		});
		ActionsPaneItemCollection actionsPaneItemCollection = new ActionsPaneItemCollection();
		actionsPaneItemCollection.AddRange(new ActionsPaneItem[7]
		{
			pauseActionGroup,
			resumeActionGroup,
			new ActionSeparator(),
			ActionFactory.CreateAction(StringExtensions.ReplaceAccelerator(CommandResources.ConnectToNode_Text), Resources.ConnectNodeActionDescription_Text, Icons.RemoteDesktopIndex, OnConnectNode),
			ActionFactory.CreateAction(StringExtensions.ReplaceAccelerator(CommandResources.ShowCriticalEvents), Resources.ShowNodeCriticalEventsActionDescription_Text, Icons.ClusterEventsIndex, OnShowCriticalEvents),
			new ActionSeparator(),
			actionGroup
		});
		return actionsPaneItemCollection;
	}

	protected override void UpdateStateBasedActions()
	{
		if (startAction == null)
		{
			return;
		}
		lock (nodeStateActionLockObject)
		{
			NodeState safeNodeState = node.SafeNodeState;
			ClusterNodeDrainStatus drainStatus = node.DrainStatus;
			pauseNoDrainAction.Enabled = safeNodeState == NodeState.Up && drainStatus == ClusterNodeDrainStatus.NotInitiated;
			pauseDrainAction.Enabled = (safeNodeState == NodeState.Up && drainStatus == ClusterNodeDrainStatus.NotInitiated) || (safeNodeState == NodeState.Paused && drainStatus == ClusterNodeDrainStatus.Failed);
			resumeFailbackAction.Enabled = safeNodeState == NodeState.Paused;
			resumeNoFailbackAction.Enabled = safeNodeState == NodeState.Paused;
			((ClusterCommand)(object)evictNodeCommand)?.CanExecuteUpdate((object)this, new EventArgs());
		}
		lock (serviceStateActionLockObject)
		{
			if (startAction != null)
			{
				startAction.Enabled = node.State == NodeState.Down || node.ServiceState == WindowsServiceState.Stopped;
			}
			if (stopAction != null)
			{
				stopAction.Enabled = node.State != NodeState.Down && node.ServiceState == WindowsServiceState.Running;
			}
		}
	}

	private void OnShowCriticalEvents(object sender, SnapinActionEventArgs e)
	{
		LegacyFactory.ExecuteCriticalEventsDialog(Node.Cluster, base.DisplayName, base.NodeType, delegate(EventLogFilter filter)
		{
			filter.ClusterNode = node.Name;
		}, null);
	}

	private void OnConnectNode(object sender, SnapinActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		string nodeName = null;
		using (CluadminWaitDialog cluadminWaitDialog = e.CreateWaitDialog(Resources.RetrievingNodeName_Text))
		{
			cluadminWaitDialog.ShowDialog(notifyUserFromSender, delegate
			{
				nodeName = Node.FqdnName;
			});
		}
		if (string.IsNullOrWhiteSpace(nodeName))
		{
			return;
		}
		string arguments = string.Format(CultureInfo.CurrentCulture, "/v:{0}", nodeName);
		ProcessStartInfo startInfo = new ProcessStartInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "mstsc.exe"), arguments)
		{
			ErrorDialog = true,
			WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Windows)
		};
		try
		{
			UIHelper.ApplicationActivate(Process.Start(startInfo));
		}
		catch (Win32Exception ex)
		{
			ClusterLog.LogException((Exception)ex, "An error occurred opening a remote desktop connection to node {0}", new object[1] { nodeName });
		}
		catch (FileNotFoundException ex2)
		{
			ClusterLog.LogException((Exception)ex2, "An error occurred opening a remote desktop connection to node {0}", new object[1] { nodeName });
		}
	}

	internal void Evict(object sender, ActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		string text = StringExtensions.RemoveAccelerator(CommandResources.EvictNodeAction_Text);
		try
		{
			VerifyActionCanBePerformedData verifyActionData = ActionVerification.BuildVerifyActionData(VerifyAction.QuorumLoss, Extensions.FormatCurrentCulture(Resources.ClusterShutdownByNodeEvict_Text, (object)node.Name), QuorumVoterActionCheck.Evict, text, Resources.ConfirmNodeEvictionContent_Text);
			if (VerifyActionCanBePerformed(notifyUserFromSender, verifyActionData, new ConfirmationMessage(Extensions.FormatCurrentCulture(Resources.EvictNodeActionConfirm_Text, (object)node.Name))) && notifyUserFromSender.ShowWindowsCodePackDialog(verifyActionData.ActionData.Name, Extensions.FormatCurrentCulture(Resources.EvictNode_ConfirmationDialogHeader, (object)node.Name), verifyActionData.ActionData.ConfirmationMessage) == DialogResult.Yes)
			{
				CluadminWaitDialog cluadminWaitDialog = e.CreateWaitDialog(Resources.EvictingNode_Text, node.Name);
				cluadminWaitDialog.CylonTime = ClusterNode.DefaultEvictTimeout;
				cluadminWaitDialog.DisplayDelay = TimeSpan.FromSeconds(10.0);
				cluadminWaitDialog.Title = text;
				using (cluadminWaitDialog)
				{
					cluadminWaitDialog.ShowDialog(notifyUserFromSender, PerformEvict);
				}
				if (cluadminWaitDialog.IsCanceled)
				{
					node.DeleteCanceled();
				}
				else
				{
					ReportDeleteToSender(sender);
				}
			}
		}
		catch (ClusterFailedCleanupEvictNodeException caughtException)
		{
			ExceptionHelp.LogException(caughtException, "The cluster node was evicted but the cleanup operation failed");
			notifyUserFromSender.ShowWindowsCodePackWarning(Resources.NodeEvictWithoutCleanup_Text, new object[1] { Node.Name });
		}
		catch (Exception caughtException2)
		{
			Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(caughtException2);
			if (firstException != null && firstException.NativeErrorCode == -2147019000)
			{
				ExceptionHelp.LogException(caughtException2, "The cluster node was evicted but the cleanup operation failed");
				notifyUserFromSender.ShowWindowsCodePackWarning(Resources.NodeEvictWithoutCleanup_Text, new object[1] { Node.Name });
			}
			else if (ExceptionHelp.IsFirstExceptionFound<TimeoutException>(caughtException2))
			{
				ExceptionHelp.LogException(caughtException2, "Timeout evicting node");
				notifyUserFromSender.ShowWindowsCodePackWarning(Resources.NodeEvictTimeout_Text, new object[1] { Node.Name });
			}
			else if (!ExceptionHelp.IsFirstExceptionFound<OperationCanceledException>(caughtException2))
			{
				throw;
			}
		}
	}

	private void PerformEvict(CluadminWaitDialog waitDialog)
	{
		node.Evict(ClusterNode.DefaultEvictTimeout, "snapin!EvictNodeActionPaneItem.PerformEvict");
	}

	private void OnStopService(object sender, SnapinActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		string actionName = StringExtensions.RemoveAccelerator(CommandResources.StopServiceAction_Text);
		try
		{
			VerifyActionCanBePerformedData verifyActionData = ActionVerification.BuildVerifyActionData(VerifyAction.QuorumLoss | VerifyAction.HostedGroups, Extensions.FormatCurrentCulture(Resources.ClusterShutdownByNodeStop_Text, (object)node.Name), QuorumVoterActionCheck.Down, actionName, Extensions.FormatCurrentCulture(Resources.StopServiceActionConfirmationFormat_Text, (object)node.Name));
			if (VerifyActionCanBePerformed(notifyUserFromSender, verifyActionData, new ConfirmationMessage(Extensions.FormatCurrentCulture(Resources.StopServiceActionConfirm_Text, (object)node.Name))))
			{
				using (CluadminWaitDialog cluadminWaitDialog = e.CreateWaitDialog(Resources.StoppingNode_Text, node.Name))
				{
					cluadminWaitDialog.ShowDialog(notifyUserFromSender, PerformStopService);
					return;
				}
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
		node.StopService();
		UpdateStateBasedActions();
	}

	private void OnStartService(object sender, SnapinActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		try
		{
			CluadminWaitDialog cluadminWaitDialog = e.CreateWaitDialog(Resources.StartingNode_Text, node.Name);
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
		node.StartService();
		UpdateStateBasedActions();
	}

	private void PauseNodeDrain(object sender, SnapinActionEventArgs e)
	{
		PauseNode(sender, e, PauseType.Drain);
	}

	private void PauseNodeNoDrain(object sender, SnapinActionEventArgs e)
	{
		PauseNode(sender, e, PauseType.NoDrain);
	}

	private void PauseNode(object sender, SnapinActionEventArgs e, PauseType pauseType)
	{
		bool drain = pauseType == PauseType.Drain;
		ClusterNodePauseExFlags flags = ((pauseType == PauseType.Drain) ? ClusterNodePauseExFlags.RemainOnPausedNodeOnMoveError : ClusterNodePauseExFlags.None);
		Utilities.StartInBackground(delegate
		{
			node.PauseEx(drain, (string)null, flags, "snapin!EvictNodeActionPaneItem.PauseNode");
		});
	}

	private void ResumeNodeFailback(object sender, SnapinActionEventArgs e)
	{
		ResumeNode(sender, e, ClusterNodeResumeFailbackType.FailbackGroupsImmediately, 0u);
	}

	private void ResumeNodeNoFailback(object sender, SnapinActionEventArgs e)
	{
		ResumeNode(sender, e, ClusterNodeResumeFailbackType.DoNotFailbackGroups, 0u);
	}

	private void ResumeNode(object sender, SnapinActionEventArgs e, ClusterNodeResumeFailbackType failbackType, uint flags)
	{
		Utilities.StartInBackground(delegate
		{
			node.ResumeEx(failbackType, flags, "snapin!EvictNodeActionPaneItem.ResumeNode");
		});
	}

	private bool VerifyActionCanBePerformed(INotifyUser notifyUser, VerifyActionCanBePerformedData verifyActionData, ConfirmationMessage confirmText)
	{
		VerifyAction verified;
		bool isCanceled;
		using (CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(verifyActionData.ActionData.Name, Resources.Group_VerifyActionCanBePerformed_Text))
		{
			verified = cluadminWaitDialog.ShowDialog(notifyUser, PerformVerifyActionCanBePerformed, verifyActionData);
			isCanceled = cluadminWaitDialog.IsCanceled;
		}
		return ActionVerification.ProcessVerifyActionResult(notifyUser, verifyActionData, isCanceled, verified, confirmText);
	}

	private VerifyAction PerformVerifyActionCanBePerformed(CluadminWaitDialog waitDialog, VerifyActionCanBePerformedData verifyData)
	{
		VerifyAction verifyAction = VerifyAction.None;
		if ((verifyData.Verifications & VerifyAction.QuorumLoss) == VerifyAction.QuorumLoss && PerformWillLoseQuourm(verifyData.QuorumData.QuorumVoterAction))
		{
			verifyAction |= VerifyAction.QuorumLoss;
		}
		else if ((verifyData.Verifications & VerifyAction.HostedGroups) == VerifyAction.HostedGroups && Node.GetGroupCount() > 0)
		{
			verifyAction |= VerifyAction.HostedGroups;
		}
		return verifyAction;
	}

	private bool PerformWillLoseQuourm(QuorumVoterActionCheck action)
	{
		bool result = false;
		switch (action)
		{
		case QuorumVoterActionCheck.Down:
			result = node.WillDownLoseQuorum();
			break;
		case QuorumVoterActionCheck.Evict:
			result = node.WillEvictLoseQuorum();
			break;
		default:
			DebugLog.LogWarning("Unknown quorum voter action");
			break;
		}
		return result;
	}
}

