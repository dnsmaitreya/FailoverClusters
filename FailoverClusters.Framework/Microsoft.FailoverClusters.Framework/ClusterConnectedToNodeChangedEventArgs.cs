using System;

namespace FailoverClusters.Framework;

public class ClusterConnectedToNodeChangedEventArgs : ClusterEventArgs
{
	public string ConnectedTo { get; internal set; }

	public ClusterConnectedToNodeChangedEventArgs(Guid id, string connectedToNode)
		: base(id, null)
	{
		ConnectedTo = connectedToNode;
	}
}

