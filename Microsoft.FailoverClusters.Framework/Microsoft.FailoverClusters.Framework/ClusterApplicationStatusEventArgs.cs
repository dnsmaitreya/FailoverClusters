using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterApplicationStatusEventArgs : ClusterEventArgs
{
	public ClusterApplicationStatusEventArgs(Guid id, ClusterException exception)
		: base(id, exception)
	{
	}
}
