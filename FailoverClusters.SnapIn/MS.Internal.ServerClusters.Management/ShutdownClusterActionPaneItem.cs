using System.Windows.Input;
using FailoverClusters.ClusterSnapIn;
using ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal class ShutdownClusterActionPaneItem : MmcActionPaneItem
{
	private ClusterContext ClusterContext { get; set; }

	public ShutdownClusterActionPaneItem(ICommand command, ClusterContext clusterContext)
		: base(command)
	{
		ClusterContext = clusterContext;
	}

	protected override void OnActionTriggered(object sender, ActionEventArgs e)
	{
		ClusterContext.ShutdownCluster(sender, e);
	}
}

