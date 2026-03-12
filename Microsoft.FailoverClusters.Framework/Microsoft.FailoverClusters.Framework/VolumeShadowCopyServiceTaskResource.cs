using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class VolumeShadowCopyServiceTaskResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.VolumeShadowCopyServiceTaskResource));

	internal VolumeShadowCopyServiceTaskResource(Cluster cluster)
		: base(cluster)
	{
	}
}
