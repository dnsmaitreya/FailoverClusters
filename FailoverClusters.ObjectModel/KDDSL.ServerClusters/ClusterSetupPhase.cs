namespace KDDSL.ServerClusters;

public enum ClusterSetupPhase
{
	ClusterInitialize = 1,
	ValidateNodeState = 100,
	QueryClusterNameAccount = 106,
	ValidateClusterNameAccount = 107,
	CreateClusterNameAccount = 108,
	ConfigureClusterNameAccount = 109,
	ValidateNetft = 102,
	ValidateClusDisk = 103,
	ConfigureClusSvc = 104,
	StartingClusSvc = 105,
	FormingCluster = 200,
	AddClusterProperties = 201,
	CreateResourceTypes = 202,
	CreateGroups = 203,
	CreateIPAddressResources = 204,
	CreateNetworkName = 205,
	ClusterGroupOnline = 206,
	GettingCurrentMembership = 300,
	AddNodeToCluster = 301,
	NodeUp = 302,
	MoveGroup = 400,
	DeleteGroup = 401,
	CleanupComputerObjects = 402,
	OfflineGroup = 403,
	EvictNode = 404,
	CleanupNode = 405,
	CoreGroupCleanup = 406,
	FailureCleanup = 999
}
