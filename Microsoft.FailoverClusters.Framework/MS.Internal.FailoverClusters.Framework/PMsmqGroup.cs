using System;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PMsmqGroup : PGroup
{
	public PMsmqGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.Msmq)
	{
	}
}
