namespace KDDSL.FailoverClusters.Framework;

public enum ClusterSetupPhrase
{
	None = 0,
	ClusterSetupPhaseInitialize = 1,
	ClusterSetupPhaseValidateNodeState = 100,
	ClusterSetupPhaseValidateNetft = 102,
	ClusterSetupPhaseValidateClusDisk = 103,
	ClusterSetupPhaseConfigureClusSvc = 104,
	ClusterSetupPhaseStartingClusSvc = 105,
	ClusterSetupPhaseQueryClusterNameAccount = 106,
	ClusterSetupPhaseValidateClusterNameAccount = 107,
	ClusterSetupPhaseCreateClusterAccount = 108,
	ClusterSetupPhaseConfigureClusterAccount = 109,
	ClusterSetupPhaseFormingCluster = 200,
	ClusterSetupPhaseAddClusterProperties = 201,
	ClusterSetupPhaseCreateResourceTypes = 202,
	ClusterSetupPhaseCreateGroups = 203,
	ClusterSetupPhaseCreateIpAddressResources = 204,
	ClusterSetupPhaseCreateNetworkName = 205,
	ClusterSetupPhaseClusterGroupOnline = 206,
	ClusterSetupPhaseGettingCurrentMembership = 300,
	ClusterSetupPhaseAddNodeToCluster = 301,
	ClusterSetupPhaseNodeUp = 302,
	ClusterSetupPhaseMoveGroup = 400,
	ClusterSetupPhaseDeleteGroup = 401,
	ClusterSetupPhaseCleanupCOs = 402,
	ClusterSetupPhaseOfflineGroup = 403,
	ClusterSetupPhaseEvictNode = 404,
	ClusterSetupPhaseCleanupNode = 405,
	ClusterSetupPhaseCoreGroupCleanup = 406,
	ClusterSetupPhaseFailureCleanup = 999
}
