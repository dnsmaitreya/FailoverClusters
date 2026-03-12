namespace Microsoft.FailoverClusters.Framework;

public enum ReplicationStatus
{
	Unknown = 0,
	ContinuouslyReplicating = 1,
	WaitingForQuorum = 2,
	RecoveringFromReplicationLog = 3,
	Failed = 4,
	ReplicationSuspended = 5,
	ConnectingToSource = 6,
	WaitingForDestination = 7,
	LogRecordCopyFromSource = 8,
	BlockCopyFromSource = 9,
	InitialBlockCopy = 10,
	ContinuouslyReplicatingOutOfRpo = 11,
	ContinuouslyReplicatingInRpo = 12,
	LogRecordCopyToDestination = 13,
	BlockCopyToDestination = 14,
	NotInPartnership = 15,
	[Filterable(false)]
	Fetching = 1073741824
}
