using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class GenericApplicationResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.GenericApplicationResource));

	internal GenericApplicationResource(Cluster cluster)
		: base(cluster)
	{
	}
}
