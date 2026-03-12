namespace Microsoft.FailoverClusters.Framework;

public enum ApplicationStatus
{
	[Filterable(false)]
	Unknown = -1,
	Running = 0,
	Stopped = 1,
	Starting = 2,
	Stopping = 3,
	Paused = 4,
	Saved = 5,
	Saving = 6,
	PartiallyRunning = 7,
	Pending = 8,
	Failed = 9,
	LiveMigrationQueued = 10,
	LiveMigrating = 11,
	SnapshotInProgress = 12,
	PowerOff = 13,
	Unmonitored = 14,
	[Filterable(false)]
	Fetching = 1073741824
}
