using System;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PVirtualMachineReplicationCoordinatorGroup : PGroup
{
	public PVirtualMachineReplicationCoordinatorGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.VirtualMachineReplicationCoordinator)
	{
	}
}
