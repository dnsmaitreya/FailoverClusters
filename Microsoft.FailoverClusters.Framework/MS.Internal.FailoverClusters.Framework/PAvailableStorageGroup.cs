using System;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PAvailableStorageGroup : PGroup
{
	public PAvailableStorageGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.AvailableStorage)
	{
	}
}
