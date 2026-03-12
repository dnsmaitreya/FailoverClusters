using System;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PDhcpServerGroup : PGroup
{
	public PDhcpServerGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.DhcpServer)
	{
	}
}
