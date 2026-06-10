using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PDhcpServiceResource : PResource
{
	public PDhcpServiceResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.DhcpService))
	{
	}
}

