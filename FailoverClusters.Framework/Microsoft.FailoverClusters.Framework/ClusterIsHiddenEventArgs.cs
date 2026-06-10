using System;

namespace FailoverClusters.Framework;

public class ClusterIsHiddenEventArgs : ClusterEventArgs
{
	public bool IsHidden { get; internal set; }

	public ClusterIsHiddenEventArgs(Guid id, bool isHidden, ClusterException exception)
		: base(id, exception)
	{
		IsHidden = isHidden;
	}
}

