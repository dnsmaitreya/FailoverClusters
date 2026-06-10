namespace FailoverClusters.Framework;

public enum DiskHealthStatus : ushort
{
	Unknown = 0,
	Healthy = 1,
	Failing = 4,
	Failed = 8
}

