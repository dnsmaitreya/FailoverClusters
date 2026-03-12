using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class DhcpServiceResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.DhcpServiceResource));

	internal DhcpServiceResource(Cluster cluster)
		: base(cluster)
	{
	}
}
