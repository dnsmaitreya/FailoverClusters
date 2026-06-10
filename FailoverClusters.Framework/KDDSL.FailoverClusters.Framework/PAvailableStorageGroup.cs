using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PAvailableStorageGroup : PGroup
{
	public PAvailableStorageGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.AvailableStorage)
	{
	}
}

