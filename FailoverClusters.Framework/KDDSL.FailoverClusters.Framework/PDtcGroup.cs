using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PDtcGroup : PGroup
{
	public PDtcGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.Dtc)
	{
	}
}

