using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class VirtualMachineMoveStorageCommand : GroupCommandBase
{
	public VirtualMachineMoveStorageCommand(VirtualMachineGroup group)
		: base(group)
	{
	}

	protected override ClusterCommand GenerateInstance()
	{
		ClusterCommand clusterCommand = new ClusterCommand(base.ClusterGroup, "VirtualMachineStorageMove", ClusterCommandId.VirtualMachineGroupStorageMove, ClusterCommandCollectionId.GroupStorageOwnership);
		clusterCommand.Text = CommandResources.VirtualMachineGroup_MoveStorageCommand_Text;
		clusterCommand.Description = string.Empty;
		clusterCommand.InputParameters = new InputParameterList<VirtualMachineGroup>(new VirtualMachineGroup[1] { (VirtualMachineGroup)base.ClusterGroup });
		clusterCommand.ExecuteDelegate = (base.ClusterGroup as VirtualMachineGroup).MoveStorage;
		clusterCommand.CanExecuteDelegate = (object o) => true;
		clusterCommand.CommandParameter = new MoveVirtualMachineStorageMapping(base.ClusterGroup.Cluster);
		return clusterCommand;
	}
}

