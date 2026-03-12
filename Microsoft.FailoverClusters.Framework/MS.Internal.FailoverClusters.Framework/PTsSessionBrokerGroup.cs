using System;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PTsSessionBrokerGroup : PGroup
{
	public PTsSessionBrokerGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.TsSessionBroker)
	{
	}
}
