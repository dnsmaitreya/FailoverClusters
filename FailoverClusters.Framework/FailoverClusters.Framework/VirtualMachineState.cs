namespace FailoverClusters.Framework;

public enum VirtualMachineState
{
	[Filterable(false)]
	Unknown = 0,
	Running = 2,
	PowerOff = 3,
	ShuttingDown = 4,
	Reset = 10,
	Saved = 32769,
	Saving = 32773,
	Paused = 32768,
	SnapshotInProgress = 32771,
	Migrating = 32772,
	Stopping = 32774,
	Pausing = 32776,
	Resuming = 32777,
	Starting = 32770,
	[Filterable(false)]
	Fetching = 1073741824
}

