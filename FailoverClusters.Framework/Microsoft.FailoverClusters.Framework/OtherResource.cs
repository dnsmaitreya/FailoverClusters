using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class OtherResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.Resource));

	internal OtherResource(Cluster cluster)
		: base(cluster)
	{
	}
}

