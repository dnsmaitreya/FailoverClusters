namespace Microsoft.FailoverClusters.Framework;

public enum NetworkState
{
	Unknown = -1,
	Unavailable = 0,
	Down = 1,
	Partitioned = 2,
	Up = 3,
	[Filterable(false)]
	Fetching = 1073741824
}
