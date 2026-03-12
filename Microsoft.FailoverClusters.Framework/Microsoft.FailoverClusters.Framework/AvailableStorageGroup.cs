using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class AvailableStorageGroup : AverageGroup
{
	public override GroupType GroupType => GroupType.AvailableStorage;

	protected override Icon2 GenerateIcon()
	{
		return new Icon2(InvariantResources.AvailableStorageGroup);
	}

	internal AvailableStorageGroup(Cluster cluster)
		: base(cluster)
	{
	}
}
