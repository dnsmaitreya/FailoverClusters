using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PTaskSchedulerResource : PResource
{
	public PTaskSchedulerResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.TaskScheduler))
	{
	}
}

