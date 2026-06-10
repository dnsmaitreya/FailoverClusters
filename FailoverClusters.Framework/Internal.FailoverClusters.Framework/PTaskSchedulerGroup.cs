using System;
using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PTaskSchedulerGroup : PGroup
{
	public PTaskSchedulerGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.TaskScheduler)
	{
	}
}

