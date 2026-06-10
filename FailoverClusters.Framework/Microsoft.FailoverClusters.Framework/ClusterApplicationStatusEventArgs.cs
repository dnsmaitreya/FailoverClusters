using System;

namespace FailoverClusters.Framework;

public class ClusterApplicationStatusEventArgs : ClusterEventArgs
{
	public ClusterApplicationStatusEventArgs(Guid id, ClusterException exception)
		: base(id, exception)
	{
	}
}

