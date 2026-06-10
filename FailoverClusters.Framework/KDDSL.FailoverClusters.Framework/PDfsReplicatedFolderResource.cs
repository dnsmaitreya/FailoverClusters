using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PDfsReplicatedFolderResource : PResource
{
	public PDfsReplicatedFolderResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.DfsReplicatedFolder))
	{
	}
}

