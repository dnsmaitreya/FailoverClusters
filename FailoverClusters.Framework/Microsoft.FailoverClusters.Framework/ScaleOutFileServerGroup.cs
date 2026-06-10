using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class ScaleOutFileServerGroup : FileServerGroup
{
	public override CommandCollection ApplicationCommands
	{
		get
		{
			CommandCollection commandCollection = new CommandCollection(ClusterCommandCollectionId.ScaleOutFileServerGroup);
			InitializeApplicationCommands(commandCollection);
			return commandCollection;
		}
	}

	public override GroupType GroupType => GroupType.ScaleOutFileServer;

	protected override Icon2 GenerateIcon()
	{
		return new Icon2(InvariantResources.ScaleOutFileServerGroup);
	}

	protected override void InitializeGroupCommands(CommandCollection commandsCollection)
	{
		base.InitializeGroupCommands(commandsCollection);
		commandsCollection.RemoveAll((ClusterCommand command) => command.Id == ClusterCommandId.GroupAddStorage);
	}

	internal ScaleOutFileServerGroup(Cluster cluster)
		: base(cluster)
	{
	}
}

