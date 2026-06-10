using System;
using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class POtherGroup : PGroup
{
	public POtherGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.Unknown)
	{
	}
}

