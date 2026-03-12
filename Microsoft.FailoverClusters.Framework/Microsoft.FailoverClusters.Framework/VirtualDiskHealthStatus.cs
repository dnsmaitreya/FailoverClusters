namespace Microsoft.FailoverClusters.Framework;

public enum VirtualDiskHealthStatus : ushort
{
	Healthy = 0,
	Warning = 1,
	Unhealthy = 2,
	Unknown = 5
}
