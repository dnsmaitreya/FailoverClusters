using System;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PClusterAwareUpdatingResource : PResource
{
	public PClusterAwareUpdatingResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.ClusterAwareUpdating))
	{
	}
}
