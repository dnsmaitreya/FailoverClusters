using System;
using System.Collections.Generic;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterStorageReplicationStatusChangedEventArgs : ClusterEventArgs
{
	public IList<ReplicationStatusInfo> ReplicationStatus { get; private set; }

	public ClusterStorageReplicationStatusChangedEventArgs(Guid id, IList<ReplicationStatusInfo> replicationStatus)
		: base(id, null)
	{
		ReplicationStatus = replicationStatus;
	}
}
