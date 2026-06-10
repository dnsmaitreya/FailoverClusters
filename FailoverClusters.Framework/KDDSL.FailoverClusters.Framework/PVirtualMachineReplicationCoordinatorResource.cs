using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PVirtualMachineReplicationCoordinatorResource : PResource
{
	public PVirtualMachineReplicationCoordinatorResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.VirtualMachineReplicationCoordinator))
	{
	}
}

