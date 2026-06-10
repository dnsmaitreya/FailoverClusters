namespace KDDSL.ServerClusters.Management;

internal struct VerifyQuorumData
{
	internal string QuorumLossMessage;

	internal QuorumLossCheck QuorumCheck;

	internal QuorumVoterActionCheck QuorumVoterAction;
}
