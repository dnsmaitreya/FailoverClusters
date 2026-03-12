using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterDataChangedEventArgs : EventArgs
{
	public ClusterDataChangedType ClusterDataChangedType { get; private set; }

	public Cluster Cluster { get; private set; }

	public Guid ClusterCacheId { get; private set; }

	public ClusterDataChangedEventArgs(Cluster cluster)
	{
		ClusterDataChangedType = ClusterDataChangedType.Addition;
		Cluster = cluster;
	}

	public ClusterDataChangedEventArgs(Guid clusterCacheId)
	{
		ClusterDataChangedType = ClusterDataChangedType.Removal;
		ClusterCacheId = clusterCacheId;
	}
}
