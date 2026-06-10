namespace FailoverClusters.Framework;

public static class Constants
{
	public const GroupMoveFlags LiveMigrationFlags = GroupMoveFlags.ReturnToSourceNodeOnError | GroupMoveFlags.Queued | GroupMoveFlags.HighPriority;

	public const int PassThroughDiskValue = 6;

	public const string NetNameDeleteVcoOnResCleanup = "DeleteVcoOnResCleanup";

	public const string NetNameNamePropertyName = "Name";

	public const string OwnerGroup = "ownergroup";

	public const string MaintenanceModePropertyName = "MaintenanceMode";

	public const string DiskRunCheckDiskPropertyName = "DiskRunChkDsk";

	public const string DiskVolumeInfoPropertyName = "DiskVolumeInfo";

	public const string PoolIdPropertyName = "PoolId";

	public const string PoolNamePropertyName = "Name";

	public const string PoolDescriptionPropertyName = "Description";

	public const string VirtualMachineIdPropertyName = "vmId";

	public const string VirtualDiskIdPropertyName = "VirtualDiskId";

	public const string VirtualDiskNamePropertyName = "VirtualDiskName";

	public const string VirtualDiskDescriptionPropertyName = "VirtualDiskDescription";

	public const string VirtualDiskResiliencyTypePropertyName = "VirtualDiskResiliencyType";

	public const string VirtualDiskHealthPropertyName = "VirtualDiskHealth";

	public const string VirtualDiskProvisioningPropertyName = "VirtualDiskProvisioning";

	public const string VirtualDiskResiliencyColumnsPropertyName = "VirtualDiskResiliencyColumns";

	public const string VirtualDiskResiliencyInterleavePropertyName = "VirtualDiskResiliencyInterleave";

	public const string VirtualDiskStatePropertyName = "VirtualDiskState";

	public const string WmiAvailableStoragePoolIdPropertyName = "Id";

	public const string WmiAvailableStoragePoolNamePropertyName = "Name";

	public const string WmiAvailableStoragePoolQuorumStatusPropertyName = "QuorumStatus";

	public const string WmiAvailableStoragePoolHealthStatusPropertyName = "HealthStatus";

	public const string WmiAvailableStoragePoolTotalSizePropertyName = "TotalSize";

	public const string WmiAvailableStoragePoolUsagePropertyName = "Usage";

	public const string WqlQueryDialect = "WQL";

	public const string StoragePoolNamePropertyName = "Name";

	public const string StoragePoolDescriptionPropertyName = "Description";

	public const string StoragePoolName2PropertyName = "Name_";

	public const string StoragePoolDescription2PropertyName = "Description_";

	public const string StoragePoolHealthPropertyName = "Health";

	public const string StoragePoolQuorumPropertyName = "State";

	public const string StoragePoolTotalCapacityPropertyName = "TotalCapacity";

	public const string StoragePoolDriveIdsPropertyName = "DriveIds";

	public const string StoragePoolTotalCapacityStorageSizePropertyName = "TotalCapacityStorageSize";

	public const string StoragePoolConsumedCapacityPropertyName = "ConsumedCapacity";

	public const string StoragePoolConsumedCapacityStorageSizePropertyName = "ConsumedCapacityStorageSize";

	public const string StoragePoolFreeCapacityPropertyName = "FreeCapacity";

	public const string StoragePoolFreeCapacityStorageSizePropertyName = "FreeCapacityStorageSize";

	public const string ClusterPropertySharedVolumesRootName = "SharedVolumesRoot";

	public const string ClusterPropertyManagementPointType = "AdminAccessPoint";

	public const string ClusterPropertyFunctionalLevel = "ClusterFunctionalLevel";

	public const string ClusterFileServerDefaultSodaShareName = "ClusterStorage$";

	public const string ClusterSharedVolumeFileSystemName = "CSVFS";

	public const string DnsSuffixPropertyName = "DnsSuffix";

	public const string FixQuorumPropertyName = "FixQuorum";

	public const string RecentEventsResetTimePropertyName = "RecentEventsResetTime";

	public const string DynamicQuorumEnabledPropertyName = "DynamicQuorumEnabled";

	public const string ResourceTypeNameGenericApplication = "Generic Application";

	public const string ResourceTypeNameGenericService = "Generic Service";

	public const string ResourceTypeNameGenericScript = "Generic Script";

	public const string ResourceTypeNameIPAddress = "IP Address";

	public const string ResourceTypeNameNetworkName = "Network Name";

	public const string ResourceTypeNameDistributedNetName = "Distributed Network Name";

