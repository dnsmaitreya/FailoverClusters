using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterLoadedEventArgs : ClusterEventArgs
{
	public bool Loaded { get; internal set; }

	public int LoadSelection { get; internal set; }

	public ClusterObject Sender { get; internal set; }

	public ClusterLoadedEventArgs(Guid id, bool loaded, int loadSelection, ClusterException exception)
		: base(id, exception)
	{
		Loaded = loaded;
		LoadSelection = loadSelection;
	}

	public ClusterLoadedEventArgs(ClusterObject clusterObject, bool loaded, int loadSelection, ClusterException exception)
		: base((clusterObject == null) ? Guid.Empty : clusterObject.Id, exception)
	{
		Sender = clusterObject;
		Loaded = loaded;
		LoadSelection = loadSelection;
	}
}
