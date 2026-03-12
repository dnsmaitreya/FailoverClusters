using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterGroupPriorityEventArgs : ClusterEventArgs
{
	public Priority? Priority { get; internal set; }

	public ClusterGroupPriorityEventArgs(Guid id, Priority? priority, ClusterException exception)
		: base(id, exception)
	{
		Priority = priority;
	}
}
