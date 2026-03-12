namespace Microsoft.FailoverClusters.Framework;

public enum VirtualMachineReplicationState
{
	Disabled = 0,
	Ready = 1,
	WaitingToCompleteInitialReplication = 2,
	Replicating = 3,
	SyncedReplicationComplete = 4,
	Recovered = 5,
	Committed = 6,
	Suspended = 7,
	Critical = 8,
	WaitingForStartResynchronize = 9,
	Resynchronize = 10,
	ResynchronizeSuspended = 11,
	FailoverInProgress = 12,
	FailbackInProgress = 13,
	FailbackComplete = 14,
	WaitingForUpdateCompletion = 15,
	UpdateCritical = 16,
	[Filterable(false)]
	NotApplicable = 65535,
	[Filterable(false)]
	Fetching = 1073741824
}
