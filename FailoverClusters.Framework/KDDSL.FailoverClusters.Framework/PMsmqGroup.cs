using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PMsmqGroup : PGroup
{
	public PMsmqGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.Msmq)
	{
	}
}

