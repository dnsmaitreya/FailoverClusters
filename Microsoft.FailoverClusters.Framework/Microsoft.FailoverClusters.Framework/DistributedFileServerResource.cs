using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class DistributedFileServerResource : FileServerResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.DistributedFileServerResource));

	internal DistributedFileServerResource(Cluster cluster)
		: base(cluster)
	{
	}
}
