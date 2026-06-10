namespace KDDSL.ServerClusters;

public enum ClusterSharedVolumeFaultState
{
	Unknown = int.MaxValue,
	NoFaults = 0,
	NoDirectIO = 1,
	NoAccess = 2,
	InMaintenance = 4
}
