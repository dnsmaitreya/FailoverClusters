using System;
using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PGenericApplicationGroup : PGroup
{
	public PGenericApplicationGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.GenericApplication)
	{
	}
}

