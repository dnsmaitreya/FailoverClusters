namespace Microsoft.FailoverClusters.Framework;

public enum GroupState
{
	[Filterable(false)]
	Unknown = -1,
	Online = 0,
	Offline = 1,
	Failed = 2,
	[Filterable(false)]
	PartialOnline = 3,
	Pending = 4,
	[Filterable(false)]
	Fetching = 1073741824
}
