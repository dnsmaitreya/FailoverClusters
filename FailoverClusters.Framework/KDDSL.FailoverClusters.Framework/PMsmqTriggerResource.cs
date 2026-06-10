using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PMsmqTriggerResource : PResource
{
	public PMsmqTriggerResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.MsmsqTrigger))
	{
	}
}

