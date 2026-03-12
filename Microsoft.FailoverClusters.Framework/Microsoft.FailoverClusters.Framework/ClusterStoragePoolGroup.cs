using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

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
