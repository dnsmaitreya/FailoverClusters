namespace KDDSL.ServerClusters;

public enum PossibleQuorumStorageStatus
{
	Valid,
	NotOnline,
	NoPartitionInformation,
	TypeNotValidForQuorum,
	HasDependencies,
	NoNtfsPartition,
	NtfsPartitionTooSmall,
	MaintenanceMode,
	ClusterSharedVolume
}
