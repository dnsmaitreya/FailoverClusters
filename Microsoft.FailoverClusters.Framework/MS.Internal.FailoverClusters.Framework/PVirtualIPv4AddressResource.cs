using System;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PVirtualIPv4AddressResource : PResource
{
	public PVirtualIPv4AddressResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.DisjointIPv4Address))
	{
	}
}
