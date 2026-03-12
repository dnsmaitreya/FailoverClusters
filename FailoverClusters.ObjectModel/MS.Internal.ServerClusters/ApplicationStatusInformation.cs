using System;

namespace MS.Internal.ServerClusters;

[Flags]
public enum ApplicationStatusInformation : ulong
{
	Locked = 1uL,
	Preempted = 2uL,
	Queued = 4uL,
	PhysicalResourcesLacking = 8uL,
	WaitingToStart = 0x10uL,
	EmbeddedFailure = 0x20uL,
	OfflineAntiAffinityConflict = 0x40uL,
	NetworkFailure = 0x80uL,
	Unmonitored = 0x100uL,
	WaitingForDependencies = 0x1000uL
}
