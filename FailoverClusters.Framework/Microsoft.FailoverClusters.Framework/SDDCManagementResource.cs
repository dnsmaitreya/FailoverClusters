using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class SDDCManagementResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.HealthService));

	internal SDDCManagementResource(Cluster cluster)
		: base(cluster)
	{
	}
}

