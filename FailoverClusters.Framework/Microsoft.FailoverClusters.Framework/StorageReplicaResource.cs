using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class StorageReplicaResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.PhysicalDisk));

	internal StorageReplicaResource(Cluster cluster)
		: base(cluster)
	{
	}
}

