using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;
using FailoverClusters.UI.Common;

namespace KDDSL.ServerClusters.Management;

[Serializable]
internal class SnapInSettings : ISerializable
{
	private readonly List<ConnectedClusterData> clusterConnectionHistory = new List<ConnectedClusterData>();

	private const string ClusterConnectionHistoryName = "clusterConnectionHistory";

	private const int ClusterMruCount = 10;

	private readonly object lockObject = new object();

	internal ICollection<string> ClusterMRU
	{
		get
		{
			List<string> list = new List<string>();
			int num = 0;
			foreach (ConnectedClusterData connectedCluster in ConnectedClusters)
			{
				list.Add(connectedCluster.ToString());
				if (++num >= 10)
				{
					break;
				}
			}
			return list.AsReadOnly();
		}
	}

	internal ICollection<ConnectedClusterData> ConnectedClusters => clusterConnectionHistory;

	internal event EventHandler SettingsChanged;

	internal SnapInSettings()
	{
	}

	internal SnapInSettings(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		try
		{
			clusterConnectionHistory = (List<ConnectedClusterData>)info.GetValue("clusterConnectionHistory", typeof(List<ConnectedClusterData>));
		}
		catch (SerializationException caughtException)
		{
			ExceptionHelp.LogException(caughtException, "last connected clusters is missing");
			clusterConnectionHistory = new List<ConnectedClusterData>();
		}
		catch (InvalidCastException caughtException2)
		{
			ExceptionHelp.LogException(caughtException2, "last connected clusters is wrong type");
			clusterConnectionHistory = new List<ConnectedClusterData>();
		}
	}

	internal void ClearSnapinSettings()
	{
		lock (lockObject)
		{
			clusterConnectionHistory.Clear();
		}
	}

	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		info.AddValue("clusterConnectionHistory", clusterConnectionHistory, typeof(List<ConnectedClusterData>));
	}

	internal void AddConnectedCluster(ConnectedClusterData connectionData)
	{
		lock (lockObject)
		{
			for (int num = clusterConnectionHistory.Count - 1; num >= 0; num--)
			{
				ConnectedClusterData connectedClusterData = clusterConnectionHistory[num];
				if (connectionData.ClusterName.Equals(connectedClusterData.ClusterName, StringComparison.CurrentCultureIgnoreCase))
				{
					try
					{
						clusterConnectionHistory.RemoveAt(num);
					}
					catch (ArgumentOutOfRangeException caughtException)
					{
						ExceptionHelp.LogException(caughtException, string.Format(CultureInfo.CurrentCulture, "Error removing cluster '{0}' from Cluadmin settings", connectionData.ClusterName));
					}
				}
			}
			clusterConnectionHistory.Insert(0, connectionData);
		}
		OnSettingsChanged();
	}

	internal void AddConnectedCluster(Cluster cluster)
	{
		if (cluster == null)
		{
			throw new ArgumentNullException("cluster");
		}
		Background.QueueWorker((WaitCallback)AddLiveClusterWorkItem, (object)cluster);
	}

	private void AddLiveClusterWorkItem(object data)
	{
		Cluster cluster = (Cluster)data;
		try
		{
			ConnectedClusterData connectionData = ConnectedClusterData.CreateRunningClusterData(cluster);
			AddConnectedCluster(connectionData);
		}
		catch (Exception ex)
		{
			ClusterLog.LogException(ex, "Error adding {0} as a connected cluster", new object[1] { cluster.Name });
		}
	}

	internal void RemoveConnectedCluster(string clusterName)
	{
		foreach (ConnectedClusterData item in clusterConnectionHistory)
		{
			if (item.ToString().Equals(clusterName, StringComparison.CurrentCultureIgnoreCase))
			{
				item.Connected = false;
				OnSettingsChanged();
			}
		}
	}

	internal void DecrementConnectedCluster(string clusterName)
	{
		for (int num = clusterConnectionHistory.Count - 1; num >= 0; num--)
		{
			ConnectedClusterData connectedClusterData = clusterConnectionHistory[num];
			if (connectedClusterData.ToString().Equals(clusterName, StringComparison.CurrentCultureIgnoreCase))
			{
				if (--connectedClusterData.UnreachableCounter <= 0)
				{
					clusterConnectionHistory.RemoveAt(num);
				}
				OnSettingsChanged();
				break;
			}
		}
	}

	private void OnSettingsChanged()
	{
		if (this.SettingsChanged != null)
		{
			this.SettingsChanged(this, EventArgs.Empty);
		}
	}
}

