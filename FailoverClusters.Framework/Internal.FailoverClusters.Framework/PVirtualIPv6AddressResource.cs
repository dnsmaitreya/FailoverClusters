using System;
using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PVirtualIPv6AddressResource : PResource
{
	public PVirtualIPv6AddressResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.DisjointIPv6Address))
	{
	}
}

