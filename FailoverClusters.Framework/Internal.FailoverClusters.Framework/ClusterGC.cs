using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace MS.Internal.FailoverClusters.Framework;

internal class ClusterGC
{
	private const int CollectLapTime = 30000;

	private readonly PCluster cluster;

	private readonly AutoResetEvent resetEvent = new AutoResetEvent(initialState: false);

	private bool unloadSweeper;

	private readonly ConcurrentDictionary<Action<object>, object> registeredCollectCallbacks = new ConcurrentDictionary<Action<object>, object>();

	public ClusterGC(PCluster cluster)
	{
		ClusterGC clusterGC = this;
		if (cluster == null)
		{
			throw new ArgumentNullException("cluster");
		}
		this.cluster = cluster;
		Worker.Start(delegate
		{
			clusterGC.SweeperStart(cluster.Name);
		});
	}

	~ClusterGC()
	{
		Unload();
	}

	public void Collect()
	{
		if (!unloadSweeper)
		{
			resetEvent.Set();
		}
	}

	public void Unload()
	{
		if (!unloadSweeper)
		{
			unloadSweeper = true;
			resetEvent.Set();
			resetEvent.Dispose();
		}
	}

	public void RegisterCollect(Action<object> collectFx, object parameter)
	{
		registeredCollectCallbacks.TryAdd(collectFx, parameter);
	}

	private void SweeperStart(string clusterName)
	{
		Thread.CurrentThread.Priority = ThreadPriority.Lowest;
		Thread.CurrentThread.Name = "ClusterGC Thread '{0}'".FormatCurrentCulture(clusterName);
		while (!unloadSweeper)
		{
			resetEvent.WaitOne(30000);
			if (unloadSweeper)
			{
				cluster.RealtimeCollections.Collect(cluster);
				WeakReferenceEx.Collect();
				break;
			}
			try
			{
				cluster.CacheManager.Collect();
				cluster.RealtimeCollections.Collect();
				cluster.Server.Collect();
				cluster.Virtualization.Collect();
				WeakReferenceEx.Collect();
				foreach (KeyValuePair<Action<object>, object> registeredCollectCallback in registeredCollectCallbacks)
				{
					registeredCollectCallback.Key(registeredCollectCallback.Value);
				}
			}
			catch (Exception ex)
			{
				Unload();
				ClusterLog.LogException(ex, "There was an error releasing memory");
				cluster.HandleFatalError(new ClusterGCException(ex));
			}
		}
	}
}

