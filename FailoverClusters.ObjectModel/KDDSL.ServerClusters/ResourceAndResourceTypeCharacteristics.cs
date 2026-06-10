using System;

namespace KDDSL.ServerClusters;

[Flags]
public enum ResourceAndResourceTypeCharacteristics
{
	Quorum = 1,
	DeleteRequiresAllNodes = 2,
	LocalQuorum = 4,
	LocalQuorumDebug = 8,
	RequiresStateChangeReason = 0x10,
	BroadcastDelete = 0x20,
	SingleClusterInstance = 0x40,
	SingleGroupInstance = 0x80,
	SharedVolumeCompatible = 0x100,
	PlacementAware = 0x200,
	MonitorDetach = 0x400,
	MonitorReattach = 0x800,
	OperationContext = 0x1000,
	Clones = 0x2000,
	NotPreemptable = 0x4000,
	NotifyNewOwner = 0x8000,
	SupportsUnmonitoredState = 0x10000,
	Infrastructure = 0x20000,
	CheckDrainVeto = 0x40000
}
