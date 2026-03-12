using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class OnlineCommand : GroupCommandBase
{
	private readonly StateCommandCollectionWrapper parentCollectionWrapper;

	public OnlineCommand(Group group, StateCommandCollectionWrapper parentCollectionWrapper)
		: base(group)
	{
		base.UpdateCanExecuteOnStateChange = true;
		this.parentCollectionWrapper = parentCollectionWrapper;
	}

	protected override ClusterCommand GenerateInstance()
	{
		CommandCollection instance = parentCollectionWrapper.GetInstance();
		return new ClusterCommand(base.ClusterGroup, "Online", ClusterCommandId.GroupOnline, instance.Category)
		{
			Text = CommandResources.BringGroupOnlineAction_Text,
			CanExecuteDelegate = (object x) => base.ClusterGroup.GroupState != 0 && base.ClusterGroup.GroupState != GroupState.Pending && base.ClusterGroup.GroupState != GroupState.Fetching,
			ExecuteDelegate = delegate
			{
				base.ClusterGroup.Online(enableOverride: true);
			}
		};
	}
}
