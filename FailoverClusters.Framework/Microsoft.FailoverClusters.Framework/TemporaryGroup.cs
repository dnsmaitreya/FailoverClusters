using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class TemporaryGroup : AverageGroup
{
	public override GroupType GroupType => GroupType.Temporary;

	protected override Icon2 GenerateIcon()
	{
		return new Icon2(InvariantResources.Group);
	}

	internal TemporaryGroup(Cluster cluster)
		: base(cluster)
	{
	}
}

