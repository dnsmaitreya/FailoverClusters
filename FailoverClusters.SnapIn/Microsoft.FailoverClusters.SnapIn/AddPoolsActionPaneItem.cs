using System.Windows.Input;
using Microsoft.FailoverClusters.ClusterSnapIn;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.ManagementConsole;
using MS.Internal.ServerClusters.Management;

namespace Microsoft.FailoverClusters.SnapIn;

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
