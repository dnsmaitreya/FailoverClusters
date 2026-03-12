using System;
using System.Diagnostics;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterLostEventArgs : ClusterEventArgs
{
	public string Name { get; internal set; }

	public Guid LostId { get; internal set; }

	[DebuggerNonUserCode]
	public ClusterLostEventArgs(Guid id, string name, Guid lostId)
		: this(id, name, lostId, null)
	{
	}

	[DebuggerNonUserCode]
	public ClusterLostEventArgs(Guid id, string name, Guid lostId, ClusterException exception)
		: base(id, exception)
	{
		Name = name;
		LostId = lostId;
	}
}
