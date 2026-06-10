namespace FailoverClusters.Framework;

public class StateCommandCollectionWrapper : WeakLazyBase<CommandCollection>
{
	private readonly Group clusterGroup;

	private readonly OnlineCommand onlineCommand;

	private readonly OfflineCommand offlineCommand;

	public StateCommandCollectionWrapper(Group group)
	{
		clusterGroup = group;
		onlineCommand = new OnlineCommand(group, this);
		offlineCommand = new OfflineCommand(group, this);
	}

	protected override CommandCollection GenerateInstance()
	{
		return new CommandCollection(ClusterCommandCollectionId.GroupState);
	}

	protected override void DoPostGenerateInstance(CommandCollection newValue)
	{
		newValue.Add(onlineCommand.GetInstance());
		newValue.Add(offlineCommand.GetInstance());
		base.DoPostGenerateInstance(newValue);
	}
}

