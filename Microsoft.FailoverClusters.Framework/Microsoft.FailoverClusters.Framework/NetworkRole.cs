namespace Microsoft.FailoverClusters.Framework;

public enum NetworkRole
{
	Unknown = -1,
	None = 0,
	InternalUse = 1,
	ClientAccess = 2,
	InternalAndClient = 3,
	[Filterable(false)]
	Fetching = 1073741824
}
