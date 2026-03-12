namespace Microsoft.FailoverClusters.Framework;

public enum PartitionOperationalStatus : short
{
	Unknown = 0,
	Online = 1,
	NoMedia = 3,
	Offline = 4,
	Failed = 5
}
