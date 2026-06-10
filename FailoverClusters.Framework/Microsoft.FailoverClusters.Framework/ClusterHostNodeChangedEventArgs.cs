using System;

namespace FailoverClusters.Framework;

public class ClusterHostNodeChangedEventArgs : ClusterEventArgs
{
	public string HostNode { get; internal set; }

	public ClusterHostNodeChangedEventArgs(Guid id, string hostNode)
		: base(id, null)
	{
		HostNode = hostNode;
	}
}

