using System;
using System.Diagnostics;

namespace FailoverClusters.Framework;

public class ClusterRefreshedEventArgs : ClusterEventArgs
{
	public string Name { get; internal set; }

	public bool Targeted { get; internal set; }

	[DebuggerNonUserCode]
	public ClusterRefreshedEventArgs(Guid id, string name, bool targeted)
		: base(id, null)
	{
		Name = name;
		Targeted = targeted;
	}
}

