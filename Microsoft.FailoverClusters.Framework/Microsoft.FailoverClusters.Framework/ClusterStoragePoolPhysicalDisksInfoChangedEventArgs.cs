using System;
using System.Collections.Generic;

namespace Microsoft.FailoverClusters.Framework;

internal class ClusterStoragePoolPhysicalDisksInfoChangedEventArgs : ClusterEventArgs
{
	public IEnumerable<PoolPhysicalDiskInfoInternal> PoolPhysicalDisks { get; private set; }

	public ClusterStoragePoolPhysicalDisksInfoChangedEventArgs(Guid id, IEnumerable<PoolPhysicalDiskInfoInternal> poolPhysicalDisks)
		: base(id, null)
	{
		PoolPhysicalDisks = poolPhysicalDisks;
	}

	public ClusterStoragePoolPhysicalDisksInfoChangedEventArgs(Guid id, ClusterException exception)
		: base(id, exception)
	{
		PoolPhysicalDisks = new List<PoolPhysicalDiskInfoInternal>();
	}
}
