namespace FailoverClusters.Framework;

public enum VirtualDiskOperationalStatus
{
	Unknown = 0,
	Other = 1,
	Ok = 2,
	Degraded = 3,
	Stressed = 4,
	PredictiveFailure = 5,
	Error = 6,
	NonRecoverableError = 7,
	Starting = 8,
	Stopping = 9,
	Stopped = 10,
	InService = 11,
	NoContact = 12,
	LostCommunication = 13,
	Aborted = 14,
	Dormant = 15,
	SupportingEntityError = 16,
	Completed = 17,
	PowerMode = 18,
	Relocating = 19,
	Detached = 53250,
	Incomplete = 53251,
	NoRedundancy = 53284
}

