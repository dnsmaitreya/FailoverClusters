namespace MS.Internal.ServerClusters;

public enum CsvVolumeRedirectedReason : ulong
{
	NoDiskConnectivity = 1uL,
	StorageSpaceNotAttached = 2uL,
	VolumeReplicationEnabled = 4uL,
	Max = 9223372036854775808uL
}
