using System;

namespace FailoverClusters.Framework;

[Flags]
public enum EnumConstants
{
	Fetching = 0x40000000,
	Reload = 0x20000000,
	All = 0x10000000
}

