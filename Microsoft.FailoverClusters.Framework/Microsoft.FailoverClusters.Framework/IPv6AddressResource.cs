using System.Net;

namespace Microsoft.FailoverClusters.Framework;

public class IPv6AddressResource : CommonIPAddressResource
{
	internal IPv6AddressResource(Cluster cluster)
		: base(cluster, IPAddress.IPv6None)
	{
	}
}
