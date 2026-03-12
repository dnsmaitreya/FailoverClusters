namespace MS.Internal.ServerClusters;

public enum VirtualMachineLiveMigrationState
{
	Unknown = 65535,
	NotStarted = 0,
	SetupPhase = 1,
	MemoryTransferPhase = 2,
	MovePhase = 3,
	Canceled = 4,
	Failed = 5,
	MigratingVirtualMachine = 32774
}
