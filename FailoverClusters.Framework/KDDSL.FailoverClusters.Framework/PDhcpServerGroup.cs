using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PDhcpServerGroup : PGroup
{
	public PDhcpServerGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.DhcpServer)
	{
	}
}

