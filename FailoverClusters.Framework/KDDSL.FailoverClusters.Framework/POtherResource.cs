using System;

namespace KDDSL.FailoverClusters.Framework;

internal class POtherResource : PResource
{
	public POtherResource(PCluster cluster, Guid id, string name, string resourceTypeName)
		: base(cluster, id, name, new PResourceType(cluster, resourceTypeName))
	{
	}
}
