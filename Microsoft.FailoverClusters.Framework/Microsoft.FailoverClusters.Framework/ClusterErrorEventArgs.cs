using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterErrorEventArgs : ClusterEventArgs
{
	public ClusterErrorEventArgs(Guid id, ClusterException exception)
		: base(id, exception)
	{
	}
}
