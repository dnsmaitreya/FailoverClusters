using System.Diagnostics;
using ManagementConsole;

namespace KDDSL.ServerClusters.Management;

public class ClusterSnapInBase : SnapIn
{
	private readonly ClusterAdministrator cluAdmin;

	protected ClusterSnapInBase(ClusterSnapInType snapInType)
	{
		cluAdmin = new ClusterAdministrator(this, snapInType);
	}

	internal static void DisplayCustomHelp(ScopeNode scopeNode)
	{
	}

	protected void DisplayCustomHelp(object scopeNodeObject)
	{
		Process.Start(((ScopeNode)scopeNodeObject).HelpTopic);
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

