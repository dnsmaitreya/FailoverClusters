using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class HyperVClusterWmiResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.HyperVClusterWMI));

	internal HyperVClusterWmiResource(Cluster cluster)
		: base(cluster)
	{
	}
}
