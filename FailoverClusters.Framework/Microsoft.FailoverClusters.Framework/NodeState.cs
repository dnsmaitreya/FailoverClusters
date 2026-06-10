namespace FailoverClusters.Framework;

public enum NodeState
{
	Unknown = -1,
	Up = 0,
	Down = 1,
	Pause = 2,
	Joining = 3,
	[Filterable(false)]
	Fetching = 1073741824
}

