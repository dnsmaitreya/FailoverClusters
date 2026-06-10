using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PWinsGroup : PGroup
{
	public PWinsGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.Wins)
	{
	}
}

