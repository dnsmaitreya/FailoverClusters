using System;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PInfrastructureFileServerGroup : PFileServerGroup
{
	public PInfrastructureFileServerGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.InfrastructureFileServer)
	{
	}
}
