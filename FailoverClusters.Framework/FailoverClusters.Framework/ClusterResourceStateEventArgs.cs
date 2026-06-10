using System;

namespace FailoverClusters.Framework;

public class ClusterResourceStateEventArgs : ClusterEventArgs
{
	public ResourceState? State { get; internal set; }

	public ClusterResourceStateEventArgs(Guid id, ResourceState? newState, ClusterException exception)
		: base(id, exception)
	{
		State = newState;
	}
}

