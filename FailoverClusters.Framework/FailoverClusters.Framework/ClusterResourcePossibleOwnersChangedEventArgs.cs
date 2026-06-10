using System;
using System.Collections.Generic;

namespace FailoverClusters.Framework;

public class ClusterResourcePossibleOwnersChangedEventArgs : ClusterEventArgs
{
	public List<Guid> PossibleNodes { get; internal set; }

	public ClusterResourcePossibleOwnersChangedEventArgs(Guid id, List<Guid> possibleNodes, ClusterException exception)
		: base(id, exception)
	{
		PossibleNodes = possibleNodes;
	}
}

