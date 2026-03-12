using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterStoragePoolInfoChangedEventArgs : ClusterEventArgs
{
	public PoolInfo PoolInfo { get; private set; }

	public ClusterStoragePoolInfoChangedEventArgs(Guid id, PoolInfo poolInfo)
		: base(id, null)
	{
		PoolInfo = poolInfo;
	}
}
