using System.Net;

namespace Microsoft.FailoverClusters.Framework;

public class IPv6TunnelAddressResource : CommonIPAddressResource
{
	internal IPv6TunnelAddressResource(Cluster cluster)
		: base(cluster, IPAddress.IPv6None)
	{
	}
}
