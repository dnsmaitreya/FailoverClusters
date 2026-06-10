using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class IScsiTargetGroup : AverageGroup
{
	public override GroupType GroupType => GroupType.IScsiTarget;

	protected override Icon2 GenerateIcon()
	{
		return new Icon2(InvariantResources.iSCSITargetResourceGroup);
	}

	internal IScsiTargetGroup(Cluster cluster)
		: base(cluster)
	{
	}
}

