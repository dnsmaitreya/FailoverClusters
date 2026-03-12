using System.Windows.Input;
using Microsoft.FailoverClusters.ClusterSnapIn;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.FileServer.Management.ServerManagerProxy;
using Microsoft.ManagementConsole;

namespace Microsoft.FailoverClusters.SnapIn;

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
