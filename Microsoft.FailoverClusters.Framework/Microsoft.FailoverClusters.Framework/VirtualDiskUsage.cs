namespace Microsoft.FailoverClusters.Framework;

public enum VirtualDiskUsage
{
	Unknown,
	Other,
	Unrestricted,
	ReservedComputerSystem,
	ReservedReplicationServices,
	ReservedMigrationServices,
	LocalReplicaSource,
	RemoteReplicaSource,
	LocalReplicaTarget,
	RemoteReplicaTarget,
	LocalReplicaSourceOrTarget,
	RemoteReplicaSourceOrTarget,
	DeltaReplicaTarget,
	ElementComponent,
	ReservedPoolContributor,
	CompositeVolumeMember,
	CompositeVirtualDiskMember,
	ReservedSparing
}
