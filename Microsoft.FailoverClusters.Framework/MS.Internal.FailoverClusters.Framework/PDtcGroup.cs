using System;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PDtcGroup : PGroup
{
	public PDtcGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.Dtc)
	{
	}
}
