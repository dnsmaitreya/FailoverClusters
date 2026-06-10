using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class CoreClusterGroup : AverageGroup
{
	public override GroupType GroupType => GroupType.CoreCluster;

	protected override Icon2 GenerateIcon()
	{
		return new Icon2(InvariantResources.CoreClusterGroup);
	}

	internal CoreClusterGroup(Cluster cluster)
		: base(cluster)
	{
	}
}

