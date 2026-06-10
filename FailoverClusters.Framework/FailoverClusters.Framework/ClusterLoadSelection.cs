using System;

namespace FailoverClusters.Framework;

[Flags]
public enum ClusterLoadSelection
{
	None = 0,
	Basic = 1,
	CommonProperties = 2,
	PrivateProperties = 4,
	VersionInformation = 8,
	QuorumConfiguration = 0x1000,
	All = 0x100F,
	Reload = 0x20000000
}

