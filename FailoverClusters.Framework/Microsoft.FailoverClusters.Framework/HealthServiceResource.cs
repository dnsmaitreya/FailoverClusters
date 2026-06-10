using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class HealthServiceResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.HealthService));

	internal HealthServiceResource(Cluster cluster)
		: base(cluster)
	{
	}
}

