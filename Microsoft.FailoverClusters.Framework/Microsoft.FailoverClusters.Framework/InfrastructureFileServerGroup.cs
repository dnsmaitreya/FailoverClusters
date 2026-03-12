using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class InfrastructureFileServerGroup : FileServerGroup
{
	public override GroupType GroupType => GroupType.InfrastructureFileServer;

	public override CommandCollection ApplicationCommands
	{
		get
		{
			CommandCollection commandCollection = new CommandCollection(ClusterCommandCollectionId.InfrastructureFileServer);
			InitializeApplicationCommands(commandCollection);
			return commandCollection;
		}
	}

	protected override Icon2 GenerateIcon()
	{
		return new Icon2(InvariantResources.ScaleOutFileServerGroup);
	}

	protected override void InitializeGroupCommands(CommandCollection commandsCollection)
	{
		base.InitializeGroupCommands(commandsCollection);
		commandsCollection.RemoveAll((ClusterCommand command) => command.Id == ClusterCommandId.GroupAddStorage);
	}

	internal InfrastructureFileServerGroup(Cluster cluster)
		: base(cluster)
	{
	}
}
