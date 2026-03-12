using System;

namespace Microsoft.FailoverClusters.Framework;

[Flags]
public enum VirtualMachineResourceLoadSelections
{
	Thumbnail = 0x1000,
	Status = 0x2000,
	Summary = 0x4000,
	Replication = 0x8000,
	Storage = 0x10000,
	ExtendedReplication = 0x20000,
	Checkpoints = 0x40000,
	AllDetails = 0x7F000,
	All = 0x7FFFF,
	Reload = 0x20000000
}
