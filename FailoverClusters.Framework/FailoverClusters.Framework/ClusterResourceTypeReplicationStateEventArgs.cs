using System;
using System.Text;
using FailoverClusters.UI.Common;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class ClusterResourceTypeReplicationStateEventArgs : ClusterResourceTypeReplicationEventArgs
{
	internal NativeMethods.WVR_RESOURCE_TYPE_REPLICA_STATE_NOTIFICATION Payload { get; private set; }

	internal ClusterResourceTypeReplicationStateEventArgs(PCluster cluster, Guid id, NativeMethods.WVR_EVENT_TYPE eventType, NativeMethods.WVR_RESOURCE_TYPE_REPLICA_STATE_NOTIFICATION payload, ClusterException exception)
		: base(cluster, id, eventType, exception)
	{
		Payload = payload;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("WVR_RESOURCE_TYPE_REPLICA_STATE_NOTIFICATION [{0}]".FormatCurrentCulture(base.EventType));
		stringBuilder.AppendLine("Replication State: {0}".FormatCurrentCulture(Payload.ReplicationState));
		stringBuilder.AppendLine("Percentage Recovered: {0}".FormatCurrentCulture(Payload.PercentageRecovered));
		stringBuilder.AppendLine("Partition Id: {0}".FormatCurrentCulture(Payload.PartitionId));
		stringBuilder.AppendLine("Replication Group Id: {0}".FormatCurrentCulture(Payload.ReplicationGroupId));
		stringBuilder.AppendLine("Replication Group Name: {0}".FormatCurrentCulture(Payload.ReplicationGroupName));
		return stringBuilder.ToString();
	}
}

