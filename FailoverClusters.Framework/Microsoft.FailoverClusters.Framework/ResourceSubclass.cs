using System;

namespace FailoverClusters.Framework;

[Flags]
public enum ResourceSubclass : long
{
	None = 0L,
	Shared = 0x80000000L,
	Compliant = 0x40000000L,
	Replication = 0x10000000L,
	[Filterable(false)]
	Fetching = 0x40000000L
}

