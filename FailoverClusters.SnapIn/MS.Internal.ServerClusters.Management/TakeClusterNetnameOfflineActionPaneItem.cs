using System.Windows.Input;
using Microsoft.ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal class TakeClusterNetnameOfflineActionPaneItem : ResourceContextActionPaneItem
{
	public TakeClusterNetnameOfflineActionPaneItem(ICommand command, ResourceContext resourceContext)
		: base(command, resourceContext)
	{
	}

	protected override void OnActionTriggered(object sender, ActionEventArgs e)
	{
		SnapinActionEventArgs e2 = new SnapinActionEventArgs(e.Action, e.Status);
		base.ResourceContext.OnTakeOffline(sender, e2);
	}
}
