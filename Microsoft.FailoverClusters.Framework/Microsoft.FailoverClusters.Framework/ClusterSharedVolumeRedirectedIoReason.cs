using System;

namespace Microsoft.FailoverClusters.Framework;

[Flags]
public enum ClusterSharedVolumeRedirectedIoReason : ulong
{
	ReasonNoDiskConnectivity = 1uL,
	ReasonStorageSpaceNotAttached = 2uL,
	Max = 9223372036854775808uL
}
