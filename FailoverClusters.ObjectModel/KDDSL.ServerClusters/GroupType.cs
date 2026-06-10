namespace KDDSL.ServerClusters;

public enum GroupType : uint
{
	CoreCluster = 1u,
	AvailableStorage = 2u,
	Temporary = 3u,
	ClusterSharedVolume = 4u,
	FileServer = 100u,
	PrintServer = 101u,
	DhcpServer = 102u,
	Dtc = 103u,
	Msmq = 104u,
	Wins = 105u,
	StandAloneDfs = 106u,
	GenericApplication = 107u,
	GenericService = 108u,
	GenericScript = 109u,
	IScsiNameService = 110u,
	VirtualMachine = 111u,
	TsSessionBroker = 112u,
	IScsiTarget = 113u,
	ScaleoutFileServer = 114u,
	ClusterStoragePool = 5u,
	VMReplicaBroker = 115u,
	TaskScheduler = 116u,
	ClusterAwareUpdating = 117u,
	StorageReplica = 119u,
	VirtualMachineReplicationCoordinator = 120u,
	InfrastructureFileServer = 122u,
	Unknown = 9999u
}
