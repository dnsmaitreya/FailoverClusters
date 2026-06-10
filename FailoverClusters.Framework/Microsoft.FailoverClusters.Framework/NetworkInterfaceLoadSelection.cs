using System;

namespace FailoverClusters.Framework;

[Flags]
public enum NetworkInterfaceLoadSelection
{
	None = 0,
	Basic = 1,
	CommonProperties = 2,
	PrivateProperties = 4,
	All = 7,
	Reload = 0x20000000
}

