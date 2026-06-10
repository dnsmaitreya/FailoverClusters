using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PClusterStoragePoolGroup : PGroup
{
	public PClusterStoragePoolGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.ClusterStoragePool)
	{
	}
}

