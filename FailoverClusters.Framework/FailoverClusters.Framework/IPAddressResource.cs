using System.Net;

namespace FailoverClusters.Framework;

public class IPAddressResource : CommonIPAddressResource
{
	internal IPAddressResource(Cluster cluster)
		: base(cluster, IPAddress.IPv6None)
	{
	}
}

