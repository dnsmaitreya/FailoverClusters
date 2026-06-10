using System;
using System.Collections.Generic;

namespace FailoverClusters.Framework;

public class ClusterResourceTypePossibleOwnersChangedEventArgs : ClusterEventArgs
{
	public List<Guid> PossibleNodes { get; internal set; }

	public ClusterResourceTypePossibleOwnersChangedEventArgs(Guid id, List<Guid> possibleNodes, ClusterException exception)
		: base(id, exception)
	{
		PossibleNodes = possibleNodes;
	}
}

