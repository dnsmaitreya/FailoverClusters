namespace Microsoft.FailoverClusters.Framework;

public enum VirtualMachineComputerSystemOperationalStatus
{
	Ok = 2,
	Degraded = 3,
	PredictiveFailure = 5,
	InService = 11,
	CreatingSnapshot = 32768,
	ApplyingSnapshot = 32769,
	DeletingSnapshot = 32770,
	WaitingToStart = 32771,
	MergingDisks = 32772,
	ExportingVirtualMachine = 32773,
	MigratingVirtualMachine = 32774,
	BackingUpVirtualMachine = 32776,
	ModifyingUpVirtualMachine = 32777,
	StorageMigrationPhaseOne = 32778,
	StorageMigrationPhaseTwo = 32779,
	[Filterable(false)]
	Fetching = 1073741824
}
