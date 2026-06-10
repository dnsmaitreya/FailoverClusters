using System;

namespace FailoverClusters.Framework;

[Flags]
public enum GroupFlags
{
	None = 0,
	Core = 1,
	[Filterable(false)]
	Fetching = 0x40000000
}

