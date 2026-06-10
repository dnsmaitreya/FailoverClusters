using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PVirtualIPv4AddressResource : PResource
{
	public PVirtualIPv4AddressResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.DisjointIPv4Address))
	{
	}
}

