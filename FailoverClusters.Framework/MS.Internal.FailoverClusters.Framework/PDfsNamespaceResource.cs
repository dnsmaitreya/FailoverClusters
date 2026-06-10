using System;
using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PDfsNamespaceResource : PResource
{
	public PDfsNamespaceResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.DistributedFileSystem))
	{
	}
}

