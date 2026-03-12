using System;

namespace Microsoft.FailoverClusters.Framework;

[Flags]
public enum ResourceFlags
{
	None = 0,
	Core = 1,
	[Filterable(false)]
	Fetching = 0x40000000
}
