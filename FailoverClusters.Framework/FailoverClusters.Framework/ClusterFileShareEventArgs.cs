using System;

namespace FailoverClusters.Framework;

public class ClusterFileShareEventArgs : ClusterEventArgs
{
	public FileShare Share { get; internal set; }

	public CollectionElementAction Action { get; internal set; }

	public ClusterFileShareEventArgs(FileShare share, CollectionElementAction action)
		: base(Guid.Empty, null)
	{
		Share = share;
		Action = action;
	}
}

