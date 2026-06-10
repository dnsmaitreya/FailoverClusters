using System;
using System.Collections.Generic;

namespace FailoverClusters.Framework;

public class ClusterGroupPreferredOwnersChangedEventArgs : ClusterEventArgs
{
	public List<Guid> PreferredNodes { get; internal set; }

	public ClusterGroupPreferredOwnersChangedEventArgs(Guid id, List<Guid> preferredNodes, ClusterException exception)
		: base(id, exception)
	{
		PreferredNodes = preferredNodes;
	}
}

