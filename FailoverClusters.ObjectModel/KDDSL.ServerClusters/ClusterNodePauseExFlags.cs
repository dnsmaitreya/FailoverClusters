using System;

namespace KDDSL.ServerClusters;

[Flags]
public enum ClusterNodePauseExFlags
{
	None = 0,
	RemainOnPausedNodeOnMoveError = 1,
	AvoidPlacement = 2,
	RetryDrainOnFailure = 4
}
