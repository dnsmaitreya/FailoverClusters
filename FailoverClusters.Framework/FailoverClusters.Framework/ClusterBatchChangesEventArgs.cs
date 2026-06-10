using System;
using System.Collections.Generic;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

internal class ClusterBatchChangesEventArgs : ClusterEventArgs
{
	public List<ClusterWrapperEventArgs> Changes { get; internal set; }

	public ClusterBatchChangesEventArgs(Guid id, List<ClusterWrapperEventArgs> changes)
		: base(id, null)
	{
		Changes = changes;
	}
}

