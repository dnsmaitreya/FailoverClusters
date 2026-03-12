using System;

namespace Microsoft.FailoverClusters.Framework;

[Flags]
public enum ResourceTypeLoadSelection
{
	None = 0,
	Basic = 1,
	CommonProperties = 2,
	PrivateProperties = 4,
	PossibleOwners = 8,
	All = 0xF,
	Reload = 0x20000000
}
