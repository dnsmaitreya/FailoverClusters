using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class AverageGroupMoveToSelectedCommand : GroupMoveCommandBase
{
	public AverageGroupMoveToSelectedCommand(Group group, ClusterObject clusterObjectIssuingMoveRequest)
		: base(group, clusterObjectIssuingMoveRequest, setInputParametersMoveTargetsOnOwnerChanged: true)
	{
	}

	protected override ClusterCommand GenerateInstance()
	{
		ClusterCommand openMoveToSelectedNodeDialogCommand = new ClusterCommand(base.ClusterGroup, "MoveSelectTo", ClusterCommandId.GroupMoveToSelectedNode, ClusterCommandCollectionId.GroupOwnership)
		{
			Text = CommandResources.Group_MoveToSelected,
			InputParameters = base.ClusterGroup.Cluster.AllUpNodes,
			CommandParameter = base.ClusterGroup,
			Description = string.Empty,
			ExecuteDelegate = base.DefaultExecuteMove
		};
		openMoveToSelectedNodeDialogCommand.CanExecuteDelegate = (object o) => DefaultCanExecuteMove(o, openMoveToSelectedNodeDialogCommand);
		return openMoveToSelectedNodeDialogCommand;
	}
}
