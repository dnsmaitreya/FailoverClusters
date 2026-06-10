namespace FailoverClusters.Framework;

public enum CannotPoolReason : ushort
{
	Unknown = 0,
	Other = 1,
	InAPool = 2,
	NotHealthy = 3,
	RemovableMedia = 4,
	InUseByCluster = 5,
	Offline = 6,
	InsufficientCapacity = 7,
	SpareDisk = 8,
	ReservedBySubsystem = 9,
	MicrosoftReserved = 32768,
	VendorReserved = 32769
}

