using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PIPv6AddressResource : PCommonIPAddressResource
{
	public PIPv6AddressResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.IPv6Address))
	{
	}
}

