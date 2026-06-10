using System;

namespace KDDSL.ServerClusters;

public class ClusterQuorumEventArgs : EventArgs
{
	private string data;

	public string Data => data;

	internal ClusterQuorumEventArgs(string data)
	{
		this.data = data;
	}
}
