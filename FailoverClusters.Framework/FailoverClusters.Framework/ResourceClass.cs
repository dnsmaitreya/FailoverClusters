using System;

namespace FailoverClusters.Framework;

[Flags]
public enum ResourceClass
{
	Unknown = 0,
	Storage = 1,
	Network = 2,
	User = 0x8000,
	[Filterable(false)]
	Fetching = 0x40000000
}

