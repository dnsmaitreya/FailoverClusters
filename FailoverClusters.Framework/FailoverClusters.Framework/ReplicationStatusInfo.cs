using System;

namespace FailoverClusters.Framework;

public class ReplicationStatusInfo
{
	public Guid PartitionId { get; private set; }

	public ReplicationStatus ReplicationStatus { get; internal set; }

	public uint PercentageRecovered { get; set; }

	internal ReplicationStatusInfo(Guid partitionId, ReplicationStatus replicationStatus)
	{
		PartitionId = partitionId;
		ReplicationStatus = replicationStatus;
		PercentageRecovered = 100u;
	}
}

