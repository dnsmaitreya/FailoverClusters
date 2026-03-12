using System;

namespace Microsoft.FailoverClusters.Framework;

[Flags]
public enum ClusterSharedVolumeFaultState
{
	NoFaults = 0,
	NoDirectIO = 1,
	NoAccess = 2,
	InMaintenance = 4,
	Dismounted = 8,
	[Filterable(false)]
	Fetching = 0x40000000
}
