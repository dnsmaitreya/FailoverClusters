using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterStorageReplicationInfoChangedEventArgs : ClusterEventArgs
{
	public ReplicationInfo ReplicationInfo { get; private set; }

	public ClusterStorageReplicationInfoChangedEventArgs(Guid id, ReplicationInfo replicationInfo)
		: base(id, null)
	{
		ReplicationInfo = replicationInfo;
	}
}
