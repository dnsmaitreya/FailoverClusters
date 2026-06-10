using System;

namespace MS.Internal.ServerClusters.Management;

[Flags]
internal enum StorageType
{
	Formatted = 1,
	NonFormatted = 2,
	All = 3
}
