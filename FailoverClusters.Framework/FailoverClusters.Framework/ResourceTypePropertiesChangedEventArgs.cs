using System;
using System.Collections.Generic;

namespace FailoverClusters.Framework;

public class ResourceTypePropertiesChangedEventArgs : ClusterEventArgs
{
	public List<Guid> PossibleNodes { get; internal set; }

	public ResourceTypePropertiesChangedEventArgs(Guid id, List<Guid> possibleNodes, ClusterException exception)
		: base(id, exception)
	{
		PossibleNodes = possibleNodes;
	}
}

