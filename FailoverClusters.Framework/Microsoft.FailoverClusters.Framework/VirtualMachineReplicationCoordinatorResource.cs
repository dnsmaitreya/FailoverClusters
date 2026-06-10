using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class VirtualMachineReplicationCoordinatorResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.VirtualMachineReplicationCoordinator));

	internal VirtualMachineReplicationCoordinatorResource(Cluster cluster)
		: base(cluster)
	{
	}
}

