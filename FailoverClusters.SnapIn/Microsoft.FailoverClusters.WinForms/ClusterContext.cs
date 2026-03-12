using System;
using Microsoft.FailoverClusters.Framework;
using Microsoft.ManagementConsole;
using MS.Internal.ServerClusters.Management;

namespace Microsoft.FailoverClusters.WinForms;

public static class ClusterContext
{
	public static Guid[] GetNodeTypes()
	{
		return new Guid[1] { ClusterAdministrator.ClusterContextGuid };
	}

	public static WritableSharedData GetSharedData(Cluster cluster)
	{
		return new WritableSharedData();
	}

	public static int GetPropertyPages(PropertyPageCollection pages, Cluster cluster)
	{
		return 0;
	}
}
