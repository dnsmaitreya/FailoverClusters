using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class DfsNamespaceResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.DfsNamespaceResource));

	internal DfsNamespaceResource(Cluster cluster)
		: base(cluster)
	{
	}
}
