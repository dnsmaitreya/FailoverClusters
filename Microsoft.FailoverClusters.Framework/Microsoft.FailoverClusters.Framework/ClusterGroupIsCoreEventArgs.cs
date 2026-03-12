using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterGroupIsCoreEventArgs : ClusterEventArgs
{
	public bool? IsCore { get; internal set; }

	public ClusterGroupIsCoreEventArgs(Guid id, bool? isCore, ClusterException exception)
		: base(id, exception)
	{
		IsCore = isCore;
	}
}
