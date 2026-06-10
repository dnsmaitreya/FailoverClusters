using System;

namespace MS.Internal.ServerClusters;

[Flags]
public enum ResourceStatusInformation : ulong
{
	Locked = 1uL,
	EmbeddedFailure = 2uL,
	InsufficientCpu = 4uL,
	InsufficientMemory = 8uL,
	InsufficientResources = 0x10uL,
	NetworkFailure = 0x20uL,
	Unmonitored = 0x40uL
}
