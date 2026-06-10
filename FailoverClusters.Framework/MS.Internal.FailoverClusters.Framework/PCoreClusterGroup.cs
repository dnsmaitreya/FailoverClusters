using System;
using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PCoreClusterGroup : PGroup
{
	public PCoreClusterGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.CoreCluster)
	{
	}
}

