using System;

namespace FailoverClusters.Framework;

public class ClusterResourceIsQuorumChangedEventArgs : ClusterEventArgs
{
	public bool IsQuorum { get; private set; }

	public ClusterResourceIsQuorumChangedEventArgs(Guid id, bool isQuorum)
		: base(id, null)
	{
		IsQuorum = isQuorum;
	}
}

