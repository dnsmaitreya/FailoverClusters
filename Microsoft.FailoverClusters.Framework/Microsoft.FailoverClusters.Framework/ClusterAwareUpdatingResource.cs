using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterAwareUpdatingResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.Resource));

	internal ClusterAwareUpdatingResource(Cluster cluster)
		: base(cluster)
	{
	}
}
