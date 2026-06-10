namespace MS.Internal.ServerClusters;

public enum DiskReplicationEligible
{
	None = 0,
	Yes = 1,
	Offline = 2,
	NotGpt = 3,
	PartitionLayoutMismatch = 4,
	InsufficientFreeSpace = 5,
	NotInSameSite = 6,
	InSameSite = 7,
	FileSystemNotSupported = 8,
	AlreadyInReplication = 9,
	SameAsSpecifiedDisk = 10,
	Other = 9999
}
