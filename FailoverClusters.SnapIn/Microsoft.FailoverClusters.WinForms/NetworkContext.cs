using System;
using FailoverClusters.Framework;
using ManagementConsole;
using MS.Internal.ServerClusters.Management;

namespace FailoverClusters.WinForms;

public static class NetworkContext
{
	public static Guid[] GetNodeTypes()
	{
		return new Guid[1] { ClusterAdministrator.NetworkContextGuid };
	}

	public static WritableSharedData GetSharedData(Network network)
	{
		return new WritableSharedData();
	}

	public static int GetNetworkPropertyPages(PropertyPageCollection pages, Cluster cluster, Guid networkId)
	{
		if (pages == null)
		{
			throw new ArgumentNullException("pages");
		}
		ClusterSnapinPropertyPage clusterSnapinPropertyPage = new ClusterSnapinPropertyPage();
		clusterSnapinPropertyPage.SetControl(new NetworkGeneralPropertyPage(cluster, networkId));
		pages.Add(clusterSnapinPropertyPage);
		return pages.Count;
	}
}

