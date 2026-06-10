using System;
using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PStandAloneDfsGroup : PGroup
{
	public PStandAloneDfsGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.StandAloneDfs)
	{
	}
}

