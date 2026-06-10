namespace FailoverClusters.Framework;

public enum VirtualDiskResiliencyType
{
	Unknown = 0,
	Simple = 1,
	Mirror = 2,
	Parity = 3,
	Max = 4,
	[Filterable(false)]
	Fetching = 1073741824
}

