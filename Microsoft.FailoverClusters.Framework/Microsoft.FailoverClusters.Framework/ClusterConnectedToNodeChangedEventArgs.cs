using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterConnectedToNodeChangedEventArgs : ClusterEventArgs
{
	public string ConnectedTo { get; internal set; }

	public ClusterConnectedToNodeChangedEventArgs(Guid id, string connectedToNode)
		: base(id, null)
	{
		ConnectedTo = connectedToNode;
	}
}
