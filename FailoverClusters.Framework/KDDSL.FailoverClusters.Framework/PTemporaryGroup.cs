using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PTemporaryGroup : PGroup
{
	public PTemporaryGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.Temporary)
	{
	}
}

