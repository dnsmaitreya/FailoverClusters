using System.Net;

namespace Microsoft.FailoverClusters.Framework;

public class IPAddressResource : CommonIPAddressResource
{
	internal IPAddressResource(Cluster cluster)
		: base(cluster, IPAddress.IPv6None)
	{
	}
}
