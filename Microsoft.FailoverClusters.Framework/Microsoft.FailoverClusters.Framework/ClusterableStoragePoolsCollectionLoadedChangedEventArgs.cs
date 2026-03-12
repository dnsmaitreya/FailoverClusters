using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterableStoragePoolsCollectionLoadedChangedEventArgs : EventArgs
{
	public bool Loaded { get; private set; }

	public ClusterableStoragePoolsCollectionLoadedChangedEventArgs(bool isLoaded)
	{
		Loaded = isLoaded;
	}
}
