using System;

namespace FailoverClusters.Framework;

public class ClusterRenamedEventArgs : ClusterEventArgs
{
	public string NewName { get; internal set; }

	public ClusterRenamedEventArgs(Guid id, string newName, ClusterException exception)
		: base(id, exception)
	{
		NewName = newName;
	}
}

