using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class MsmqTriggerResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.MsmqTriggerResource));

	internal MsmqTriggerResource(Cluster cluster)
		: base(cluster)
	{
	}
}

