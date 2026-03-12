using System;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PWinsGroup : PGroup
{
	public PWinsGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.Wins)
	{
	}
}
