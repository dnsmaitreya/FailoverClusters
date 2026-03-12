using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class NetworkFileSystemResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.NetworkFileSystem));

	internal NetworkFileSystemResource(Cluster cluster)
		: base(cluster)
	{
	}
}
