namespace FailoverClusters.Framework;

public enum DiskOfflineReason : uint
{
	Unknown,
	Policy,
	RedundantPath,
	Snapshot,
	Collision,
	ResourceExhaustion,
	CriticalWriteFailures,
	DataIntegrityScanRequired
}

