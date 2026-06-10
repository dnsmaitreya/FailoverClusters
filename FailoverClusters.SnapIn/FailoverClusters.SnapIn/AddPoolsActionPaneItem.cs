using System.Windows.Input;
using FailoverClusters.ClusterSnapIn;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using ManagementConsole;
using KDDSL.ServerClusters.Management;

namespace FailoverClusters.SnapIn;

internal class AddPoolsActionPaneItem : MmcActionPaneItem
{
	private ClusterContext ClusterContext { get; set; }

	private Cluster FrameworkCluster { get; set; }

	public AddPoolsActionPaneItem(ICommand command, Cluster frameworkCluster, ClusterContext clusterContext)
		: base(command)
	{
		Exceptions.ThrowIfNull((object)clusterContext, "cluster");
		ClusterContext = clusterContext;
		FrameworkCluster = frameworkCluster;
	}

	protected override void OnActionTriggered(object sender, ActionEventArgs e)
	{
		new AddPoolsOperation(FrameworkCluster, ClusterContext.Cluster).Execute();
	}
}

