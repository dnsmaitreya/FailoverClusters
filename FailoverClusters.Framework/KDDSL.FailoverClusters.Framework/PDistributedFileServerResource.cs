using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PDistributedFileServerResource : PFileServerResource
{
	public PDistributedFileServerResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.ScaleOutFileServer))
	{
	}
}

