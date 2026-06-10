using System;
using System.Text;
using FailoverClusters.UI.Common;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class ClusterResourceTypeReplicationGroupDeletedEventArgs : ClusterResourceTypeReplicationEventArgs
{
	internal NativeMethods.WVR_RESOURCE_TYPE_REPLICATION_GROUP_NOTIFICATION Payload { get; private set; }

	internal ClusterResourceTypeReplicationGroupDeletedEventArgs(PCluster cluster, Guid id, NativeMethods.WVR_EVENT_TYPE eventType, NativeMethods.WVR_RESOURCE_TYPE_REPLICATION_GROUP_NOTIFICATION payload, ClusterException exception)
		: base(cluster, id, eventType, exception)
	{
		Payload = payload;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("WVR_RESOURCE_TYPE_REPLICATION_GROUP_NOTIFICATION [{0}]".FormatCurrentCulture(base.EventType));
		stringBuilder.AppendLine("Replication Group Id: {0}".FormatCurrentCulture(Payload.ReplicationGroupId));
		stringBuilder.AppendLine("Replication Group Name: {0}".FormatCurrentCulture(Payload.ReplicationGroupName));
		return stringBuilder.ToString();
	}
}

