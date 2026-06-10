using System;

namespace KDDSL.ServerClusters.Management;

[Flags]
internal enum VerifyAction
{
	None = 0,
	QuorumLoss = 1,
	NetworkName = 2,
	HostedGroups = 4
}
