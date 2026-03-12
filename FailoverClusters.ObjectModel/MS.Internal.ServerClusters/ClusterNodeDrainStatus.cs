namespace MS.Internal.ServerClusters;

public enum ClusterNodeDrainStatus
{
	Unknown = -1,
	NotInitiated,
	InProgress,
	Completed,
	Failed
}
