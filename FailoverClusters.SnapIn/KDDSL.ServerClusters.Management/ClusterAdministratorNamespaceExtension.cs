using ManagementConsole;
using ManagementConsole.Advanced;

namespace KDDSL.ServerClusters.Management;

[SnapInSettings("{A83E0714-53DF-4f3e-824B-67FF54048B53}", Description = "Failover Cluster Manager", DisplayName = "Failover Cluster Manager", Vendor = "Corporation", UseCustomHelp = true)]
[ExtendsNodeType("{1E5F3C57-CA68-4194-AE4B-45B4EA171E37}")]
public class ClusterAdministratorNamespaceExtension : NamespaceExtension
{
	private ClusterAdministrator cluAdmin;

	private void DisplayCustomHelp(object scopeNodeObject)
	{
		ClusterSnapInBase.DisplayCustomHelp(scopeNodeObject as ScopeNode);
	}

	public ClusterAdministratorNamespaceExtension()
	{
		cluAdmin = new ClusterAdministrator(this, ClusterSnapInType.FailoverClusterManager);
	}

	protected override void OnInitialize()
	{
		cluAdmin.Initialize();
		SnapInCallbackService.RegisterSnapInHelpTopicCallback(this, DisplayCustomHelp);
	}

	protected override void OnLoadCustomData(AsyncStatus status, byte[] persistenceData)
	{
		cluAdmin.LoadCustomData(status, persistenceData);
	}

	protected override byte[] OnSaveCustomData(SyncStatus status)
	{
		return cluAdmin.SaveCustomData(status);
	}

	protected override void OnShutdown(AsyncStatus status)
	{
		cluAdmin.OnShutdown(status);
	}
}

