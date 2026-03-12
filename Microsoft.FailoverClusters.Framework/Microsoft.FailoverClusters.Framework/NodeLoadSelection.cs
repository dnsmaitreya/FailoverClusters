using System;

namespace Microsoft.FailoverClusters.Framework;

[Flags]
public enum NodeLoadSelection
{
	None = 0,
	Basic = 1,
	CommonProperties = 2,
	PrivateProperties = 4,
	OperatingSystemInformation = 0x1000,
	ProcessorInformation = 0x2000,
	ServerInformation = 0x4000,
	All = 0x7007,
	Reload = 0x20000000
}
