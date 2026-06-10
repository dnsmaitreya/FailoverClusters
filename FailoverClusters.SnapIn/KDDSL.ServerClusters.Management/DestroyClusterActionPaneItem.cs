using System.Windows.Input;
using FailoverClusters.ClusterSnapIn;
using ManagementConsole;

namespace KDDSL.ServerClusters.Management;

internal class DestroyClusterActionPaneItem : MmcActionPaneItem
{
	private ClusterContext ClusterContext { get; set; }

	public DestroyClusterActionPaneItem(ICommand command, ClusterContext clusterContext)
		: base(command)
	{
		ClusterContext = clusterContext;
	}

	protected override void OnActionTriggered(object sender, ActionEventArgs e)
	{
		ClusterContext.DestoryCluster(sender, e);
	}
}

