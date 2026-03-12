using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterEventArgs : EventArgs
{
	public virtual Guid Id { get; internal set; }

	public virtual ClusterException Error { get; internal set; }

	public ClusterEventArgs(Guid id, ClusterException exception)
	{
		Id = id;
		Error = exception;
	}
}
