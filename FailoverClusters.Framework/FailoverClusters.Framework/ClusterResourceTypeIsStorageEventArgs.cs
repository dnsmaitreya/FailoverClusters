using System;

namespace FailoverClusters.Framework;

public class ClusterResourceTypeIsStorageEventArgs : ClusterEventArgs
{
	public bool? IsStorage { get; internal set; }

	public ClusterResourceTypeIsStorageEventArgs(Guid id, bool? isStorage, ClusterException exception)
		: base(id, exception)
	{
		IsStorage = isStorage;
	}
}

