using System;

namespace FailoverClusters.Framework;

public class ClusterStoragePoolInfoChangedEventArgs : ClusterEventArgs
{
	public PoolInfo PoolInfo { get; private set; }

	public ClusterStoragePoolInfoChangedEventArgs(Guid id, PoolInfo poolInfo)
		: base(id, null)
	{
		PoolInfo = poolInfo;
	}
}

