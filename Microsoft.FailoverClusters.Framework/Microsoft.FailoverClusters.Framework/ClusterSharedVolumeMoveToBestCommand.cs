namespace Microsoft.FailoverClusters.Framework;

public class ClusterSharedVolumeMoveToBestCommand : AverageGroupMoveToBestCommand
{
	public ClusterSharedVolumeMoveToBestCommand(Group group, ClusterObject clusterObjectIssuingMoveRequest)
		: base(group, clusterObjectIssuingMoveRequest)
	{
	}

	protected override void DoPostGenerateInstance(ClusterCommand newValue)
	{
		ClusterSharedVolumeRegisterCallbacks(newValue);
		base.DoPostGenerateInstance(newValue);
	}

	protected override bool DefaultCanExecuteMove(object nodeOrGroup, ClusterCommand clusterCommand)
	{
		if (ClusterSharedVolumeCanExecuteMove(nodeOrGroup, clusterCommand) != false)
		{
			return base.DefaultCanExecuteMove(nodeOrGroup, clusterCommand);
		}
		return false;
	}
}
