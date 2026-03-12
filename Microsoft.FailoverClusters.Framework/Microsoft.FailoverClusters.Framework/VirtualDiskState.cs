namespace Microsoft.FailoverClusters.Framework;

public enum VirtualDiskState
{
	Unknown = 0,
	Detached = 1,
	NoRedundancy = 2,
	Degraded = 3,
	Incomplete = 4,
	Regenerating = 5,
	NeedsRebalance = 6,
	Ok = 7,
	Max = 8,
	[Filterable(false)]
	Fetching = 1073741824
}
