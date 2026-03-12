using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class AverageGroupMoveCommand : GroupCommandBase
{
	public AverageGroupMoveToSelectedCommand AverageGroupMoveToSelectedCommand { get; protected set; }

	public AverageGroupMoveToBestCommand AverageGroupMoveToBestCommand { get; protected set; }

	public AverageGroupMoveCommand(Group group)
		: this(group, null)
	{
	}

	public AverageGroupMoveCommand(Group group, ClusterObject clusterObjectIssuingMoveRequest)
		: base(group)
	{
		AverageGroupMoveToSelectedCommand = new AverageGroupMoveToSelectedCommand(group, clusterObjectIssuingMoveRequest);
		AverageGroupMoveToBestCommand = new AverageGroupMoveToBestCommand(group, clusterObjectIssuingMoveRequest);
	}

	protected override ClusterCommand GenerateInstance()
	{
		ClusterCommandContainer obj = new ClusterCommandContainer(base.ClusterGroup, "MoveTo", ClusterCommandId.GroupMoveTo, ClusterCommandCollectionId.GroupOwnership)
		{
			Text = CommandResources.Group_Move,
			CommandParameter = base.ClusterGroup
		};
		ClusterCommand item = AverageGroupMoveToBestCommand;
		obj.ChildrenInternal.Add(item);
		ClusterCommand item2 = AverageGroupMoveToSelectedCommand;
		obj.ChildrenInternal.Add(item2);
		return obj;
	}
}
