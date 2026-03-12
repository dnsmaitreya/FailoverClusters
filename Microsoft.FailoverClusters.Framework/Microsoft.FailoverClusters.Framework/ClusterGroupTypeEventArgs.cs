using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterGroupTypeEventArgs : ClusterEventArgs
{
	public GroupType GroupType { get; internal set; }

	public ClusterGroupTypeEventArgs(Guid id, GroupType groupType, ClusterException exception)
		: base(id, exception)
	{
		GroupType = groupType;
	}
}
