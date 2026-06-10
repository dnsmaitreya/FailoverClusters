using System;
using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PVMReplicaBrokerGroup : PGroup
{
	public PVMReplicaBrokerGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.VMReplicaBroker)
	{
	}
}

