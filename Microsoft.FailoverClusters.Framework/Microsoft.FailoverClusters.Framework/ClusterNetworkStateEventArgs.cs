using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterNetworkStateEventArgs : ClusterEventArgs
{
	public NetworkState? State { get; internal set; }

	public ClusterNetworkStateEventArgs(Guid id, NetworkState? newState, ClusterException exception)
		: base(id, exception)
	{
		State = newState;
	}
}
