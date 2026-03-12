using System;
using System.Diagnostics;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterDisconnectedEventArgs : ClusterEventArgs
{
	[DebuggerNonUserCode]
	public ClusterDisconnectedEventArgs(Guid id, ClusterException exception)
		: base(id, exception)
	{
	}
}
