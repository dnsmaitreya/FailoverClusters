namespace FailoverClusters.Framework;

public enum ClusterNodeDrainStatus
{
	Unknown = -1,
	NotInitiated = 0,
	InProgress = 1,
	Completed = 2,
	Failed = 3,
	[Filterable(false)]
	Fetching = 1073741824
}

