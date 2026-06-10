using System;
using System.Collections.Generic;
using FailoverClusters.UI.Common;
using FailoverClusters.UIFramework;

namespace KDDSL.ServerClusters.Management;

internal class ClusterEventsMonitorRegistry : IClusterEventsMonitorRegistry
{
	private readonly object clusterEventsMonitorDictionaryLock = new object();

	private readonly Dictionary<string, IClusterEventsMonitor> clusterEventsMonitorDictionary = new Dictionary<string, IClusterEventsMonitor>();

	public IClusterEventsMonitor this[string clusterCachedId]
	{
		get
		{
			Exceptions.ThrowIfNullOrEmpty(clusterCachedId, "clusterCachedId");
			lock (clusterEventsMonitorDictionaryLock)
			{
				if (clusterEventsMonitorDictionary.ContainsKey(clusterCachedId))
				{
					return clusterEventsMonitorDictionary[clusterCachedId];
				}
			}
			return null;
		}
	}

	public event EventHandler<ClusterEventsMonitorChangedEventArgs> ClusterEventsMonitorChanged;

	public void AddClusterEventsMonitor(IClusterEventsMonitor clusterEventsMonitor)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		Exceptions.ThrowIfNull((object)clusterEventsMonitor, "clusterEventsMonitor");
		bool flag = false;
		lock (clusterEventsMonitorDictionaryLock)
		{
			if (!clusterEventsMonitorDictionary.ContainsKey(clusterEventsMonitor.ClusterCachedId.ToString()))
			{
				clusterEventsMonitorDictionary.Add(clusterEventsMonitor.ClusterCachedId.ToString(), clusterEventsMonitor);
				flag = true;
			}
		}
		if (flag && this.ClusterEventsMonitorChanged != null)
		{
			this.ClusterEventsMonitorChanged(this, new ClusterEventsMonitorChangedEventArgs(clusterEventsMonitor));
		}
	}

	public void RemoveClusterEventsMonitor(string clusterCachedId)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		Exceptions.ThrowIfNullOrEmpty(clusterCachedId, "clusterCachedId");
		bool flag = false;
		lock (clusterEventsMonitorDictionaryLock)
		{
			if (clusterEventsMonitorDictionary.ContainsKey(clusterCachedId))
			{
				clusterEventsMonitorDictionary.Remove(clusterCachedId);
				flag = true;
			}
		}
		if (flag && this.ClusterEventsMonitorChanged != null)
		{
			this.ClusterEventsMonitorChanged(this, new ClusterEventsMonitorChangedEventArgs(clusterCachedId));
		}
	}

	public IDictionary<string, IClusterEventsMonitor> GetAll()
	{
		IDictionary<string, IClusterEventsMonitor> dictionary = new Dictionary<string, IClusterEventsMonitor>();
		lock (clusterEventsMonitorDictionaryLock)
		{
			foreach (KeyValuePair<string, IClusterEventsMonitor> item in clusterEventsMonitorDictionary)
			{
				dictionary.Add(item);
			}
			return dictionary;
		}
	}
}

