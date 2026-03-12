using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public abstract class GroupMoveCommandBase : GroupCommandBase
{
	private readonly bool setInputParametersMoveTargetsOnOwnerChanged;

	protected ClusterObject ClusterObjectIssuingMoveRequest { get; private set; }

	protected GroupMoveCommandBase(Group group, ClusterObject clusterObjectIssuingMoveRequest, bool setInputParametersMoveTargetsOnOwnerChanged)
		: base(group)
	{
		this.setInputParametersMoveTargetsOnOwnerChanged = setInputParametersMoveTargetsOnOwnerChanged;
		base.UpdateCanExecuteOnApplicationStatusChange = true;
		base.UpdateCanExecuteOnStateChange = true;
		ClusterObjectIssuingMoveRequest = clusterObjectIssuingMoveRequest;
	}

	internal static void DefaultExecuteMove(object node, IEnumerable<GroupMoveDescriptor> groupMoveDescriptors)
	{
		if (node is Group)
		{
			return;
		}
		Node nodeParam = node as Node;
		if (nodeParam == null && node != null)
		{
			throw new InvalidOperationException(ExceptionResources.InvalidOperation_IsNotNodeMoveCommand);
		}
		List<GroupMoveDescriptor> descriptorList = groupMoveDescriptors.ToList();
		descriptorList.ForEach(delegate(GroupMoveDescriptor groupMoveDescriptor)
		{
			Group ownerGroup = groupMoveDescriptor.OwnerGroup;
			if (ownerGroup != null)
			{
				Node ownerNode = ownerGroup.OwnerNode;
				if (ownerNode != null && (nodeParam == null || ownerNode.NodeId != nodeParam.NodeId))
				{
					ownerGroup.Move(nodeParam, delegate(OperationResult r)
					{
						if (groupMoveDescriptor.ClusterObjectIssuingMoveRequest != null)
						{
							groupMoveDescriptor.ClusterObjectIssuingMoveRequest.Error = r.Error;
						}
						else
						{
							ownerGroup.Error = r.Error;
						}
					}, descriptorList.Count == 1);
				}
			}
		});
	}

	protected override void DoPostGenerateInstance(ClusterCommand newValue)
	{
		base.ClusterGroup.OwnerNodeChanged += UpdateMoveTargets;
		newValue.Finalizing += delegate
		{
			base.ClusterGroup.OwnerNodeChanged -= UpdateMoveTargets;
		};
		base.DoPostGenerateInstance(newValue);
	}

	protected virtual bool DefaultCanExecuteMove(object nodeOrGroup, ClusterCommand clusterCommand)
	{
		return true;
	}

	protected void DefaultExecuteMove(object node)
	{
		DefaultExecuteMove(node, new List<GroupMoveDescriptor>
		{
			new GroupMoveDescriptor
			{
				ClusterObjectIssuingMoveRequest = ClusterObjectIssuingMoveRequest,
				OwnerGroup = base.ClusterGroup
			}
		});
	}

	protected void UpdateMoveTargets(object sender, ClusterGroupOwnerNodeEventArgs e)
	{
		if (setInputParametersMoveTargetsOnOwnerChanged)
		{
			ClusterCommand clusterCommand = TryGetInstance();
			if (clusterCommand != null)
			{
				clusterCommand.InputParameters = base.ClusterGroup.Cluster.AllUpNodes;
			}
		}
		TryUpdateCanExecute();
	}

	protected bool? ClusterSharedVolumeCanExecuteMove(object nodeOrGroup, ClusterCommand clusterCommand)
	{
		CsvVolumeResource csvVolumeResource = ClusterObjectIssuingMoveRequest as CsvVolumeResource;
		if (csvVolumeResource != null)
		{
			return !csvVolumeResource.IsProcessing && csvVolumeResource.OwnerGroup != null && !csvVolumeResource.OwnerGroup.IsProcessing;
		}
		return null;
	}

	protected bool? VirtualMachineGroupCanExecuteMove(ClusterCommand clusterCommand, bool isLiveMigration)
	{
		if (isLiveMigration && base.ClusterGroup.ApplicationStatus != 0 && base.ClusterGroup.ApplicationStatus != ApplicationStatus.PartiallyRunning && base.ClusterGroup.ApplicationStatus != ApplicationStatus.Paused)
		{
			clusterCommand.CannotExecuteReason = CommandResources.Group_Move_Cannot_VM_Not_Online;
			return false;
		}
		return null;
	}

	protected void ClusterSharedVolumeRegisterCallbacks(ClusterCommand newValue)
	{
		((CsvVolumeResource)ClusterObjectIssuingMoveRequest).MaintenanceModeChanged += TryUpdateCanExecuteHandler;
		newValue.Finalizing += delegate
		{
			((CsvVolumeResource)ClusterObjectIssuingMoveRequest).MaintenanceModeChanged -= TryUpdateCanExecuteHandler;
		};
	}

	protected void TryUpdateCanExecuteHandler(object sender, ClusterMaintenanceModeEventArgs args)
	{
		TryUpdateCanExecute();
	}
}
