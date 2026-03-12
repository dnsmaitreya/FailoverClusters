using System;
using System.Collections.Generic;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterNetworkInterfaceChangedEventArgs : ClusterEventArgs
{
	public List<NetworkInfo> NetworksInfo { get; internal set; }

	public ClusterNetworkInterfaceChangedEventArgs(Guid id, List<NetworkInfo> newNetworksInfo, ClusterException exception)
		: base(id, exception)
	{
		NetworksInfo = newNetworksInfo;
	}
}
