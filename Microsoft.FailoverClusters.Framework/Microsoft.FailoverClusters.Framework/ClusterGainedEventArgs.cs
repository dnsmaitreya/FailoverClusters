using System;
using System.Diagnostics;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterGainedEventArgs : ClusterEventArgs
{
	public string Name { get; internal set; }

	public Guid GainedId { get; internal set; }

	[DebuggerNonUserCode]
	public ClusterGainedEventArgs(Guid id, string name, Guid gainedId)
		: this(id, name, gainedId, null)
	{
	}

	[DebuggerNonUserCode]
	public ClusterGainedEventArgs(Guid id, string name, Guid gainedId, ClusterException exception)
		: base(id, exception)
	{
		Name = name;
		GainedId = gainedId;
	}
}
