namespace Microsoft.FailoverClusters.Framework;

public enum VirtualMachineReplicationMode
{
	None = 0,
	Primary = 1,
	Recovery = 2,
	TestReplica = 3,
	ExtendedReplica = 4,
	[Filterable(false)]
	Fetching = 1073741824
}
