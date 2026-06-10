using System;
using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PFileServerGroup : PGroup
{
	protected PFileServerGroup(PCluster cluster, Guid id, string name, GroupType groupType)
		: base(cluster, id, name, groupType)
	{
	}

	public PFileServerGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.FileServer)
	{
	}
}

