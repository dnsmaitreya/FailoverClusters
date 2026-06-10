using System.Windows.Input;
using FailoverClusters.ClusterSnapIn;
using ManagementConsole;

namespace KDDSL.ServerClusters.Management;

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

