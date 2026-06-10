using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class IScsiNameServiceResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.IScsiNameServiceResource));

	internal IScsiNameServiceResource(Cluster cluster)
		: base(cluster)
	{
	}
}

