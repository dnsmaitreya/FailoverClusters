using System;

namespace FailoverClusters.Framework;

[Flags]
public enum GroupSubStatus
{
	None = 0,
	Locked = 1,
	Preempted = 2,
	WaitingInQueueForMove = 4,
	PhysicalResourcesLacking = 8,
	WaitingToStart = 0x10,
	EmbeddedFailure = 0x20,
	AffinityConflict = 0x40,
	NetworkFailure = 0x80,
	Unmonitored = 0x100,
	WaitingForDependencies = 0x1000,
	[Filterable(false)]
	Fetching = 0x40000000
}

