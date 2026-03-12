using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class VMReplicaBrokerResource : AverageResource
{
	private readonly VMReplicaBrokerSettingsCommand replicaBrokerSettingsCommand;

	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.VmReplicaBrokerResource));

	internal VMReplicaBrokerResource(Cluster cluster)
		: base(cluster)
	{
		replicaBrokerSettingsCommand = new VMReplicaBrokerSettingsCommand(this);
	}

	protected override void InitializeStateCommands(CommandCollection commandsCollection)
	{
		base.InitializeStateCommands(commandsCollection);
		commandsCollection.Add(replicaBrokerSettingsCommand.GetInstance());
	}
}
