using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class WinsServiceResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.WinsServiceResource));

	internal WinsServiceResource(Cluster cluster)
		: base(cluster)
	{
	}
}
