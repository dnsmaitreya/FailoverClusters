namespace KDDSL.ServerClusters;

public enum ResourceState
{
	Unknown = -1,
	Inherited = 0,
	Initializing = 1,
	Online = 2,
	Offline = 3,
	Failed = 4,
	Pending = 128,
	OnlinePending = 129,
	OfflinePending = 130
}
