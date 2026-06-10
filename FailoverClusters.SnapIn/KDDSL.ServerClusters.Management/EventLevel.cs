using System;

namespace KDDSL.ServerClusters.Management;

[Flags]
internal enum EventLevel
{
	Critical = 1,
	Error = 2,
	Warning = 4,
	Informational = 8,
	Verbose = 0x10
}
