using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PHyperVNetworkVirtualizationPAResource : PResource
{
	public PHyperVNetworkVirtualizationPAResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.HyperVNetworkVirtualizationProviderAddress))
	{
	}
}

