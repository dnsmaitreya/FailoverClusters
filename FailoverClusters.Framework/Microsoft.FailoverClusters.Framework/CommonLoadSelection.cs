using System;

namespace FailoverClusters.Framework;

[Flags]
public enum CommonLoadSelection
{
	None = 0,
	Basic = 1,
	CommonProperties = 2,
	PrivateProperties = 4,
	BasicExtended = 7,
	All = 0x10000000,
	Reload = 0x20000000
}

