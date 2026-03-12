namespace MS.Internal.ServerClusters.Management;

internal struct VerifyQuorumData
{
	internal string QuorumLossMessage;

	internal QuorumLossCheck QuorumCheck;

	internal QuorumVoterActionCheck QuorumVoterAction;
}
