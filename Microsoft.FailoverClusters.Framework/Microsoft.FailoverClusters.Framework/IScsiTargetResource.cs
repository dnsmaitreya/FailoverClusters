using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class IScsiTargetResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.IScsiTargetResource));

	internal IScsiTargetResource(Cluster cluster)
		: base(cluster)
	{
	}
}
