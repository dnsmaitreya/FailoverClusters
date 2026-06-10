using System;
using System.Diagnostics;
using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class ClusterRemovedEventArgs : ClusterEventArgs
{
	public string Name { get; internal set; }

	internal PCluster Cluster { get; set; }

	[DebuggerNonUserCode]
	public ClusterRemovedEventArgs(Guid id, string name, ClusterException exception)
		: base(id, exception)
	{
		Name = name;
	}
}

