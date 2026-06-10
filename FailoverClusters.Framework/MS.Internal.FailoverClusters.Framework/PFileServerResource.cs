using System;
using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PFileServerResource : PResource
{
	protected PFileServerResource(PCluster cluster, Guid id, string name, PResourceType resourceType)
		: base(cluster, id, name, resourceType)
	{
	}

	public PFileServerResource(PCluster cluster, Guid id, string name)
		: this(cluster, id, name, new PResourceType(cluster, ResourceKind.FileServer))
	{
	}
}

