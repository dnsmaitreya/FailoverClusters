using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class HyperVNetworkVirtualizationPAResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.IPAddressResource));

	internal HyperVNetworkVirtualizationPAResource(Cluster cluster)
		: base(cluster)
	{
	}
}
