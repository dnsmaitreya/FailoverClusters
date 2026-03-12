using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class VMReplicaBrokerGroup : AverageGroup
{
	public override GroupType GroupType => GroupType.VMReplicaBroker;

	public override CommandCollection ApplicationCommands
	{
		get
		{
			CommandCollection applicationCommands = base.ApplicationCommands;
			VMReplicaBrokerSettingsCommand vMReplicaBrokerSettingsCommand = new VMReplicaBrokerSettingsCommand(this);
			applicationCommands.Add(vMReplicaBrokerSettingsCommand.GetInstance());
			return applicationCommands;
		}
	}

	protected override Icon2 GenerateIcon()
	{
		return new Icon2(InvariantResources.VmReplicaBrokerGroup);
	}

	internal VMReplicaBrokerGroup(Cluster cluster)
		: base(cluster)
	{
	}
}
