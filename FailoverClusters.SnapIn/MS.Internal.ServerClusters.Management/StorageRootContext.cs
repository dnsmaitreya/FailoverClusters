using System;
using FailoverClusters.UI.Common;
using ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal class StorageRootContext : ScopeNodeContextBase, IViewContext
{
	private readonly ClusterContext clusterContext;

	public override ActionsPaneItemCollection ActionsPaneItems => new ActionsPaneItemCollection();

	public override ViewDescriptionCollection ViewDescriptions => new ViewDescriptionCollection { Utilities.CreateFormViewDescription(CommonResources.Storage_Text, typeof(StorageRootPageControl)) };

	ClusterContext IViewContext.ClusterContext => clusterContext;

	public string[] DisplayColumns => new string[0];

	public string EmptyMessage => string.Empty;

	public bool IsEnumerating => false;

	internal StorageRootContext(ClusterContext clusterContext)
		: base(new Guid("{E8775B71-0EB4-4030-97BB-745A7F0B4D57}"), ExpandIconOptions.DoNotShow)
	{
		base.DisplayName = CommonResources.Storage_Text;
		base.ImageIndex = Icons.StorageIndex;
		this.clusterContext = clusterContext;
		UpdateStandardVerbs();
		AddChildContext(ContextFactory.CreateWpfClusterDisksContext(clusterContext), delayedAdd: true);
		AddChildContext(ContextFactory.CreateWpfClusterPoolsContext(clusterContext), delayedAdd: true);
		if (clusterContext.Cluster.CurrentVersion >= ClusterVersion.WindowsThreshold)
		{
			AddChildContext(ContextFactory.CreateWpfClusterEnclosuresContext(clusterContext), delayedAdd: true);
		}
	}

	public override void Dispose()
	{
		if (!isDisposed)
		{
			base.Dispose();
			GC.SuppressFinalize(this);
		}
	}

	protected override void UpdateStateBasedActions()
	{
	}

	public override void Refresh()
	{
	}

	protected virtual void UpdateStandardVerbs()
	{
		base.EnabledStandardVerbs &= ~StandardVerbs.Refresh;
	}
}

