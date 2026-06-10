using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PScaleOutFileServerGroup : PFileServerGroup
{
	public PScaleOutFileServerGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.ScaleOutFileServer)
	{
	}
}

