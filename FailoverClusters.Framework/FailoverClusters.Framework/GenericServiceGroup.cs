using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class GenericServiceGroup : AverageGroup
{
	public override GroupType GroupType => GroupType.GenericService;

	protected override Icon2 GenerateIcon()
	{
		return new Icon2(InvariantResources.GenericServiceGroup);
	}

	internal GenericServiceGroup(Cluster cluster)
		: base(cluster)
	{
	}
}

