using System;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PClusterAwareUpdatingGroup : PGroup
{
	public PClusterAwareUpdatingGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.ClusterAwareUpdating)
	{
	}
}
