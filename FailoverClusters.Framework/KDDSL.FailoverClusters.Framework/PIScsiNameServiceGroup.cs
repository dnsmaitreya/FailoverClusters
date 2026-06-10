using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PIScsiNameServiceGroup : PGroup
{
	public PIScsiNameServiceGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.IScsiNameService)
	{
	}
}

