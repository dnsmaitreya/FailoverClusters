using System;

namespace FailoverClusters.Framework;

public class ClusterResourceFlagsEventArgs : ClusterEventArgs
{
	public ResourceFlags? Flags { get; internal set; }

	public ClusterResourceFlagsEventArgs(Guid id, ResourceFlags? newFlags, ClusterException exception)
		: base(id, exception)
	{
		Flags = newFlags;
	}
}

