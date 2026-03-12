namespace Microsoft.FailoverClusters.Framework;

public enum VirtualMachineOfflineAction
{
	Turnoff = 0,
	Save = 1,
	Shutdown = 2,
	ForcedShutdown = 3,
	Unknown = 65535
}
