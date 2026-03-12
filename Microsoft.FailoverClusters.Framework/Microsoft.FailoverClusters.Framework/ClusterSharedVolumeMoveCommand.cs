namespace Microsoft.FailoverClusters.Framework;

public class ClusterSharedVolumeMoveCommand : AverageGroupMoveCommand
{
	public ClusterSharedVolumeMoveCommand(Group group, ClusterObject clusterObjectIssuingMoveRequest)
		: base(group, clusterObjectIssuingMoveRequest)
	{
		base.AverageGroupMoveToSelectedCommand = new ClusterSharedVolumeMoveToSelectedCommand(group, clusterObjectIssuingMoveRequest);
		base.AverageGroupMoveToBestCommand = new ClusterSharedVolumeMoveToBestCommand(group, clusterObjectIssuingMoveRequest);
	}
}
