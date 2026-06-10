using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class AverageGroupMoveToBestCommand : GroupMoveCommandBase
{
	public AverageGroupMoveToBestCommand(Group group, ClusterObject clusterObjectIssuingMoveRequest)
		: base(group, clusterObjectIssuingMoveRequest, setInputParametersMoveTargetsOnOwnerChanged: false)
	{
	}

	protected override ClusterCommand GenerateInstance()
	{
		ClusterCommand moveToCommand = new ClusterCommand(base.ClusterGroup, "MoveTo", ClusterCommandId.GroupMoveToBest, ClusterCommandCollectionId.GroupOwnership)
		{
			Text = CommandResources.Group_MoveToBestPossible,
			CommandParameter = null,
			ExecuteDelegate = base.DefaultExecuteMove
		};
		moveToCommand.CanExecuteDelegate = (object o) => DefaultCanExecuteMove(o, moveToCommand);
		return moveToCommand;
	}
}

