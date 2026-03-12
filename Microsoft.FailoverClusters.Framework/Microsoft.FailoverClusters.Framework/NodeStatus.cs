namespace Microsoft.FailoverClusters.Framework;

public enum NodeStatus
{
	Unknown = -1,
	Up = 0,
	Down = 1,
	Paused = 2,
	Joining = 3,
	Draining = 4,
	DrainFailed = 5,
	Isolated = 6,
	Quarantined = 7,
	[Filterable(false)]
	Fetching = 1073741824
}
