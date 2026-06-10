namespace FailoverClusters.Framework;

public enum StoragePoolHealth
{
	Unknown = 0,
	Unhealthy = 1,
	Warning = 2,
	Healthy = 3,
	Max = 4,
	[Filterable(false)]
	Fetching = 1073741824
}

