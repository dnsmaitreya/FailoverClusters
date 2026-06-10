using System;
using System.Collections.Generic;
using FailoverClusters.ClusterSnapIn;
using FailoverClusters.Framework;
using FailoverClusters.SnapIn;
using FailoverClusters.UI.Common;
using FailoverClusters.UIFramework;
using ManagementConsole;

namespace KDDSL.ServerClusters.Management;

internal class WpfClusterDisksContext : ScopeNodeContextBase, IClusterSpecific, IViewContext
{
	private ClusterGroup availableStorageGroup;

	private readonly CollectionNotificationManager<ClusterNode> nodeNotifications;

	private readonly ClusterContext clusterContext;

	private volatile ActionsPaneItemCollection actions;

	private bool actionsAreDirty = true;

	private ClusterCommand moveGroupCommand;

	private ClusterCommand moveGroupToBestCommand;

	private ClusterCommand moveGroupToSelectedCommand;

	private static readonly Guid contextId = new Guid("{fc4aec75-a9fd-424c-9dfa-3ed0e10c0df8}");

	private Action<ViewModelDataSignal> signalViewModel;

	public Cluster Cluster => clusterContext.Cluster;

	internal ClusterContext ClusterContext => clusterContext;

	public override ActionsPaneItemCollection ActionsPaneItems
	{
		get
		{
			if (isDisposed)
			{
				return new ActionsPaneItemCollection();
			}
			lock (base.ActionsLock)
			{
				if (actionsAreDirty)
				{
					ClearActions();
					CreateActions();
					actionsAreDirty = false;
				}
			}
			return actions;
		}
	}

	public override ViewDescriptionCollection ViewDescriptions
	{
		get
		{
			ViewDescriptionCollection viewDescriptionCollection = new ViewDescriptionCollection();
			ViewModelData viewModelData = new ViewModelData(clusterContext.Cluster.Id)
			{
				HelpTopic = HelpTopic
			};
			signalViewModel = viewModelData.ProcessMessage;
			viewDescriptionCollection.Add(new FormViewDescription
			{
				DisplayName = CommonResources.Storage_Text,
				ViewType = typeof(ClusterDisksViewAdapter),
				ControlType = typeof(WpfViewHostControl<ClusterDisksView, ClusterDisksViewAdapter>),
				Tag = viewModelData
			});
			viewDescriptionCollection.DefaultIndex = 0;
			return viewDescriptionCollection;
		}
	}

	ClusterContext IViewContext.ClusterContext => clusterContext;

	public string[] DisplayColumns => new string[0];

	public string EmptyMessage => string.Empty;

	public bool IsEnumerating => false;

	internal WpfClusterDisksContext(ClusterContext clusterContext)
		: base(contextId, ExpandIconOptions.Show)
	{
		base.DisplayName = Resources.Disks_Text;
		base.ImageIndex = Icons.PhysicalDiskIndex;
		this.clusterContext = clusterContext;
		availableStorageGroup = ClusterContext.Cluster.GetAvailableStorageGroup();
		availableStorageGroup.StateChanged += OnAvailableStorageGroupStateChanged;
		this.clusterContext.FrameworkCluster.Refreshed += OnAvailableStorageGroupStateChanged;
		Cluster.PropertiesChanged += ClusterPropertiesChanged;
		nodeNotifications = new CollectionNotificationManager<ClusterNode>((GetCollection<ClusterNode>)availableStorageGroup.Cluster.GetNodes, (NotificationSubscription<ClusterNode>)Utilities.NodeStateSubscription);
		availableStorageGroup.Cluster.NodesChanged += nodeNotifications.OnCollectionChanged;
		nodeNotifications.NotificationRaised += OnAvailableStorageGroupStateChanged;
		nodeNotifications.StartNotificationMonitoring();
	}

	private void ClusterPropertiesChanged(object sender, EventArgs e)
	{
		actionsAreDirty = true;
		OnActionsUpdated();
	}

