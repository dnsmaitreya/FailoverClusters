namespace FailoverClusters.Framework;

public enum ClusterAccessRights
{
	None = 0,
	MaximumAllowed = 33554432,
	GenericRead = int.MinValue,
	GenericAll = 268435456
}

