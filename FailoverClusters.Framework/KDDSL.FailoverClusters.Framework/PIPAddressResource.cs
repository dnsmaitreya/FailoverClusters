using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PIPAddressResource : PCommonIPAddressResource
{
	public PIPAddressResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.IPAddress))
	{
	}
}

