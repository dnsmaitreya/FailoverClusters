using System;
using System.Windows.Input;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using FailoverClusters.UIFramework;
using ManagementConsole;
using MS.Internal.ServerClusters;

namespace FailoverClusters.ClusterSnapIn;

public class MmcActionPaneItem : IDisposable
{
	private readonly WeakReferenceEx clusterCommandRef;

	private ActionsPaneItem actionsPaneItem;

	private bool disposed;

	public ActionsPaneItem Action => actionsPaneItem;

	protected ICommand Command
	{
		get
		{
			object target = clusterCommandRef.Target;
			if (target == null)
			{
				return null;
			}
			if (target is ClusterCommand result)
			{
				return result;
			}
			UIProxyCommand val = (UIProxyCommand)((target is UIProxyCommand) ? target : null);
			if (val != null)
			{
				return (ICommand)val;
			}
			return null;
		}
	}

	public MmcActionPaneItem(ICommand command)
	{
		if (command == null)
		{
			throw new ArgumentNullException("command");
		}
		clusterCommandRef = new WeakReferenceEx(command);
		command.CanExecuteChanged += ClusterCommandCanExecuteChanged;
		UICommand val = (UICommand)((command is UICommand) ? command : null);
		if (val != null)
		{
			ManagementConsole.Action action = new ManagementConsole.Action(StringExtensions.ReplaceAccelerator(((ClusterCommand)(object)val).Text), string.Empty, ConvertCommandIdToImageIndex(val))
			{
				Enabled = ((ClusterCommand)(object)val).CanExecute(((ClusterCommand)(object)val).CommandParameter)
			};
			action.Triggered += ConsoleActionTriggered;
			actionsPaneItem = action;
			return;
		}
		if (command is ClusterCommandContainer clusterCommandContainer)
		{
			ActionGroup actionGroup = new ActionGroup(StringExtensions.ReplaceAccelerator(clusterCommandContainer.Text), string.Empty, ConvertCommandIdToImageIndex((ClusterCommand)clusterCommandContainer));
			foreach (ClusterCommand child in clusterCommandContainer.Children)
			{
				if (child is ClusterCommandContainer clusterCommandContainer2 && clusterCommandContainer2.Children.Count > 0)
				{
					ActionGroup actionGroup2 = new ActionGroup(StringExtensions.ReplaceAccelerator(child.Text), string.Empty, ConvertCommandIdToImageIndex(child));
					foreach (ClusterCommand child2 in clusterCommandContainer2.Children)
					{
						actionGroup2.Items.Add(new MmcActionPaneItem(child2).Action);
					}
					actionGroup.Items.Add(actionGroup2);
				}
				else
				{
					actionGroup.Items.Add(new MmcActionPaneItem(child).Action);
				}
			}
			actionsPaneItem = actionGroup;
			return;
		}
		if (command is ClusterCommand clusterCommand2)
		{
			ManagementConsole.Action action2 = new ManagementConsole.Action(StringExtensions.ReplaceAccelerator(clusterCommand2.Text), string.Empty, ConvertCommandIdToImageIndex(clusterCommand2))
			{
				Enabled = clusterCommand2.CanExecute(null)
			};
			action2.Triggered += ConsoleActionTriggered;
			actionsPaneItem = action2;
			return;
		}
		UIProxyCommandWithParameter val2 = (UIProxyCommandWithParameter)((command is UIProxyCommandWithParameter) ? command : null);
		if (val2 != null)
		{
			ManagementConsole.Action action3 = new ManagementConsole.Action(StringExtensions.ReplaceAccelerator(((UIProxyCommand)val2).Text), string.Empty, ConvertCommandIdToImageIndex((UIProxyCommand)(object)val2))
			{
				Enabled = ((UIProxyCommand)val2).CanExecute(((UIProxyCommand)val2).CommandParameter)
			};
			action3.Triggered += ConsoleActionTriggered;
			actionsPaneItem = action3;
			return;
		}
		UIProxyCommand val3 = (UIProxyCommand)((command is UIProxyCommand) ? command : null);
		if (val3 == null)
		{
			return;
		}
		if (val3.Children != null && val3.Children.Count > 0)
		{
			ActionGroup actionGroup3 = new ActionGroup(StringExtensions.ReplaceAccelerator(val3.Text), string.Empty, ConvertCommandIdToImageIndex(val3));
			foreach (ICommand child3 in val3.Children)
			{
				if (child3 is UISeparator)
				{
					actionGroup3.Items.Add(new ActionSeparator());
					continue;
				}
				UIProxyCommand val4 = (UIProxyCommand)((child3 is UIProxyCommand) ? child3 : null);
				if (val4 != null && val4.Children != null && val4.Children.Count > 0)
				{
					ActionGroup actionGroup4 = new ActionGroup(StringExtensions.ReplaceAccelerator(val4.Text), string.Empty, ConvertCommandIdToImageIndex(val4));
					foreach (ICommand child4 in val4.Children)
					{
						actionGroup4.Items.Add(new MmcActionPaneItem(child4).Action);
					}
					actionGroup3.Items.Add(actionGroup4);
				}
				else
				{
					actionGroup3.Items.Add(new MmcActionPaneItem(child3).Action);
				}
			}
			_ = val3.Children.Count;
			actionsPaneItem = actionGroup3;
		}
		else
		{
			ManagementConsole.Action action4 = new ManagementConsole.Action(StringExtensions.ReplaceAccelerator(val3.Text), string.Empty, ConvertCommandIdToImageIndex(val3))
			{
				Enabled = val3.CanExecute(val3.CommandParameter)
			};
			action4.Triggered += ConsoleActionTriggered;
			actionsPaneItem = action4;
		}
	}

