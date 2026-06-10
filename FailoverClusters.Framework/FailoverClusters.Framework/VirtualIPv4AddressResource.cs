using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class VirtualIPv4AddressResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.IPAddressResource));

	internal VirtualIPv4AddressResource(Cluster cluster)
		: base(cluster)
	{
	}
}

