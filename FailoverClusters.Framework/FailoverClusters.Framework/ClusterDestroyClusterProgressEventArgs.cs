using System;

namespace FailoverClusters.Framework;

public class ClusterDestroyClusterProgressEventArgs : ClusterEventArgs
{
	public ClusterDestroyProgress DestroyProgress { get; private set; }

	public ClusterDestroyClusterProgressEventArgs(ClusterDestroyProgress clusterDestroyProgress, Guid id, ClusterException exception)
		: base(id, exception)
	{
		DestroyProgress = clusterDestroyProgress;
	}
}

