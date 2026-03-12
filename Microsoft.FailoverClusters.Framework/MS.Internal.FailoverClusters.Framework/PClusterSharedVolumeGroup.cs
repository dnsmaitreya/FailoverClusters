using System;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PClusterSharedVolumeGroup : PGroup
{
	public PClusterSharedVolumeGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.ClusterSharedVolume)
	{
	}
}
