using System;

namespace MS.Internal.FailoverClusters.Framework;

[Flags]
internal enum VirtualMachineReplicationFlags
{
	TestFailoverRunning = 8,
	PlannedFailover = 4,
	PendingInitialReplication = 2,
	EndpointProvider = 1
}
