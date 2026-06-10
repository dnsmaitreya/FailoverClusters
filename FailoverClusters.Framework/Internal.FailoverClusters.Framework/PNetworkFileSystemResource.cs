using System;
using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PNetworkFileSystemResource : PResource
{
	public PNetworkFileSystemResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.NetworkFileSystem))
	{
	}
}

