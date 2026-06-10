using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class DtcResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.DtcResource));

	internal DtcResource(Cluster cluster)
		: base(cluster)
	{
	}
}

