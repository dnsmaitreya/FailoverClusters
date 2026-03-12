namespace Microsoft.FailoverClusters.Framework;

public enum VirtualMachineIntegrationServicesStatus
{
	Unknown = 0,
	Installed = 1,
	NotInstalled = 2,
	[Filterable(false)]
	Fetching = 0x40000000
}
