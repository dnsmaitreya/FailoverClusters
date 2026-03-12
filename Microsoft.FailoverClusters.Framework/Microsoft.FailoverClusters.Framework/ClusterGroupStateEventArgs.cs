using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterGroupStateEventArgs : ClusterEventArgs
{
	public GroupState? State { get; internal set; }

	public ClusterGroupStateEventArgs(Guid id, GroupState? newState, ClusterException exception)
		: base(id, exception)
	{
		State = newState;
	}
}
