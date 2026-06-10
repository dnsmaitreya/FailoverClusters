using System;
using System.Diagnostics;

namespace FailoverClusters.Framework;

public class ClusterOpenEventArgs : ClusterEventArgs
{
	public bool IsOpen { get; internal set; }

	[DebuggerNonUserCode]
	public ClusterOpenEventArgs(Guid id, bool opened, ClusterException exception)
		: base(id, exception)
	{
		IsOpen = opened;
	}
}

