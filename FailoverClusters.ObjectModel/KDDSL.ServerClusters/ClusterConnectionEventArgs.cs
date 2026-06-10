using System;

namespace KDDSL.ServerClusters;

public class ClusterConnectionEventArgs : EventArgs
{
	private ClusterConnectionState connectionState;

	public ClusterConnectionState ConnectionState => connectionState;

	internal ClusterConnectionEventArgs(ClusterConnectionState connectionState)
	{
		this.connectionState = connectionState;
	}
}
