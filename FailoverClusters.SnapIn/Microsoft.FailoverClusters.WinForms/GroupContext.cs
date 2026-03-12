using System;
using Microsoft.FailoverClusters.Framework;
using Microsoft.ManagementConsole;
using MS.Internal.ServerClusters.Management;

namespace Microsoft.FailoverClusters.WinForms;

public static class GroupContext
{
	public static Guid[] GetNodeTypes()
	{
		return new Guid[1] { ClusterAdministrator.GroupContextGuid };
	}

	public static WritableSharedData GetSharedData(Group group)
	{
		return new WritableSharedData();
	}

	public static int GetPropertyPages(PropertyPageCollection pages, GroupType type, Cluster cluster, Guid groupId)
	{
		if (pages == null)
		{
			throw new ArgumentNullException("pages");
		}
		try
		{
			ClusterSnapinPropertyPage clusterSnapinPropertyPage = new ClusterSnapinPropertyPage();
			clusterSnapinPropertyPage.SetControl(new GroupGeneralPropertyPage(cluster, groupId));
			pages.Add(clusterSnapinPropertyPage);
			clusterSnapinPropertyPage = new ClusterSnapinPropertyPage();
			clusterSnapinPropertyPage.SetControl(new GroupFailoverPropertyPage(cluster, groupId));
			pages.Add(clusterSnapinPropertyPage);
		}
		catch (Exception e)
		{
			ErrorPropertyPage.CreateErrorPropertySheet(pages, e, Resources.Properties_Text);
		}
		return pages.Count;
	}
}
