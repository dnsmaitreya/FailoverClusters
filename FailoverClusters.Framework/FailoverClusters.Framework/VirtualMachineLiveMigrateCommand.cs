using System;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class VirtualMachineLiveMigrateCommand : VirtualMachineMoveCommandBase
{
	public VirtualMachineLiveMigrateCommand(VirtualMachineGroup group)
		: base(group)
	{
	}

	protected override ClusterCommand GenerateInstance()
	{
		return new ClusterCommandContainer(base.ClusterGroup, "LiveMigrate", ClusterCommandId.VirtualMachineGroupLiveMigrate, ClusterCommandCollectionId.GroupOwnership)
		{
			Text = CommandResources.Group_VM_Live_Migration,
			CommandParameter = base.ClusterGroup,
			Description = string.Empty,
			ExecuteDelegate = delegate(object node)
			{
				if (node != null && !(node is Node))
				{
					throw new InvalidOperationException(ExceptionResources.InvalidOperation_IsNotNodeMoveCommand);
				}
				Node node2 = node as Node;
				((VirtualMachineGroup)base.ClusterGroup).Migrate(node2, VirtualMachineMigrationType.Live, enableOverride: true);
			}
		};
	}
}

