namespace FailoverClusters.Framework;

public enum NetworkInterfaceState
{
	Unknown = -1,
	Unavailable = 0,
	Failed = 1,
	Unreachable = 2,
	Up = 3,
	[Filterable(false)]
	Fetching = 1073741824
}

