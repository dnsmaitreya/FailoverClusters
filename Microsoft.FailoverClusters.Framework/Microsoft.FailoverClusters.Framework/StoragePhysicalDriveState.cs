namespace Microsoft.FailoverClusters.Framework;

public enum StoragePhysicalDriveState
{
	Unknown = 0,
	BecomingReady = 1,
	UnrecognizedMetadata = 2,
	FailedMedia = 3,
	HardwareError = 4,
	Split = 5,
	StaleMetadata = 6,
	IOError = 7,
	Missing = 8,
	PredictingFailure = 9,
	NotUsable = 10,
	TransientError = 11,
	InService = 12,
	InMaintenance = 13,
	Okay = 14,
	Max = 15,
	[Filterable(false)]
	Fetching = 1073741824
}
