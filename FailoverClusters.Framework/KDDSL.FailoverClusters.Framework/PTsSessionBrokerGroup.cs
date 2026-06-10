using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PTsSessionBrokerGroup : PGroup
{
	public PTsSessionBrokerGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.TsSessionBroker)
	{
	}
}

