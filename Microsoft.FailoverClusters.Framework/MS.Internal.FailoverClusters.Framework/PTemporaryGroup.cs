using System;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PTemporaryGroup : PGroup
{
	public PTemporaryGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.Temporary)
	{
	}
}