	private static int ConvertCommandIdToImageIndex(ClusterCommand command)
	{
		return FindImageIndex(command.Id);
	}

	private static int ConvertCommandIdToImageIndex(UIProxyCommand command)
	{
		return FindImageIndex((ClusterCommandId)command.Id);
	}

	private static int FindImageIndex(ClusterCommandId id)
	{
		switch (id)
		{
		case ClusterCommandId.GroupOnline:
		case ClusterCommandId.MultipleGroupOnline:
			return Icons.BringGroupOnlineIndex;
		case ClusterCommandId.GroupOffline:
		case ClusterCommandId.MultipleGroupOffline:
			return Icons.TakeGroupOfflineIndex;
		case ClusterCommandId.GroupMoveTo:
		case ClusterCommandId.MultipleGroupMove:
		case ClusterCommandId.MultipleCsvMoveTo:
		case ClusterCommandId.MultipleStoragePoolResourceMoveTo:
			return Icons.MoveIndex;
		case ClusterCommandId.GroupPriority:
		case ClusterCommandId.MultipleGroupPriority:
			return Icons.PartialOnlineIndex;
		case ClusterCommandId.GroupProperties:
		case ClusterCommandId.ResourceProperties:
		case ClusterCommandId.FileShareProperties:
		case ClusterCommandId.NetworkProperties:
		case ClusterCommandId.ClusterProperties:
			return Icons.PropertiesIndex;
		case ClusterCommandId.GroupAddResource:
		case ClusterCommandId.GroupAddResourceItem:
		case ClusterCommandId.GroupAddMoreResource:
			return Icons.AddResourceIndex;
		case ClusterCommandId.GroupAddStorage:
			return Icons.AddDiskIndex;
		case ClusterCommandId.FileShareStopSharing:
			return Icons.DeleteIndex;
		case ClusterCommandId.GroupRefresh:
		case ClusterCommandId.FileShareRefreshShare:
		case ClusterCommandId.GCCollect:
		case ClusterCommandId.RefreshPoolPhysicalDisks:
		case ClusterCommandId.RefreshCluster:
			return Icons.RefreshIndex;
		case ClusterCommandId.StorageAddToClusterSharedVolume:
		case ClusterCommandId.MultipleStorageResourceAddToClusterSharedVolumes:
			return Icons.NewStorageIndex;
		case ClusterCommandId.StoragePoolCreateDisks:
			return Icons.NewSpaceActionIndex;
		case ClusterCommandId.StoragePoolAddDisks:
			return Icons.AddSpaceActionIndex;
		case ClusterCommandId.GroupDelete:
		case ClusterCommandId.ResourceDelete:
		case ClusterCommandId.MultipleGroupDelete:
		case ClusterCommandId.MultipleResourceDelete:
			return Icons.DeleteIndex;
		case ClusterCommandId.StorageRemoveFromGroup:
		case ClusterCommandId.MultipleStorageResourceRemove:
			return Icons.RemoveIndex;
		case ClusterCommandId.StorageChangeDriveLetter:
		case ClusterCommandId.StorageRepair:
		case ClusterCommandId.StorageTurnOnMaintenanceMode:
		case ClusterCommandId.StorageTurnoffMaintenanceMode:
		case ClusterCommandId.MultipleStorageResourceTurnOnMaintenanceMode:
		case ClusterCommandId.MultipleStorageResourceTurnoffMaintenanceMode:
		case ClusterCommandId.ClusterShareVolumeTurnOnRedirectedAccess:
		case ClusterCommandId.ClusterShareVolumeTurnoffRedirectedAccess:
			return Icons.PhysicalDiskIndex;
		case ClusterCommandId.ShowCriticalEvent:
			return Icons.ClusterEventsIndex;
		case ClusterCommandId.GroupShowDependencyReport:
			return Icons.DependencyReportIndex;
		case ClusterCommandId.ReplicationGroup:
			return Icons.ReplicationGroupResourceIndex;
		case ClusterCommandId.ReplicationNew:
			return Icons.ReplicationNewResourceIndex;
		case ClusterCommandId.ReplicationAdd:
			return Icons.ReplicationAddResourceIndex;
		case ClusterCommandId.ReplicationRemove:
			return Icons.ReplicationRemoveResourceIndex;
		case ClusterCommandId.GroupMoreActions:
		case ClusterCommandId.ResourceMoreActions:
		case ClusterCommandId.MultipleStorageResourceMoreAction:
		case ClusterCommandId.NodeMoreActions:
		case ClusterCommandId.NodeMultipleMoreActions:
		case ClusterCommandId.ClusterMoreActions:
			return Icons.BlueArrowIndex;
		case ClusterCommandId.ResourceOnline:
		case ClusterCommandId.MultipleResourceOnline:
			return Icons.BringResourceOnlineIndex;
		case ClusterCommandId.ResourceOffline:
		case ClusterCommandId.MultipleResourceOffline:
			return Icons.TakeResourceOfflineIndex;
		case ClusterCommandId.ResourceSimulateFailure:
		case ClusterCommandId.MultipleResourceSimulateFailure:
			return Icons.FailIndex;
		case ClusterCommandId.ResourceShowDependencyReport:
			return Icons.DependencyReportIndex;
		case ClusterCommandId.VirtualMachineGroupLiveMigrate:
		case ClusterCommandId.VirtualMachineGroupQuickMigrate:
			return Icons.LiveMigrationIndex;
		case ClusterCommandId.VirtualMachineGroupCancelLiveMigration:
			return Icons.CancelLiveMigrationIndex;
		case ClusterCommandId.VirtualMachineGroupConnect:
		case ClusterCommandId.VirtualMachineResourceConnect:
		case ClusterCommandId.VirtualMachineMultipleGroupConnect:
			return Icons.VirtualMachineConnectActionIndex;
		case ClusterCommandId.VirtualMachineGroupStart:
		case ClusterCommandId.VirtualMachineResourceStart:
			return Icons.VirtualMachineStartActionIndex;
		case ClusterCommandId.VirtualMachineGroupTurnoff:
		case ClusterCommandId.VirtualMachineResourceTurnoff:
		case ClusterCommandId.VirtualMachineMultipleGroupTurnoff:
			return Icons.VirtualMachineTurnoffActionIndex;
		case ClusterCommandId.VirtualMachineGroupShutdown:
		case ClusterCommandId.VirtualMachineResourceShutdown:
		case ClusterCommandId.VirtualMachineMultipleGroupShutdown:
			return Icons.VirtualMachineShutdownActionIndex;
		case ClusterCommandId.VirtualMachineGroupSave:
		case ClusterCommandId.VirtualMachineResourceSave:
		case ClusterCommandId.VirtualMachineMultipleGroupSave:
			return Icons.VirtualMachineSaveStateActionIndex;
		case ClusterCommandId.VirtualMachineGroupDeleteSavedState:
		case ClusterCommandId.VirtualMachineResourceDeleteSavedState:
			return Icons.VirtualMachineDeleteSavedStateIndex;
		case ClusterCommandId.VirtualMachineGroupSettings:
		case ClusterCommandId.VirtualMachineResourceSettings:
		case ClusterCommandId.VMReplicaBrokerResourceSettings:
			return Icons.VirtualMachineSettingsIndex;
		case ClusterCommandId.VirtualMachineGroupManage:
			return Icons.ManageVirtualMachineIndex;
		case ClusterCommandId.VirtualMachineGroupPause:
		case ClusterCommandId.VirtualMachineResourcePause:
			return Icons.PauseIndex;
		case ClusterCommandId.VirtualMachineGroupResume:
		case ClusterCommandId.VirtualMachineResourceResume:
			return Icons.ResumeIndex;
		case ClusterCommandId.VirtualMachineGroupReset:
		case ClusterCommandId.VirtualMachineResourceReset:
		case ClusterCommandId.VirtualMachineMultipleGroupReset:
			return Icons.VirtualMachineResetIndex;
		case ClusterCommandId.VirtualMachineResourceManage:
			return Icons.ManageVirtualMachineIndex;
		case ClusterCommandId.FileServerGroupAddSharedFolder:
			return Icons.NewShareIndex;
		case ClusterCommandId.NodeStartService:
		case ClusterCommandId.NodeMultipleStartService:
			return Icons.StartIndex;
		case ClusterCommandId.NodeStopService:
		case ClusterCommandId.NodeMultipleStopService:
			return Icons.StopIndex;
		case ClusterCommandId.NodeEvict:
			return Icons.EvictIndex;
		case ClusterCommandId.NodePauseActions:
		case ClusterCommandId.NodeMultiplePauseActions:
			return Icons.PauseNodeIndex;
		case ClusterCommandId.NodeResumeActions:
		case ClusterCommandId.NodeMultipleResumeActions:
			return Icons.ResumeNodeIndex;
		case ClusterCommandId.VirtualMachineMultipleGroupStart:
			return Icons.VirtualMachineStartActionIndex;
		case ClusterCommandId.VirtualMachineMultipleGroupPause:
			return Icons.VirtualMachinePauseActionIndex;
		case ClusterCommandId.VirtualMachineMultipleGroupResume:
			return Icons.VirtualMachineResumeActionIndex;
		case ClusterCommandId.VirtualMachineResourceReplication:
		case ClusterCommandId.VirtualMachineResourceReplicationRelationship:
		case ClusterCommandId.VirtualMachineEnableReplication:
		case ClusterCommandId.VirtualMachineExtendReplication:
			return Icons.CreateReplicationRelationshipIndex;
		case ClusterCommandId.VirtualMachineRemoveReplication:
			return Icons.DeleteReplicationRelationshipIndex;
		case ClusterCommandId.VirtualMachinePauseReplication:
		case ClusterCommandId.VirtualMachinePauseExtendedReplication:
			return Icons.PauseReplicationIndex;
		case ClusterCommandId.VirtualMachineReverseReplication:
			return Icons.ReverseReplicationIndex;
		case ClusterCommandId.VirtualMachineInitializeReplication:
			return Icons.StartInitialReplicationIndex;
		case ClusterCommandId.VirtualMachineResumeReplication:
		case ClusterCommandId.VirtualMachineResumeExtendedReplication:
			return Icons.ResumeReplicationIndex;
		case ClusterCommandId.VirtualMachineCancelFailoverReplication:
		case ClusterCommandId.VirtualMachineCancelInitialReplication:
		case ClusterCommandId.VirtualMachineCancelDiskUpdateReplication:
			return Icons.CancelFailoverIndex;
		case ClusterCommandId.VirtualMachineCancelPrepareFailoverReplication:
			return Icons.CancelFailoverPreparationIndex;
		case ClusterCommandId.VirtualMachineImportReplication:
		case ClusterCommandId.VirtualMachineStartRecovery:
		case ClusterCommandId.VirtualMachinePrepareFailoverReplication:
		case ClusterCommandId.VirtualMachineTestRecovery:
		case ClusterCommandId.VirtualMachineCancelTestFailoverReplication:
		case ClusterCommandId.VirtualMachineViewReplicationHealth:
		case ClusterCommandId.VirtualMachineCommitFailover:
			return Icons.VirtualMachineSettingsIndex;
		case ClusterCommandId.VirtualMachineCancelResynchronize:
			return Icons.VirtualMachineSettingsIndex;
		case ClusterCommandId.DisplayClusterError:
			return Icons.EventErrorIndex;
		case ClusterCommandId.UICommand:
			return Icons.ChooseColumnsIndex;
		case ClusterCommandId.DhcpServerManage:
			return Icons.DhcpServerIndex;
		case ClusterCommandId.DtcManage:
			return Icons.MsdtcIndex;
		case ClusterCommandId.IScsiManage:
			return Icons.IScsiNameServiceIndex;
		case ClusterCommandId.MsmqManage:
			return Icons.MsmqIndex;
		case ClusterCommandId.StandAloneDfsManage:
			return Icons.DfsNamespaceResourceIndex;
		case ClusterCommandId.WinsManage:
			return Icons.WinsServerIndex;
		case ClusterCommandId.NetworkNameRepairActiveDirectoryObject:
			return Icons.NetworkNameRepairActiveDirectoryObjectIndex;
		case ClusterCommandId.NodeRemoteDesktop:
		case ClusterCommandId.NodeMultipleRemoteDesktop:
			return Icons.RemoteDesktopIndex;
		case ClusterCommandId.ConfigureRole:
			return Icons.HARoleIndex;
		case ClusterCommandId.ValidateClusterConfiguration:
			return Icons.ValidateConfigurationIndex;
		case ClusterCommandId.ViewValidationReport:
			return Icons.ViewValidationReportIndex;
		case ClusterCommandId.AddNode:
			return Icons.AddNodeIndex;
		case ClusterCommandId.CloseConnection:
			return Icons.CloseIndex;
		case ClusterCommandId.ConfigureClusterQuorumSettings:
			return Icons.QuorumConfigurationIndex;
		case ClusterCommandId.CopyClusterRoles:
			return Icons.MigrateClusterIndex;
		case ClusterCommandId.ShutdownCluster:
			return Icons.ShutdownClusterIndex;
		case ClusterCommandId.DestroyCluster:
			return Icons.DestroyClusterIndex;
		case ClusterCommandId.ClusterAwareUpdating:
			return Icons.UpdatesIndex;
		case ClusterCommandId.VirtualMachineGroupCheckpoint:
		case ClusterCommandId.VirtualMachineGroupMultipleCheckpoint:
		case ClusterCommandId.VirtualMachineResourceCheckpoint:
			return Icons.VirtualMachineCheckpointIndex;
		case ClusterCommandId.VirtualMachineCheckpointApply:
			return Icons.VirtualMachineCheckpointApplyIndex;
		case ClusterCommandId.VirtualMachineCheckpointDelete:
			return Icons.VirtualMachineCheckpointDeleteIndex;
		case ClusterCommandId.VirtualMachineCheckpointDeleteTree:
			return Icons.VirtualMachineCheckpointDeleteTreeIndex;
		case ClusterCommandId.VirtualMachineCheckpointRename:
			return Icons.VirtualMachineCheckpointRenameIndex;
		case ClusterCommandId.VirtualMachineGroupCheckpointRevert:
		case ClusterCommandId.VirtualMachineResourceCheckpointRevert:
			return Icons.VirtualMachineCheckpointRevertIndex;
		default:
			return Icons.EmptyIconIndex;
		}
	}

