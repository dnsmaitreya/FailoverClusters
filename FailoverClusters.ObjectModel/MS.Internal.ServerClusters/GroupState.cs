namespace MS.Internal.ServerClusters;

public enum GroupState
{
	Unknown = -1,
	Online,
	Offline,
	Failed,
	PartialOnline,
	Pending
}
