using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class CrossClusterDependencyOrchestratorResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.HealthService));

	internal CrossClusterDependencyOrchestratorResource(Cluster cluster)
		: base(cluster)
	{
	}
}

