using System.Windows.Input;
using Microsoft.FailoverClusters.ClusterSnapIn;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal class EvictNodeActionPaneItem : MmcActionPaneItem
{
	private NodeContext NodeContext { get; set; }

	public EvictNodeActionPaneItem(ICommand command, NodeContext nodeContext)
		: base(command)
	{
		Exceptions.ThrowIfNull((object)nodeContext, "nodeContext");
		NodeContext = nodeContext;
	}

	protected override void OnActionTriggered(object sender, ActionEventArgs e)
	{
		NodeContext.Evict(sender, e);
	}
}
