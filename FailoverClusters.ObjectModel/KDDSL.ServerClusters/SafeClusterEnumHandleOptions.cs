using System;

namespace KDDSL.ServerClusters;

[Flags]
internal enum SafeClusterEnumHandleOptions
{
	None = 0,
	NoCoreGroups = 1,
	NoCoreResources = 2
}
