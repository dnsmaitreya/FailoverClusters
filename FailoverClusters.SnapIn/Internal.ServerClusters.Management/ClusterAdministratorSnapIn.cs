using ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

[SnapInSettings("{D2779945-405B-4ace-8618-508F3E3054AC}", Description = "Failover Cluster Manager", DisplayName = "Failover Cluster Manager", Vendor = "Corporation", UseCustomHelp = true)]
[SnapInAbout("failoverclusters.snapinsupport.dll", ApplicationBaseRelative = true, DisplayNameId = 101, DescriptionId = 102, VendorId = 103, VersionId = 104, IconId = 110, LargeFolderBitmapId = 111, SmallFolderBitmapId = 112, SmallFolderSelectedBitmapId = 112)]
[PublishesNodeType("{9E48D9FE-87FB-4285-A044-6547071FDEBF}")]
[PublishesNodeType("{258C9D33-ABB6-4c07-89E5-E4437C43030E}")]
[PublishesNodeType("{E665B79D-EFB2-4af8-A147-C9365BC6E7CE}")]
public class ClusterAdministratorSnapIn : ClusterSnapInBase
{
	public ClusterAdministratorSnapIn()
		: base(ClusterSnapInType.FailoverClusterManager)
	{
	}
}

