using System;
using System.Diagnostics;

namespace FailoverClusters.Framework;

public class ClusterGroupOpenEventArgs : EventArgs
{
	public Guid Id { get; internal set; }

	public bool IsOpen { get; internal set; }

	public ClusterException Error { get; internal set; }

	[DebuggerNonUserCode]
	public ClusterGroupOpenEventArgs(Guid id, bool opened, ClusterException ex)
	{
		Id = id;
		IsOpen = opened;
		Error = ex;
	}
}

