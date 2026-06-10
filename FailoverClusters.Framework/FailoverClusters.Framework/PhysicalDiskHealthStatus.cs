namespace FailoverClusters.Framework;

public enum PhysicalDiskHealthStatus : ushort
{
	Healthy = 0,
	Warning = 1,
	Unhealthy = 2,
	Unknown = 5
}

