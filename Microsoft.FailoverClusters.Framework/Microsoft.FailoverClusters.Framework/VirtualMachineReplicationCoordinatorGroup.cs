using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class VirtualMachineReplicationCoordinatorGroup : AverageGroup
{
	public override GroupType GroupType => GroupType.VirtualMachineReplicationCoordinator;

	protected override Icon2 GenerateIcon()
	{
		return new Icon2(InvariantResources.Group);
	}

	internal VirtualMachineReplicationCoordinatorGroup(Cluster cluster)
		: base(cluster)
	{
	}
}
