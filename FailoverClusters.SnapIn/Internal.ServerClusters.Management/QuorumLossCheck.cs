namespace MS.Internal.ServerClusters.Management;

internal enum QuorumLossCheck
{
	None,
	Offline,
	GroupChange,
	RemoveStorage
}
