using System;

namespace Microsoft.FailoverClusters.Framework;

[Flags]
public enum ClusterSharedRedirectedIoReason : ulong
{
	UserRequest = 1uL,
	UnsafeFileSystemFilter = 2uL,
	UnsafeVolumeFilter = 4uL,
	FileSystemTiering = 8uL,
	Max = 9223372036854775808uL
}
