using System;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public abstract class VirtualMachineMoveCommandBase : GroupMoveCommandBase
{
	private readonly VirtualMachineMoveToSelectedCommand virtualMachineLiveMigrateToSelectedCommand;

	private bool isLiveMigrationCommand;

	protected VirtualMachineMoveCommandBase(VirtualMachineGroup group)
		: base(group, null, setInputParametersMoveTargetsOnOwnerChanged: false)
	{
		virtualMachineLiveMigrateToSelectedCommand = new VirtualMachineMoveToSelectedCommand(this);
	}

	protected override void DoPostGenerateInstance(ClusterCommand newValue)
	{
		if (!(newValue is ClusterCommandContainer))
		{
			throw new InvalidOperationException("Consumers of VirtualMachineMoveCommandBase must produce ClusterCommandContainers");
		}
		ClusterCommandContainer clusterCommandContainer = (ClusterCommandContainer)newValue;
		clusterCommandContainer.ChildrenInternal.Add(CreateMoveToBestNodeCommand(clusterCommandContainer));
		clusterCommandContainer.ChildrenInternal.Add(virtualMachineLiveMigrateToSelectedCommand.GetInstance());
		base.DoPostGenerateInstance(newValue);
	}

	protected ClusterCommand CreateMoveToBestNodeCommand(ClusterCommand parent)
	{
		ClusterCommand moveToNodeCommand = new ClusterCommand(base.ClusterGroup, "MoveTo", ClusterCommandId.GroupMoveToBest, ClusterCommandCollectionId.GroupOwnership)
		{
			Text = CommandResources.Group_MoveToBestPossible,
			CommandParameter = null,
			ExecuteDelegate = parent.ExecuteDelegate
		};
		isLiveMigrationCommand = parent.Id == ClusterCommandId.VirtualMachineGroupLiveMigrate;
		moveToNodeCommand.CanExecuteDelegate = (object o) => DefaultCanExecuteMove(o, moveToNodeCommand);
		return moveToNodeCommand;
	}

	protected override bool DefaultCanExecuteMove(object nodeOrGroup, ClusterCommand clusterCommand)
	{
		if (VirtualMachineGroupCanExecuteMove(clusterCommand, isLiveMigrationCommand) != false)
		{
			return base.DefaultCanExecuteMove(nodeOrGroup, clusterCommand);
		}
		return false;
	}
}
