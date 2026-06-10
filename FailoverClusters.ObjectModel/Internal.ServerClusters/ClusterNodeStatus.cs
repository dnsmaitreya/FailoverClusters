using System;

namespace MS.Internal.ServerClusters;

[Flags]
public enum ClusterNodeStatus
{
	None = 0,
	Isolated = 1,
	Quarantine = 2,
	DrainInProgress = 4,
	DrainCompleted = 8,
	DrainFailed = 0x10,
	AvoidPlacement = 0x20,
	Unknown = int.MinValue
}
