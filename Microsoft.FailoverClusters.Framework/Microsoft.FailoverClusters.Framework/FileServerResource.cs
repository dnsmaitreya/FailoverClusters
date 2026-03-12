using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class FileServerResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.FileServerResource));

	internal FileServerResource(Cluster cluster)
		: base(cluster)
	{
	}
}
