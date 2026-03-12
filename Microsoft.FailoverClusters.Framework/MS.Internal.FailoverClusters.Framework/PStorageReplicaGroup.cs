using System;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PStorageReplicaGroup : PGroup
{
	public PStorageReplicaGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.StorageReplica)
	{
	}
}
