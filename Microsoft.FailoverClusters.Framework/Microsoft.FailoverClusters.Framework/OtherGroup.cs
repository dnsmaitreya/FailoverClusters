using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class OtherGroup : AverageGroup
{
	public override GroupType GroupType => GroupType.Unknown;

	protected override Icon2 GenerateIcon()
	{
		return new Icon2(InvariantResources.Group);
	}

	internal OtherGroup(Cluster cluster)
		: base(cluster)
	{
	}
}
