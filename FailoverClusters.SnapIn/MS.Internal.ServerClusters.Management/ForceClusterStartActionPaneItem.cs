using System.Windows.Input;
using Microsoft.FailoverClusters.ClusterSnapIn;
using Microsoft.ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal class ForceClusterStartActionPaneItem : MmcActionPaneItem
{
	private DownClusterContext DownClusterContext { get; set; }

	internal bool Enabled
	{
		set
		{
			((ActionBase)base.Action).Enabled = value;
		}
	}

	public ForceClusterStartActionPaneItem(ICommand command, DownClusterContext downClusterContext)
		: base(command)
	{
		DownClusterContext = downClusterContext;
	}

	protected override void OnActionTriggered(object sender, ActionEventArgs e)
	{
		DownClusterContext.ForceClusterStart(sender, e);
	}
}
