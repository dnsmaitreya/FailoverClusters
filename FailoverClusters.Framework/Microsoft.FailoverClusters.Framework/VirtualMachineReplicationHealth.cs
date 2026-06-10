namespace FailoverClusters.Framework;

public enum VirtualMachineReplicationHealth
{
	[Filterable(false)]
	NotApplicable = 0,
	Normal = 1,
	Warning = 2,
	Error = 3,
	[Filterable(false)]
	Fetching = 1073741824
}

