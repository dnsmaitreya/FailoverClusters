using System.Windows.Input;
using FailoverClusters.ClusterSnapIn;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using FileServer.Management.ServerManagerProxy;
using ManagementConsole;

namespace FailoverClusters.SnapIn;

public class CreatePoolsActionPaneItem : MmcActionPaneItem
{
	private Cluster Cluster { get; set; }

	public CreatePoolsActionPaneItem(ICommand command, Cluster cluster)
		: base(command)
	{
		Exceptions.ThrowIfNull((object)cluster, "cluster");
		Cluster = cluster;
	}

	protected override void OnActionTriggered(object sender, ActionEventArgs e)
	{
		ServerManagerProxy.StartStorageWizardAsync(StorageWizardType.PoolWizard, Cluster, delegate
		{
			((Action)base.Action).Enabled = false;
		}, delegate
		{
			((Action)base.Action).Enabled = true;
		});
	}
}

