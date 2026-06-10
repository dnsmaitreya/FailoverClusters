using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class GenericServiceResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.GenericScriptResource));

	internal GenericServiceResource(Cluster cluster)
		: base(cluster)
	{
	}
}

