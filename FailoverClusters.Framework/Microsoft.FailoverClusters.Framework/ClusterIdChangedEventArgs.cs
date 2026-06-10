using System;

namespace FailoverClusters.Framework;

public class ClusterIdChangedEventArgs : ClusterEventArgs
{
	public Guid NewId { get; internal set; }

	public ClusterIdChangedEventArgs(Guid id, Guid newId, ClusterException exception)
		: base(id, exception)
	{
		NewId = newId;
	}
}

