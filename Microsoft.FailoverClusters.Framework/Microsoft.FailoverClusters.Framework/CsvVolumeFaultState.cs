namespace Microsoft.FailoverClusters.Framework;

public enum CsvVolumeFaultState
{
	VolumeStateNoFaults,
	VolumeStateNoDirectIO,
	VolumeStateNoAccess,
	VolumeStateInMaintenance,
	VolumeStateDismounted
}
