using System;

namespace Microsoft.FailoverClusters.Framework;

[Flags]
public enum GroupMoveFlags
{
	IgnoreResourceStatus = 1,
	ReturnToSourceNodeOnError = 2,
	Queued = 4,
	HighPriority = 8
}
