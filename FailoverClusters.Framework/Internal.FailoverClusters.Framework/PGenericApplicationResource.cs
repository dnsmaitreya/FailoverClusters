using System;
using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PGenericApplicationResource : PResource
{
	public PGenericApplicationResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.GenericApplication))
	{
	}
}

