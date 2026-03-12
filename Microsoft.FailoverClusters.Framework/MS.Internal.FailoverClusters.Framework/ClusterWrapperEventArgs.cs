using System;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class ClusterWrapperEventArgs : EventArgs
{
	private readonly EventType eventType;

	private readonly ClusterEventArgs eventArgs;

	public EventType EventType => eventType;

	public ClusterEventArgs EventArgument => eventArgs;

	public ClusterWrapperEventArgs(EventType eventType, ClusterEventArgs eventArgs)
	{
		this.eventType = eventType;
		this.eventArgs = eventArgs;
	}
}