	private static int ConvertCommandIdToImageIndex(UICommand command)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected I4, but got Unknown
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected I4, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Invalid comparison between Unknown and I4
		UICommandId uIId = command.UIId;
		switch ((int)uIId)
		{
		default:
			switch (uIId - 9)
			{
			default:
				if ((int)uIId == 16)
				{
					return Icons.TakeResourceOfflineIndex;
				}
				return Icons.FailedIndex;
			case 2:
				return Icons.GenericScriptIndex;
			case 0:
				return Icons.AddPoolActionIndex;
			case 1:
				return Icons.NewPoolActionIndex;
			}
		case 0:
			return Icons.ClusterCriticalEventIndex;
		case 1:
			return Icons.DeleteIndex;
		case 2:
			return Icons.PropertiesIndex;
		}
	}

	protected virtual void OnActionTriggered(object sender, ActionEventArgs e)
	{
		ExecuteCommand();
	}

	private void ConsoleActionTriggered(object sender, ActionEventArgs e)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			OnActionTriggered(sender, e);
		}
		catch (Exception ex)
		{
			ClusterDialogException.ShowTaskDialog(ex, Global.DefaultWindowHandle);
		}
	}

	private void ExecuteCommand()
	{
		ExecuteCommand(null);
	}

	protected void ExecuteCommand(object target)
	{
		if (clusterCommandRef.Target is ClusterCommand clusterCommand)
		{
			clusterCommand.Execute(target ?? clusterCommand.CommandParameter);
			return;
		}
		object target2 = clusterCommandRef.Target;
		UIProxyCommand val = (UIProxyCommand)((target2 is UIProxyCommand) ? target2 : null);
		if (val != null)
		{
			val.Execute(target ?? val.CommandParameter);
		}
		else
		{
			((ICommand)clusterCommandRef.Target).Execute(target);
		}
	}

	private void ClusterCommandCanExecuteChanged(object sender, EventArgs e)
	{
		ICommand command = (ICommand)clusterCommandRef.Target;
		if (command != null && actionsPaneItem is ManagementConsole.Action action)
		{
			try
			{
				action.Enabled = command.CanExecute(null);
			}
			catch (NullReferenceException)
			{
			}
			catch (IndexOutOfRangeException)
			{
			}
		}
	}

	~MmcActionPaneItem()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposing || disposed)
		{
			return;
		}
		ICommand command = (ICommand)clusterCommandRef.Target;
		if (command != null)
		{
			command.CanExecuteChanged -= ClusterCommandCanExecuteChanged;
		}
		if (actionsPaneItem is ManagementConsole.Action action)
		{
			try
			{
				action.Triggered -= ConsoleActionTriggered;
			}
			catch (NullReferenceException)
			{
			}
			catch (IndexOutOfRangeException)
			{
			}
		}
		actionsPaneItem = null;
		disposed = true;
	}
}

