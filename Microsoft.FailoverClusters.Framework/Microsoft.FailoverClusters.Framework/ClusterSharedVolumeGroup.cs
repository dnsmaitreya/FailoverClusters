using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterSharedVolumeGroup : AverageGroup
{
	public override string DisplayName => EnumResources.ClusterSharedVolume;

	public override GroupType GroupType => GroupType.ClusterSharedVolume;

	protected override Icon2 GenerateIcon()
	{
		return new Icon2(InvariantResources.ClusterSharedVolumeGroup);
	}

	internal ClusterSharedVolumeGroup(Cluster cluster)
		: base(cluster)
	{
	}
}
