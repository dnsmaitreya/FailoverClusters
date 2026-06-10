using System;

namespace FailoverClusters.Framework;

public class ClusterNetworkInterfaceStateEventArgs : ClusterEventArgs
{
	public NetworkInterfaceState? State { get; internal set; }

	public ClusterNetworkInterfaceStateEventArgs(Guid id, NetworkInterfaceState? newState, ClusterException exception)
		: base(id, exception)
	{
		State = newState;
	}
}

