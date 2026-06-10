using System.Windows.Input;
using FailoverClusters.ClusterSnapIn;
using FailoverClusters.UI.Common;
using ManagementConsole;

namespace KDDSL.ServerClusters.Management;

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

