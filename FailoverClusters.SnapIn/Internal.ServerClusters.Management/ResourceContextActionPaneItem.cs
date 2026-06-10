using System.Windows.Input;
using FailoverClusters.ClusterSnapIn;

namespace MS.Internal.ServerClusters.Management;

internal abstract class ResourceContextActionPaneItem : MmcActionPaneItem
{
	protected ResourceContext ResourceContext { get; set; }

	protected ResourceContextActionPaneItem(ICommand command, ResourceContext resourceContext)
		: base(command)
	{
		ResourceContext = resourceContext;
	}
}

