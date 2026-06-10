using System;
using System.Collections.Generic;

namespace FailoverClusters.Framework;

public class ClusterNetworkChangedEventArgs : ClusterEventArgs
{
	public List<NetworkInfo> NetworksInfo { get; internal set; }

	public ClusterNetworkChangedEventArgs(Guid id, List<NetworkInfo> newNetworksInfo, ClusterException exception)
		: base(id, exception)
	{
		NetworksInfo = newNetworksInfo;
	}
}

