namespace Microsoft.FailoverClusters.Framework;

public enum StorageNodeOperationalStatus : ushort
{
	Unknown = 0,
	Up = 2,
	Down = 6,
	Joining = 8,
	Paused = 10
}
