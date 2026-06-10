using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PClusterSharedVolumeGroup : PGroup
{
	public PClusterSharedVolumeGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.ClusterSharedVolume)
	{
	}
}

