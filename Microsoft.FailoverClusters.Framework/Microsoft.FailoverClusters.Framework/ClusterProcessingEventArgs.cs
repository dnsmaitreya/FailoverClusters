using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterProcessingEventArgs : ClusterEventArgs
{
	public bool IsProcessing { get; internal set; }

	public ClusterProcessingEventArgs(Guid id, bool processing)
		: base(id, null)
	{
		IsProcessing = processing;
	}
}
