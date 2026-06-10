using System;
using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PGenericServiceGroup : PGroup
{
	public PGenericServiceGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.GenericService)
	{
	}
}

