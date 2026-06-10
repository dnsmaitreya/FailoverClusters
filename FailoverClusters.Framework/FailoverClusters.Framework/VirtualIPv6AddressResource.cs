using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class VirtualIPv6AddressResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.IPAddressResource));

	internal VirtualIPv6AddressResource(Cluster cluster)
		: base(cluster)
	{
	}
}

