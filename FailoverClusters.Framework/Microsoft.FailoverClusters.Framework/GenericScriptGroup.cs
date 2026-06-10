using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

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

