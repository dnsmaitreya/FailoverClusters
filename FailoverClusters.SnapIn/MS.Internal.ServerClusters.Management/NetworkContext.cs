using System;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.FailoverClusters.UI.Controls;
using Microsoft.ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal class NetworkContext : ScopeNodeContextBase, IHasPropertyPages, IDisposable, IClusterSpecific
{
	private EventHandler stateChangedEventHandler;

	private EventHandler propertiesChangedEventHandler;

	private ClusterNetwork network;

	private ClusterContext clusterContext;

	private volatile ActionsPaneItemCollection actions;

	public Cluster Cluster => clusterContext.Cluster;

	public override Guid Id => network.Id;

	public override ViewDescriptionCollection ViewDescriptions => new ViewDescriptionCollection();

	public override ActionsPaneItemCollection ActionsPaneItems
	{
		get
		{
			if (network == null || network.IsDeleted || isDisposed)
			{
				return new ActionsPaneItemCollection();
			}
			lock (base.ActionsLock)
			{
				actions = Utilities.DisposeActions(actions);
				actions = new ActionsPaneItemCollection();
				actions.AddRange(new ActionsPaneItem[2]
				{
					new ActionSeparator(),
					ActionFactory.CreateAction(StringExtensions.ReplaceAccelerator(CommandResources.ShowCriticalEvents), Resources.ShowNetworkCriticalEventsActionDescription_Text, Icons.ClusterEventsIndex, OnShowCriticalEvents)
				});
			}
			return actions;
		}
	}

	public ClusterNetwork Network => network;

	public PropertyPageCollection PropertyPages
	{
		get
		{
			PropertyPageCollection propertyPageCollection = new PropertyPageCollection();
			ClusterPropertyPage clusterPropertyPage = new ClusterPropertyPage();
			clusterPropertyPage.SetControl(new NetworkGeneralPropertiesPage(this));
			propertyPageCollection.Add(clusterPropertyPage);
			return propertyPageCollection;
		}
	}

	internal NetworkContext(ClusterNetwork network, ClusterContext clusterContext)
		: base(new Guid("{F5545327-F2FD-410a-A56C-0D7B90C4FC72}"), ExpandIconOptions.Show)
	{
		this.clusterContext = clusterContext;
		this.network = network;
		base.DisplayName = this.network.Name;
		base.ImageIndex = IconsHelp.GetNetworkIconIndex(this.network.State);
		stateChangedEventHandler = OnStateChanged;
		this.network.StateChanged += stateChangedEventHandler;
		propertiesChangedEventHandler = OnPropertiesChanged;
		this.network.PropertiesChanged += propertiesChangedEventHandler;
	}

	private void OnPropertiesChanged(object sender, EventArgs e)
	{
		base.DisplayName = network.Name;
	}

	private void OnStateChanged(object sender, EventArgs e)
	{
		base.ImageIndex = IconsHelp.GetNetworkIconIndex(network.State);
	}

	protected override void UpdateStateBasedActions()
	{
	}

	public override void ClearActions()
	{
		lock (base.ActionsLock)
		{
			actions = Utilities.DisposeActions(actions);
		}
		base.ClearActions();
	}

	private void OnShowCriticalEvents(object sender, SnapinActionEventArgs e)
	{
		LegacyFactory.ExecuteCriticalEventsDialog(Network.Cluster, base.DisplayName, base.NodeType, delegate(EventLogFilter filter)
		{
			filter.ClusterNetwork = base.DisplayName;
		}, null);
	}

	public override void Refresh()
	{
		base.DisplayName = network.Name;
		base.ImageIndex = IconsHelp.GetNetworkIconIndex(network.State);
		RefreshChildren(SortOptions.Sort);
	}

	public override void Dispose()
	{
		if (!isDisposed)
		{
			base.Dispose();
			lock (base.ActionsLock)
			{
				actions = Utilities.DisposeActions(actions);
			}
			if (network != null)
			{
				network.StateChanged -= stateChangedEventHandler;
				network.PropertiesChanged -= propertiesChangedEventHandler;
			}
			GC.SuppressFinalize(this);
		}
	}
}
