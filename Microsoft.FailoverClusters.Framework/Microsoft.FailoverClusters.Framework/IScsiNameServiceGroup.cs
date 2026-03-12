using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class IScsiNameServiceGroup : AverageGroup
{
	public override GroupType GroupType => GroupType.IScsiNameService;

	protected override Icon2 GenerateIcon()
	{
		return new Icon2(InvariantResources.IScsiNameServiceGroup);
	}

	internal IScsiNameServiceGroup(Cluster cluster)
		: base(cluster)
	{
	}
}
