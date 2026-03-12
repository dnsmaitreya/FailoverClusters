namespace Microsoft.FailoverClusters.Framework;

public enum PhysicalDiskUsage : ushort
{
	Unknown,
	AutoSelect,
	ManualSelect,
	HotSpare,
	Retired,
	Journal
}
