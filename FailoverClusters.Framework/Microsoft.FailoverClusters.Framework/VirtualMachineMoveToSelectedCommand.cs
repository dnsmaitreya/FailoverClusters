using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class VirtualMachineMoveToSelectedCommand : GroupMoveCommandBase
{
	private readonly GroupMoveCommandBase parentWrapper;

	private bool isLiveMigrationCommand;

	public VirtualMachineMoveToSelectedCommand(GroupMoveCommandBase parentWrapper)
		: base(parentWrapper?.ClusterGroup, null, setInputParametersMoveTargetsOnOwnerChanged: true)
	{
		this.parentWrapper = parentWrapper;
	}

	protected override ClusterCommand GenerateInstance()
	{
		ClusterCommand instance = parentWrapper.GetInstance();
		ClusterCommand openMoveToSelectedNodeDialogCommand = new ClusterCommand(base.ClusterGroup, "MoveTo", ClusterCommandId.GroupMoveVMToSelectedNode, ClusterCommandCollectionId.GroupOwnership)
		{
			Text = CommandResources.Group_MoveToSelected,
			Description = instance.Description,
			InputParameters = base.ClusterGroup.Cluster.AllUpNodes,
			CommandParameter = base.ClusterGroup,
			ExecuteDelegate = instance.ExecuteDelegate
		};
		isLiveMigrationCommand = instance.Id == ClusterCommandId.VirtualMachineGroupLiveMigrate;
		openMoveToSelectedNodeDialogCommand.CanExecuteDelegate = (object o) => DefaultCanExecuteMove(o, openMoveToSelectedNodeDialogCommand);
		return openMoveToSelectedNodeDialogCommand;
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

