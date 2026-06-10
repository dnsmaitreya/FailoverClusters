using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class TsSessionBrokerGroup : AverageGroup
{
	public override GroupType GroupType => GroupType.TsSessionBroker;

	protected override Icon2 GenerateIcon()
	{
		return new Icon2(InvariantResources.TsSessionBrokerGroup);
	}

	internal TsSessionBrokerGroup(Cluster cluster)
		: base(cluster)
	{
	}
}

