using System.Collections.Generic;

namespace MS.Internal.ServerClusters.Management;

internal static class ContextFactory
{
	public static ClusterContext CreateClusterContext(string clusterName)
	{
		return new ClusterContext(clusterName);
	}

	public static ClusterContext CreateContext(Cluster cluster)
	{
		return new ClusterContext(cluster);
	}

	public static GroupContext CreateContext(ClusterGroup group, ClusterContext clusterContext)
	{
		return new GroupContext(group, clusterContext);
	}

	public static ResourceContext CreateContext(ClusterResource resource)
	{
		if (resource.IsStorage)
		{
			return new StorageResourceContext(resource);
		}
		return new ResourceContext(resource);
	}

	public static NodeContext CreateContext(ClusterNode node, ClusterContext clusterContext)
	{
		return new NodeContext(node, clusterContext);
	}

	public static NetworkContext CreateContext(ClusterNetwork network, ClusterContext clusterContext)
	{
		return new NetworkContext(network, clusterContext);
	}

	public static NetworkInterfaceContext CreateContext(ClusterNetworkInterface networkInterface)
	{
		return new NetworkInterfaceContext(networkInterface);
	}

	public static WpfAllNetworksContext CreateWpfAllNetworksContext(ClusterContext clusterContext)
	{
		return new WpfAllNetworksContext(clusterContext);
	}

	public static WpfAllNodesContext CreateWpfAllNodesContext(ClusterContext clusterContext)
	{
		return new WpfAllNodesContext(clusterContext);
	}

	public static WpfClusterRolesContext CreateWpfRolesContext(ClusterContext clusterContext)
	{
		return new WpfClusterRolesContext(clusterContext);
	}

	public static WpfClusterDisksContext CreateWpfClusterDisksContext(ClusterContext clusterContext)
	{
		return new WpfClusterDisksContext(clusterContext);
	}

	public static WpfClusterPoolsContext CreateWpfClusterPoolsContext(ClusterContext clusterContext)
	{
		return new WpfClusterPoolsContext(clusterContext);
	}

	public static WpfClusterEnclosuresContext CreateWpfClusterEnclosuresContext(ClusterContext clusterContext)
	{
		return new WpfClusterEnclosuresContext(clusterContext);
	}

	public static RootContext CreateRootContext(ClusterAdministrator snapin, ICollection<string> commandLineClusterNames)
	{
		return RootContext.CreateInstance(snapin, commandLineClusterNames);
	}

	public static ClusterEventsContext CreateClusterEventsContext(Cluster cluster)
	{
		return new ClusterEventsContext(cluster);
	}

	public static ClusterEventsContext CreateClusterEventsContext(ClusterDatabase clusterDatabase)
	{
		return new ClusterEventsContext(clusterDatabase);
	}

	public static DownNodeContext CreateDownNodeContext(ClusterDatabaseNode node)
	{
		return new DownNodeContext(node);
	}
}
