namespace KDDSL.ServerClusters;

internal enum ClusterEnumType
{
	Node = 1,
	Network = 16,
	InternalNetwork = int.MinValue,
	NetworkInterface = 32,
	Group = 8,
	Resource = 4,
	ResourceType = 2,
	ClusterFileSystem = 1073741824
}
