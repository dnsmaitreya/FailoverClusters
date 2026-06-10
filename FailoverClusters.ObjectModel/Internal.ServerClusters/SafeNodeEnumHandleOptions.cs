using System;

namespace MS.Internal.ServerClusters;

[Flags]
internal enum SafeNodeEnumHandleOptions
{
	None = 0,
	NoCoreGroups = 1
}
