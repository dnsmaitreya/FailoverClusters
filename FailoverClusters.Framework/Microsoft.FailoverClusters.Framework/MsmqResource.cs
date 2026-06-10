using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class MsmqResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.MsmqResource));

	internal MsmqResource(Cluster cluster)
		: base(cluster)
	{
	}
}

