using System;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class ClusterObjectLoaderParam
{
	public readonly ClusterObject ClusterObject;

	public readonly int LoadSelection;

	public readonly Action<ClusterLoadedEventArgs> BackgroundCallback;

	public ClusterObjectLoaderParam(ClusterObject clusterObject, Action<ClusterLoadedEventArgs> backgroundCallback, int loadSelection)
	{
		ClusterObject = clusterObject;
		LoadSelection = loadSelection;
		BackgroundCallback = backgroundCallback;
	}
}
