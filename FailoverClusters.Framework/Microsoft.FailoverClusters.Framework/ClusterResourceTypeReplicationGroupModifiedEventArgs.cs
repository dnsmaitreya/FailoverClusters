using System;
using System.Collections.Generic;
using System.Text;
using FailoverClusters.UI.Common;
using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class ClusterResourceTypeReplicationGroupModifiedEventArgs : ClusterResourceTypeReplicationEventArgs
{
	internal NativeMethods.WVR_RESOURCE_TYPE_REPLICATION_GROUP_MODIFIED_NOTIFICATION Payload { get; private set; }

	internal IList<NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK> ReplicatedDisks { get; private set; }

	internal ClusterResourceTypeReplicationGroupModifiedEventArgs(PCluster cluster, Guid id, NativeMethods.WVR_EVENT_TYPE eventType, NativeMethods.WVR_RESOURCE_TYPE_REPLICATION_GROUP_MODIFIED_NOTIFICATION payload, IList<NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK> replicatedDisks, ClusterException exception)
		: base(cluster, id, eventType, exception)
	{
		Payload = payload;
		ReplicatedDisks = replicatedDisks;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("WVR_RESOURCE_TYPE_REPLICATION_GROUP_MODIFIED_NOTIFICATION [{0}]".FormatCurrentCulture(base.EventType));
		stringBuilder.AppendLine("Cluster Group Id: {0}".FormatCurrentCulture(Payload.ClusterGroupId));
		stringBuilder.AppendLine("Group Type: {0}".FormatCurrentCulture((ReplicationGroupRole)Payload.GroupType));
		stringBuilder.AppendLine("Replication Group Id: {0}".FormatCurrentCulture(Payload.ReplicationGroupId));
		stringBuilder.AppendLine("Replication Group Name: {0}".FormatCurrentCulture(Payload.ReplicationGroupName));
		foreach (NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK replicatedDisk in ReplicatedDisks)
		{
			stringBuilder.AppendLine("  REPLICATED DISK");
			stringBuilder.AppendLine("  Role: {0}".FormatCurrentCulture(replicatedDisk.Role));
			stringBuilder.AppendLine("  Cluster Resource Id: {0}".FormatCurrentCulture(replicatedDisk.ClusterResourceId));
			stringBuilder.AppendLine("  Replication Group Id: {0}".FormatCurrentCulture(replicatedDisk.ReplicationGroupId));
			stringBuilder.AppendLine("  Replication Group Name: {0}".FormatCurrentCulture(replicatedDisk.ReplicationGroupName));
		}
		return stringBuilder.ToString();
	}
}

