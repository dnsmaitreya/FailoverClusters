using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class VirtualMachineCancelLiveMigrationCommand : GroupCommandBase
{
	public VirtualMachineCancelLiveMigrationCommand(VirtualMachineGroup group)
		: base(group)
	{
		base.UpdateCanExecuteOnApplicationStatusChange = true;
	}

	public static bool CanExecute(Group group)
	{
		if (group.ApplicationStatus != ApplicationStatus.LiveMigrating)
		{
			return group.ApplicationStatus == ApplicationStatus.LiveMigrationQueued;
		}
		return true;
	}

	protected override ClusterCommand GenerateInstance()
	{
		return new ClusterCommand(base.ClusterGroup, "CancelLiveMigration", ClusterCommandId.VirtualMachineGroupCancelLiveMigration, ClusterCommandCollectionId.GroupOwnership)
		{
			Text = CommandResources.VirtualMachineGroup_CancelLiveMigration,
			CanExecuteDelegate = (object x) => CanExecute(base.ClusterGroup),
			ExecuteDelegate = delegate
			{
				base.ClusterGroup.CancelLiveMigration();
			}
		};
	}
}

