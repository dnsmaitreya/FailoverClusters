using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class GenericScriptResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.GenericScriptResource));

	internal GenericScriptResource(Cluster cluster)
		: base(cluster)
	{
	}
}

