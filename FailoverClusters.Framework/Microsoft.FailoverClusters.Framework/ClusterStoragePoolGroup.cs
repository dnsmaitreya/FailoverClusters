using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class ClusterStoragePoolGroup : AverageGroup
{
	public override GroupType GroupType => GroupType.ClusterStoragePool;

	protected override Icon2 GenerateIcon()
	{
		return new Icon2(InvariantResources.AvailableStorageGroup);
	}

	internal ClusterStoragePoolGroup(Cluster cluster)
		: base(cluster)
	{
	}
}

