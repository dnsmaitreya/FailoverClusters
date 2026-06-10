using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class StorageQoSResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.StorageQoS));

	internal StorageQoSResource(Cluster cluster)
		: base(cluster)
	{
	}
}

