using System;

namespace MS.Internal.ServerClusters;

[Flags]
public enum ClusterAccessRights
{
	None = 0,
	MaximumAllowed = 0x2000000,
	GenericRead = int.MinValue,
	GenericAll = 0x10000000
}
