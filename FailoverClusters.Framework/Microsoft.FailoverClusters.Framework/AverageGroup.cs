namespace FailoverClusters.Framework;

public abstract class AverageGroup : Group
{
	public AverageGroupMoveCommand AverageGroupMoveCommand { get; private set; }

	protected override ClusterCommand InitializeOwnershipCommand()
	{
		return AverageGroupMoveCommand.GetInstance();
	}

	private void Init()
	{
		AverageGroupMoveCommand = new AverageGroupMoveCommand(this);
	}

	internal AverageGroup(Cluster cluster)
		: base(cluster)
	{
		Init();
	}
}

