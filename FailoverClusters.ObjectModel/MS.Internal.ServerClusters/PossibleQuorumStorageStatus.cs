namespace MS.Internal.ServerClusters;

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
