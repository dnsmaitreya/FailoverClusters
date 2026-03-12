namespace Microsoft.FailoverClusters.Framework;

public enum VirtualMachineHeartbeatStatus
{
	Unknown = 0,
	Ok = 2,
	Error = 6,
	NoContact = 12,
	LostCommunication = 13,
	Disabled = 32896,
	[Filterable(false)]
	Fetching = 1073741824
}
