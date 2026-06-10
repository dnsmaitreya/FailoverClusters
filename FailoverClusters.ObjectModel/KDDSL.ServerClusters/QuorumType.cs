namespace KDDSL.ServerClusters;

public enum QuorumType
{
	None,
	LegacyDisk,
	MajorityOfNodes,
	StorageWitness,
	FileShareWitness,
	Unknown,
	AzureWitness
}
