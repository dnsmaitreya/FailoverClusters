using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterAwareUpdatingGroup : AverageGroup
{
	public override GroupType GroupType => GroupType.ClusterAwareUpdating;

	protected override Icon2 GenerateIcon()
	{
		return new Icon2(InvariantResources.Group);
	}

	internal ClusterAwareUpdatingGroup(Cluster cluster)
		: base(cluster)
	{
	}
}
