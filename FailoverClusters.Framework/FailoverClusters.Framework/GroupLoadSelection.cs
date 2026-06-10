using System;

namespace FailoverClusters.Framework;

[Flags]
public enum GroupLoadSelection
{
	None = 0,
	Basic = 1,
	CommonProperties = 2,
	PrivateProperties = 4,
	PreferredOwners = 8,
	All = 0xF,
	Reload = 0x20000000
}

