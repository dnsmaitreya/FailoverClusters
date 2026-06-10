namespace FailoverClusters.Framework;

public enum VolumeHealthStatus : ushort
{
	Unknown = 255,
	Healthy = 0,
	ScanNeeded = 1,
	SpotFixNeeded = 2,
	FullRepairNeeded = 3
}