	public const string ResourceTypeNameFileShare = "File Share";

	public const string ResourceTypeNamePrintSpooler = "Print Spooler";

	public const string ResourceTypeNameIPVersion6 = "IPv6 Address";

	public const string ResourceTypeNameIPVersion6Tunnel = "IPv6 Tunnel Address";

	public const string ResourceTypeNameVolumeShadowCopy = "Volume Shadow Copy Service Task";

	public const string ResourceTypeNameWinsService = "WINS Service";

	public const string ResourceTypeNameDhcpService = "DHCP Service";

	public const string ResourceTypeNameMsmq = "MSMQ";

	public const string ResourceTypeNameMsmqTriggers = "MSMQTriggers";

	public const string ResourceTypeNameDtc = "Distributed Transaction Coordinator";

	public const string ResourceTypeNameNetworkFileSystemShare = "NFS Share";

	public const string ResourceTypeNameNetworkFileSystem = "Network File System";

	public const string ResourceTypeNameIScsiNameService = "iSNS";

	public const string ResourceTypeNamePhysicalDisk = "Physical Disk";

	public const string ResourceTypeNameFileServerWitness = "File Share Witness";

	public const string ResourceTypeNameCloudWitness = "Cloud Witness";

	public const string ResourceTypeNameFileServer = "File Server";

	public const string ResourceTypeNameScaleOutFileServer = "Scale Out File Server";

	public const string ResourceTypeNameDistributedFileSystem = "Distributed File System";

	public const string ResourceTypeNameDfsReplicatedFolder = "DFS Replicated Folder";

	public const string ResourceTypeNameVirtualMachine = "Virtual Machine";

	public const string ResourceTypeNameVirtualMachineConfiguration = "Virtual Machine Configuration";

	public const string ResourceTypeNameIScsiTarget = "iSCSI Target Server";

	public const string ResourceTypeNameStoragePool = "Storage Pool";

	public const string ResourceTypeNameTaskScheduler = "Task Scheduler";

	public const string ResourceTypeNameHyperVNetworkVirtualizationProviderAddress = "Provider Address";

	public const string ResourceTypeNameNetworkAddressTranslator = "Nat";

	public const string ResourceTypeNameVirtualMachineReplicationBroker = "Virtual Machine Replication Broker";

	public const string ResourceTypeNameClusterFileSystem = "cluster file system";

	public const string ResourceTypeNameClusterAwareUpdating = "clusterawareupdatingresource";

	public const string ResourceTypeNameDisjointIPv4Address = "Disjoint IPv4 Address";

	public const string ResourceTypeNameDisjointIPv6Address = "Disjoint IPv6 Address";

	public const string ResourceTypeNameStorageReplica = "Storage Replica";

	public const string ResourceTypeNameHealthServices = "Health Service";

	public const string ResourceTypeNameStorageQoS = "Storage QoS Policy Manager";

	public const string ResourceTypeNameHyperVClusterWmi = "Virtual Machine Cluster WMI";

	public const string ResourceTypeNameVirtualMachineReplicationCoordinator = "Virtual Machine Replication Coordinator";

	public const string ResourceTypeNameSDDCManagement = "SDDC Management";

	public const string ResourceTypeNameCrossClusterDependencyOrchestrator = "Cross Cluster Dependency Orchestrator";

	public const string ResourceTypeNameOther = "Other";

	public const string VirtualMachineManufacturerName = "Corporation";

	public const string FileSystemTypeCsv = "CSVFS";

	public const string FileSystemTypeNtfs = "NTFS";

	public const string FileSystemTypeExFat = "exFAT";

	public const string FileSystemTypeFat32 = "FAT32";

	public const string FileSystemTypeFat = "FAT";

	public const string FileSystemTypeReFs = "ReFS";

	public const string FileSystemTypeRaw = "RAW";

	public const string FileSystemTypeCsvFs = "CSVFS";

	public const string ClusterCoreGroupName = "Cluster Group";

	public const string CauResourceNamePropertyName = "CauResourceName";

	public const string CauLastRunCompletedPropertyName = "CauLastRunCompleted";

	public const string AutoBalancerModePropertyName = "AutoBalancerMode";

	public const string AutoBalancerLevelPropertyName = "AutoBalancerLevel";

	public const string FaultDomainSitePropertyName = "Site";

	public const string FaultDomainRackPropertyName = "Rack";

	public const string FaultDomainChassisPropertyName = "Chassis";

	public const char FaultDomainPropertyValueSeparator = ':';
}

