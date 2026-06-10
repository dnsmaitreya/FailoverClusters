using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class NfsShareResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.NfsShareResource));

	internal NfsShareResource(Cluster cluster)
		: base(cluster)
	{
	}
}

