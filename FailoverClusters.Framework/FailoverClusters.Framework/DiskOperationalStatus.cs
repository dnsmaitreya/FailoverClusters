namespace FailoverClusters.Framework;

public enum DiskOperationalStatus : ushort
{
	Unknown,
	Online,
	NotReady,
	NoMedia,
	Offline,
	Failed,
	Missing
}

