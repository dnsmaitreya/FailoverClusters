using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PVirtualMachineReplicationCoordinatorGroup : PGroup
{
	public PVirtualMachineReplicationCoordinatorGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.VirtualMachineReplicationCoordinator)
	{
	}
}

