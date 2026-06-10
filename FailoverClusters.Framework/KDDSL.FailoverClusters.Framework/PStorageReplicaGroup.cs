using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PStorageReplicaGroup : PGroup
{
	public PStorageReplicaGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.StorageReplica)
	{
	}
}

