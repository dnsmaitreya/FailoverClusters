using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class TaskSchedulerResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.Resource));

	internal TaskSchedulerResource(Cluster cluster)
		: base(cluster)
	{
	}
}

