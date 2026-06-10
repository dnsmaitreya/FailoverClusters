using System.Windows.Input;
using FailoverClusters.ClusterSnapIn;

namespace KDDSL.ServerClusters.Management;

internal abstract class ResourceContextActionPaneItem : MmcActionPaneItem
{
	protected ResourceContext ResourceContext { get; set; }

	protected ResourceContextActionPaneItem(ICommand command, ResourceContext resourceContext)
		: base(command)
	{
		ResourceContext = resourceContext;
	}
}

