using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class GenericApplicationGroup : AverageGroup
{
	public override GroupType GroupType => GroupType.GenericApplication;

	protected override Icon2 GenerateIcon()
	{
		return new Icon2(InvariantResources.GenericApplicationGroup);
	}

	internal GenericApplicationGroup(Cluster cluster)
		: base(cluster)
	{
	}
}
