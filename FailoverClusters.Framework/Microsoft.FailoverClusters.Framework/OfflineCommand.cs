using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class OfflineCommand : GroupCommandBase
{
	private readonly StateCommandCollectionWrapper parentCollectionWrapper;

	public OfflineCommand(Group group, StateCommandCollectionWrapper parentCollectionWrapper)
		: base(group)
	{
		base.UpdateCanExecuteOnStateChange = true;
		this.parentCollectionWrapper = parentCollectionWrapper;
	}

	protected override ClusterCommand GenerateInstance()
	{
		CommandCollection instance = parentCollectionWrapper.GetInstance();
		return new ClusterCommand(base.ClusterGroup, "Offline", ClusterCommandId.GroupOffline, instance.Category)
		{
			Text = CommandResources.TakeGroupOfflineAction_Text,
			CanExecuteDelegate = (object x) => base.ClusterGroup.GroupState != GroupState.Offline && base.ClusterGroup.GroupState != GroupState.Fetching,
			ExecuteDelegate = delegate
			{
				base.ClusterGroup.Offline(enableOverride: true);
			}
		};
	}
}