	private void OnAvailableStorageGroupStateChanged(object sender, EventArgs e)
	{
		moveActionsAreDirty = true;
		actionsAreDirty = true;
		OnActionsUpdated();
	}

	public override void ClearActions()
	{
		lock (base.ActionsLock)
		{
			if (actions != null)
			{
				actions = Utilities.DisposeActions(actions);
			}
		}
		actionsAreDirty = true;
		base.ClearActions();
	}

	public override void Dispose()
	{
		if (!isDisposed)
		{
			base.Dispose();
			if (availableStorageGroup != null)
			{
				availableStorageGroup.StateChanged -= OnAvailableStorageGroupStateChanged;
				availableStorageGroup = null;
			}
			clusterContext.FrameworkCluster.Refreshed -= OnAvailableStorageGroupStateChanged;
			Cluster.PropertiesChanged -= ClusterPropertiesChanged;
			if (availableStorageGroup != null)
			{
				availableStorageGroup.Cluster.NodesChanged -= nodeNotifications.OnCollectionChanged;
				availableStorageGroup = null;
			}
			if (nodeNotifications != null)
			{
				nodeNotifications.NotificationRaised -= OnAvailableStorageGroupStateChanged;
				nodeNotifications.Dispose();
			}
			ClearActions();
			GC.SuppressFinalize(this);
		}
	}

	public override void Refresh()
	{
		signalViewModel.SafeCall(ViewModelDataSignal.TargetedRefresh);
		Cluster.Refresh();
	}

	protected override ActionsPaneItem[] CreateMoveActions()
	{
		AverageGroup obj = clusterContext.FrameworkCluster.AvailableStorage as AverageGroup;
		if (obj == null)
		{
			throw new InvalidOperationException("Available Storage group must be an average group.");
		}
		List<ActionsPaneItem> list = new List<ActionsPaneItem>();
		ActionGroup actionGroup = new ActionGroup(StringExtensions.ReplaceAccelerator(CommandResources.MoveAvailableStorageActionGroup_Text), CommandResources.MoveAvailableStorageActionGroupDescription_Text, Icons.MoveAvailableStorageIndex);
		list.Add(actionGroup);
		AverageGroupMoveCommand averageGroupMoveCommand = obj.AverageGroupMoveCommand;
		moveGroupToBestCommand = averageGroupMoveCommand.AverageGroupMoveToBestCommand;
		MmcActionPaneItem mmcActionPaneItem = new MoveGroupActionPaneItem(obj, moveGroupToBestCommand, null, moveToBest: true);
		actionGroup.Items.Add(mmcActionPaneItem.Action);
		moveGroupCommand = averageGroupMoveCommand;
		moveGroupToSelectedCommand = averageGroupMoveCommand.AverageGroupMoveToSelectedCommand;
		MmcActionPaneItem mmcActionPaneItem2 = new MoveGroupActionPaneItem(obj, moveGroupToSelectedCommand, moveGroupCommand, moveToBest: false);
		actionGroup.Items.Add(mmcActionPaneItem2.Action);
		return list.ToArray();
	}

	private void CreateActions()
	{
		ActionsPaneItem[] items = base.MoveActions;
		lock (base.ActionsLock)
		{
			actions = new ActionsPaneItemCollection
			{
				ActionFactory.CreateAction(StringExtensions.ReplaceAccelerator(CommandResources.AddDiskStorageAction_Text), CommandResources.AddDiskStorageActionDescription_Text, Icons.AddDiskIndex, AddDiskAction),
				new ActionSeparator()
			};
			actions.AddRange(items);
		}
		UpdateStateBasedActions();
	}

	private void AddDiskAction(object sender, SnapinActionEventArgs e)
	{
		new AddDisksOperation(ClusterContext.Cluster).Execute();
	}

	protected override void UpdateStateBasedActions()
	{
	}
}

