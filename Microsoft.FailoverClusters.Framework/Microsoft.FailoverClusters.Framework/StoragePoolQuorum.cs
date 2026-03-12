namespace Microsoft.FailoverClusters.Framework;

public enum StoragePoolQuorum
{
	Unknown = 0,
	Minority = 1,
	Majority = 2,
	Ok = 3,
	Max = 4,
	[Filterable(false)]
	Fetching = 1073741824
}
