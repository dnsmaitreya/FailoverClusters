using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class GenericScriptGroup : AverageGroup
{
	public override GroupType GroupType => GroupType.GenericScript;

	protected override Icon2 GenerateIcon()
	{
		return new Icon2(InvariantResources.GenericScriptGroup);
	}

	internal GenericScriptGroup(Cluster cluster)
		: base(cluster)
	{
	}
}
