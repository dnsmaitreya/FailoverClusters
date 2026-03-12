using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class FileServerGroup : AverageGroup
{
	public override GroupType GroupType => GroupType.FileServer;

	protected override Icon2 GenerateIcon()
	{
		return new Icon2(InvariantResources.FileServerGroup);
	}

	internal FileServerGroup(Cluster cluster)
		: base(cluster)
	{
	}
}
