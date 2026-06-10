using System;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class ClusterResourceTypeReplicationEventArgs : ClusterEventArgs
{
	internal NativeMethods.WVR_EVENT_TYPE EventType { get; private set; }

	internal PCluster Cluster { get; set; }

	internal ClusterResourceTypeReplicationEventArgs(PCluster cluster, Guid id, NativeMethods.WVR_EVENT_TYPE eventType, ClusterException exception)
		: base(id, exception)
	{
		EventType = eventType;
		Cluster = cluster;
	}
}

