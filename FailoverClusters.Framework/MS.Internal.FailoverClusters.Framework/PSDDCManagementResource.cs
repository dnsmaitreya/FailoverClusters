using System;
using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PSDDCManagementResource : PResource
{
	public PSDDCManagementResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.SDDCManagement))
	{
	}
}

