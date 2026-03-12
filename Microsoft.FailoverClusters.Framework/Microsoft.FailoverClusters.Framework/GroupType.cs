namespace Microsoft.FailoverClusters.Framework;

public enum GroupType
{
	[Filterable(false)]
	CoreCluster = 1,
	[Filterable(false)]
	AvailableStorage = 2,
	[Filterable(false)]
	Temporary = 3,
	[Filterable(false)]
	ClusterSharedVolume = 4,
	[Filterable(false)]
	ClusterStoragePool = 5,
	FileServer = 100,
	DhcpServer = 102,
	Dtc = 103,
	Msmq = 104,
	Wins = 105,
	StandAloneDfs = 106,
	GenericApplication = 107,
	GenericService = 108,
	GenericScript = 109,
	IScsiNameService = 110,
	VirtualMachine = 111,
	TsSessionBroker = 112,
	IScsiTarget = 113,
	ScaleOutFileServer = 114,
	VMReplicaBroker = 115,
	[Filterable(false)]
	TaskScheduler = 116,
	[Filterable(false)]
	ClusterAwareUpdating = 117,
	ScaleoutCluster = 118,
	StorageReplica = 119,
	[Filterable(false)]
	VirtualMachineReplicationCoordinator = 120,
	InfrastructureFileServer = 122,
	Unknown = 9999,
	[Filterable(false)]
	Fetching = 1073741824
}
