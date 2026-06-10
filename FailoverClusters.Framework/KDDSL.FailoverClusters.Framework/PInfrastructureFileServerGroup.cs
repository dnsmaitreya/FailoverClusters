using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PInfrastructureFileServerGroup : PFileServerGroup
{
	public PInfrastructureFileServerGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.InfrastructureFileServer)
	{
	}
}

