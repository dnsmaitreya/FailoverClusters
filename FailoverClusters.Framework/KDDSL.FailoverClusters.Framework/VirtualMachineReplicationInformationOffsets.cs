namespace KDDSL.FailoverClusters.Framework;

internal enum VirtualMachineReplicationInformationOffsets
{
	ReplicationMode = 0,
	ReplicationRelationshipType = 16,
	ReplicationVersion = 30,
	ReplicationState = 32,
	ExtendedReplicationState = 40,
	ExtendedReplicationHealth = 50,
	ExtendedReplicationFlags = 52,
	ReplicationHealth = 58,
	ReplicationFlags = 60
}
