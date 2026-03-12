using System;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PIPv6TunnelAddressResource : PCommonIPAddressResource
{
	public PIPv6TunnelAddressResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.IPv6TunnelAddress))
	{
	}
}
