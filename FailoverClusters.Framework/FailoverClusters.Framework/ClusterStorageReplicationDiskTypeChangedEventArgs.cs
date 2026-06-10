using System;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class ClusterStorageReplicationDiskTypeChangedEventArgs : ClusterEventArgs
{
	public ReplicationDiskType ReplicationDiskType { get; private set; }

	public ClusterStorageReplicationDiskTypeChangedEventArgs(Guid id, ReplicationDiskType replicationDiskType)
		: base(id, null)
	{
		ReplicationDiskType = replicationDiskType;
	}
}

