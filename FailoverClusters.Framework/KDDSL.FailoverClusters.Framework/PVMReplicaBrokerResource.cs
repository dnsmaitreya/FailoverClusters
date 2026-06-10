using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PVMReplicaBrokerResource : PResource
{
	public PVMReplicaBrokerResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.VirtualMachineReplicationBroker))
	{
	}
}

