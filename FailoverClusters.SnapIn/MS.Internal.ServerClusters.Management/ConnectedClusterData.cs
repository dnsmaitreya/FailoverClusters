using System;
using System.Collections.Generic;

namespace MS.Internal.ServerClusters.Management;

[Serializable]
internal class ConnectedClusterData
{
	public static readonly int UNREACHABLE_COUNTER_DEFAULT = 5;

	private string clusterName;

	private List<string> nodeNames;

	private bool connected;

	private int unreachableCounter = UNREACHABLE_COUNTER_DEFAULT;

	public string ClusterName => clusterName;

	public bool Connected
	{
		get
		{
			return connected;
		}
		set
		{
			connected = value;
		}
	}

	public ICollection<string> NodeNames => nodeNames;

	public int UnreachableCounter
	{
		get
		{
			return unreachableCounter;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			unreachableCounter = value;
		}
	}

	private ConnectedClusterData(string name)
	{
		clusterName = name;
		nodeNames = new List<string>();
	}

	private ConnectedClusterData(Cluster cluster)
		: this(cluster.FqdnName)
	{
		nodeNames.AddRange(ClusterUtilities.GetNodeNamesFromCluster(cluster));
		cluster.NodesChanged += cluster_NodesChanged;
	}

	private ConnectedClusterData(string clusterName, ICollection<string> nodeNames)
		: this(clusterName)
	{
		this.nodeNames.AddRange(nodeNames);
	}

	private void cluster_NodesChanged(object sender, ClusterObjectEventArgs e)
	{
		if (e.EventType == ClusterObjectEventType.Added)
		{
			nodeNames.Add(e.ClusterObject);
		}
		else
		{
			nodeNames.Remove(e.ClusterObject);
		}
	}

	public static ConnectedClusterData CreateRunningClusterData(Cluster cluster)
	{
		return new ConnectedClusterData(cluster)
		{
			Connected = true
		};
	}

	public static ConnectedClusterData CreateDownClusterData(ClusterDatabase clusterDb)
	{
		return new ConnectedClusterData(clusterDb.FqdnClusterName, clusterDb.NodeNames)
		{
			Connected = true
		};
	}

	public static ConnectedClusterData CreateNameData(string name)
	{
		return CreateNameData(name, new List<string>());
	}

	public static ConnectedClusterData CreateNameData(string clusterName, ICollection<string> nodeNames)
	{
		if (clusterName == ".")
		{
			clusterName = Environment.MachineName;
		}
		return new ConnectedClusterData(clusterName, nodeNames);
	}

	public override string ToString()
	{
		return clusterName;
	}
}
