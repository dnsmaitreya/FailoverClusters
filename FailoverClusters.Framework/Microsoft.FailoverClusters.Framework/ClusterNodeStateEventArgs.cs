using System;

namespace FailoverClusters.Framework;

public class ClusterNodeStateEventArgs : ClusterEventArgs
{
	public NodeState? State { get; internal set; }

	public ClusterNodeStateEventArgs(Guid id, NodeState? newState, ClusterException exception)
		: base(id, exception)
	{
		State = newState;
	}
}

