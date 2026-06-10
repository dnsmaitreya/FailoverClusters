using System;
using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PStorageReplicaResource : PResource
{
	public PStorageReplicaResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.StorageReplica))
	{
	}
}

