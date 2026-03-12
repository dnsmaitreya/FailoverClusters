using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class TaskSchedulerGroup : AverageGroup
{
	public override GroupType GroupType => GroupType.TaskScheduler;

	protected override Icon2 GenerateIcon()
	{
		return new Icon2(InvariantResources.Group);
	}

	internal TaskSchedulerGroup(Cluster cluster)
		: base(cluster)
	{
	}
}
