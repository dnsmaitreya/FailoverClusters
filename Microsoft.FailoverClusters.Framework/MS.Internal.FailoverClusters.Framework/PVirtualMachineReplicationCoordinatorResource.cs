using System;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PVirtualMachineReplicationCoordinatorResource : PResource
{
	public PVirtualMachineReplicationCoordinatorResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.VirtualMachineReplicationCoordinator))
	{
	}
}
