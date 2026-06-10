using System;
using FailoverClusters.UI.Common;
using FailoverClusters.UI.Controls;
using ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal class NetworkInterfaceContext : ContextBase, IDisposable, IClusterSpecific
{
	private ClusterNetworkInterface networkInterface;

	private volatile ActionsPaneItemCollection actions;

	public Cluster Cluster => networkInterface.Cluster;

	public override Guid Id => networkInterface.Id;

	public override ActionsPaneItemCollection ActionsPaneItems
	{
		get
		{
			if (networkInterface == null || networkInterface.IsDeleted || isDisposed)
			{
				return new ActionsPaneItemCollection();
			}
			actions = Utilities.DisposeActions(actions);
			actions = new ActionsPaneItemCollection();
			actions.AddRange(new ActionsPaneItem[2]
			{
				new ActionSeparator(),
				ActionFactory.CreateAction(StringExtensions.ReplaceAccelerator(CommandResources.ShowCriticalEvents), Resources.ShowNetworkInterfaceCriticalEventsActionDescription_Text, Icons.ClusterEventsIndex, OnShowCriticalEvents)
			});
			return actions;
		}
	}

	internal NetworkInterfaceContext(ClusterNetworkInterface networkInterface)
		: base(new Guid("{423B143A-B85B-47d4-B8FC-79933927EC95}"))
	{
		this.networkInterface = networkInterface;
		this.networkInterface.StateChanged += OnStateChanged;
		this.networkInterface.PropertiesChanged += OnPropertiesChanged;
		base.DisplayName = this.networkInterface.Name;
		base.ImageIndex = IconsHelp.GetNetworkInterfaceIconIndex(this.networkInterface.State);
	}

	private void OnPropertiesChanged(object sender, EventArgs e)
	{
		base.DisplayName = networkInterface.Name;
	}

	private void OnStateChanged(object sender, EventArgs e)
	{
		base.ImageIndex = IconsHelp.GetNetworkInterfaceIconIndex(networkInterface.State);
	}

	public override void ClearActions()
	{
		actions = Utilities.DisposeActions(actions);
		base.ClearActions();
	}

	private void OnShowCriticalEvents(object sender, SnapinActionEventArgs e)
	{
		LegacyFactory.ExecuteCriticalEventsDialog(networkInterface.Cluster, base.DisplayName, base.NodeType, delegate(EventLogFilter filter)
		{
			filter.ClusterNetworkInterface = base.DisplayName;
		}, null);
	}

	public override void Dispose()
	{
		if (!isDisposed)
		{
			base.Dispose();
			actions = Utilities.DisposeActions(actions);
			networkInterface.StateChanged -= OnStateChanged;
			networkInterface.PropertiesChanged -= OnPropertiesChanged;
			GC.SuppressFinalize(this);
		}
	}

	protected override void UpdateStateBasedActions()
	{
	}
}

