using System.Windows.Input;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class VirtualMachineMoveCommand : GroupCommandBase
{
	private readonly VirtualMachineLiveMigrateCommand virtualMachineLiveMigrateCommand;

	private readonly VirtualMachineQuickMigrateCommand virtualMachineQuickMigrateCommand;

	private readonly VirtualMachineMoveStorageCommand virtualMachineMoveStorageCommand;

	public VirtualMachineMoveCommand(VirtualMachineGroup group)
		: base(group)
	{
		virtualMachineLiveMigrateCommand = new VirtualMachineLiveMigrateCommand(group);
		virtualMachineQuickMigrateCommand = new VirtualMachineQuickMigrateCommand(group);
		virtualMachineMoveStorageCommand = new VirtualMachineMoveStorageCommand(group);
	}

	protected override ClusterCommand GenerateInstance()
	{
		return new ClusterCommandContainer(base.ClusterGroup, "MoveTo", ClusterCommandId.GroupMoveTo, ClusterCommandCollectionId.GroupOwnership)
		{
			Text = CommandResources.Group_Move,
			ChildrenInternal = 
			{
				(ICommand)virtualMachineLiveMigrateCommand.GetInstance(),
				(ICommand)virtualMachineQuickMigrateCommand.GetInstance(),
				(ICommand)virtualMachineMoveStorageCommand.GetInstance()
			}
		};
	}
}
