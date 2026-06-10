using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PClusterAwareUpdatingResource : PResource
{
	public PClusterAwareUpdatingResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.ClusterAwareUpdating))
	{
	}
}

