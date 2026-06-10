using System;

namespace KDDSL.ServerClusters.Management;

[Flags]
internal enum StorageType
{
	Formatted = 1,
	NonFormatted = 2,
	All = 3
}
