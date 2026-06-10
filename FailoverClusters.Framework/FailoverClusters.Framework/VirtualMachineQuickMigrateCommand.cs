using System;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class VirtualMachineQuickMigrateCommand : VirtualMachineMoveCommandBase
{
	public VirtualMachineQuickMigrateCommand(VirtualMachineGroup group)
		: base(group)
	{
	}

	protected override ClusterCommand GenerateInstance()
	{
		ClusterCommandContainer quickMigrateCommand = new ClusterCommandContainer(base.ClusterGroup, "QuickMigrate", ClusterCommandId.VirtualMachineGroupQuickMigrate, ClusterCommandCollectionId.GroupOwnership)
		{
			Text = CommandResources.Group_VM_Quick_Migrate,
			Description = string.Empty,
			CommandParameter = base.ClusterGroup,
			ExecuteDelegate = delegate(object node)
			{
				if (node != null && !(node is Node))
				{
					throw new InvalidOperationException(ExceptionResources.InvalidOperation_IsNotNodeMoveCommand);
				}
				Node node2 = node as Node;
				((VirtualMachineGroup)base.ClusterGroup).Migrate(node2, VirtualMachineMigrationType.Quick, enableOverride: true);
			}
		};
		quickMigrateCommand.CanExecuteDelegate = (object o) => DefaultCanExecuteMove(o, quickMigrateCommand);
		return quickMigrateCommand;
	}
}

