namespace Microsoft.FailoverClusters.Framework;

public enum StoragePhysicalDriveUsage
{
	Unknown = 0,
	AutoAllocation = 1,
	ManualAllocation = 2,
	Spare = 3,
	Journal = 4,
	Retired = 5,
	Max = 6,
	[Filterable(false)]
	Fetching = 1073741824
}
