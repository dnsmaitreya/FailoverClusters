using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterResourceSharedVolumeStateEventArgs : ClusterEventArgs
{
	public ClusterSharedVolumeStateInfo State { get; internal set; }

	public ClusterResourceSharedVolumeStateEventArgs(Guid id, ClusterSharedVolumeStateInfo newState, ClusterException exception)
		: base(id, exception)
	{
		State = newState;
	}
}
