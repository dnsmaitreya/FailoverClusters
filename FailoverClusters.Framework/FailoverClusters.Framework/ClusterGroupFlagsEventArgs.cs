using System;

namespace FailoverClusters.Framework;

public class ClusterGroupFlagsEventArgs : ClusterEventArgs
{
	public GroupFlags? Flags { get; internal set; }

	public ClusterGroupFlagsEventArgs(Guid id, GroupFlags? newFlags, ClusterException exception)
		: base(id, exception)
	{
		Flags = newFlags;
	}
}

