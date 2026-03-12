using System;

namespace Microsoft.FailoverClusters.Framework;

[Flags]
public enum ResourceSubStatus
{
	None = 0,
	Locked = 1,
	EmbeddedFailure = 2,
	FailedDueToInsufficientCpu = 4,
	FailedDueToInsufficientMemory = 8,
	FailedDueToInsufficientGenericResources = 0x10,
	[Filterable(false)]
	Fetching = 0x40000000
}
