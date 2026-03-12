using System.Windows.Input;
using Microsoft.ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal class SimulateFailureActionPaneItem : ResourceContextActionPaneItem
{
	public SimulateFailureActionPaneItem(ICommand command, ResourceContext resourceContext)
		: base(command, resourceContext)
	{
	}

	protected override void OnActionTriggered(object sender, ActionEventArgs e)
	{
		SnapinActionEventArgs e2 = new SnapinActionEventArgs(e.Action, e.Status);
		base.ResourceContext.OnSimulateFailure(sender, e2);
	}
}
