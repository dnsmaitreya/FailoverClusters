namespace FailoverClusters.Framework;

public enum OSProductType
{
	Unknown = 0,
	Client = 1,
	DomainController = 2,
	Server = 3,
	[Filterable(false)]
	Fetching = 1073741824
}

