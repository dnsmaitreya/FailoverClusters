using System;

namespace FailoverClusters.Framework;

[Flags]
public enum NodeStatusInformation
{
	None = 0,
	Isolated = 1,
	Quarantined = 2,
	DrainInProgress = 4,
	DrainCompleted = 8,
	DrainFailed = 0x10,
	AvoidPlacement = 0x20
}

