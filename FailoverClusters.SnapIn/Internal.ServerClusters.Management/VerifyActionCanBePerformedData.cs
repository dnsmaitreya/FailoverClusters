namespace MS.Internal.ServerClusters.Management;

internal struct VerifyActionCanBePerformedData
{
	internal ClusterGroup ClusterGroup;

	internal VerifyQuorumData QuorumData;

	internal VerifyActionData ActionData;

	internal VerifyAction Verifications;

	public VerifyActionCanBePerformedData(ClusterGroup clusterGroup)
	{
		ClusterGroup = clusterGroup;
		QuorumData = default(VerifyQuorumData);
		ActionData = default(VerifyActionData);
		Verifications = VerifyAction.None;
	}
}
