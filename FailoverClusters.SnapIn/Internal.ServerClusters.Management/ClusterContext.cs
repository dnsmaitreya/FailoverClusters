using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using FailoverClusters.ClusterSnapIn;
using FailoverClusters.Configuration;
using FailoverClusters.Framework;
using FailoverClusters.SnapIn;
using FailoverClusters.UI.Common;
using FailoverClusters.UI.Common.Services;
using FailoverClusters.UIFramework;
using ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal class ClusterContext : ScopeNodeContextBase, IHasPropertyPages, ICloseable, IClusterSpecific
{
	private Cluster cluster;

	private CollectionNotificationManager<ClusterNode> nodeNotifications;

	private ClusterResource quorumResource;

	private ReportChannel report;

	private volatile ActionsPaneItemCollection actions;

	private bool reportConnectionChanges;

	private ResourceTypeManager resTypeManager;

	private ClusterEventsMonitor clusterEventsMonitor;

	private ClusterGroup clusterGroup;

	private bool actionsAreDirty = true;

	private ClusterCommand moveGroupCommand;

	private ClusterCommand moveGroupToBestCommand;

	private ClusterCommand moveGroupToSelectedCommand;

	private DestroyClusterActionPaneItem destroyClusterActionPaneItem;

	private UICommand destroyClusterCommand;

	private ShutdownClusterActionPaneItem shutdownClusterActionPaneItem;

	private UICommand shutdownClusterCommand;

	private bool isClusterFailureSoon;

	private readonly object isStorageReplicaInstalledLock = new object();

	private bool? isStorageReplicaFeatureInstalled;

	private string[] storageReplicaMissingNodes;

	private bool disposable = true;

	private CluadminWaitDialog waitDialog;

	private int closingCluster;

	private Action<ViewModelDataSignal> signalViewModel;

	internal ActionBase ConfigureRoleAction { get; private set; }

	internal ActionBase ValidateClusterAction { get; private set; }

	internal ActionBase AddNodeAction { get; private set; }

	internal ActionBase CopyClusterRolesAction { get; private set; }

	internal ActionBase ClusterAwareUpdatingAction { get; private set; }

	public FailoverClusters.Framework.Cluster FrameworkCluster { get; private set; }

	public ResourceTypeManager ResourceTypeManager => resTypeManager;

	public bool ReportConnectionChanges
	{
		set
		{
			reportConnectionChanges = value;
		}
	}

	public ClusterEventsMonitor ClusterEventsMonitor => clusterEventsMonitor;

	public override ViewDescriptionCollection ViewDescriptions
	{
		get
		{
			ViewDescriptionCollection viewDescriptionCollection = new ViewDescriptionCollection();
			ViewModelData viewModelData = new ViewModelData(Cluster.Id)
			{
				HelpTopic = HelpTopic,
				ViewCommandsProvider = (IViewCommandsProvider)(object)new ClusterOverviewCommandsProvider(this)
			};
			signalViewModel = viewModelData.ProcessMessage;
			FailoverClusters.Framework.Cluster frameworkCluster = FrameworkCluster;
			if (frameworkCluster != null)
			{
				viewDescriptionCollection.Add(new FormViewDescription
				{
					DisplayName = frameworkCluster.DisplayName,
					ViewType = typeof(ClusterOverviewViewAdapter),
					ControlType = typeof(WpfViewHostControl<ClusterOverview, ClusterOverviewViewAdapter>),
					Tag = viewModelData
				});
			}
			return viewDescriptionCollection;
		}
	}

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
					actions = CreateActions();
					actionsAreDirty = false;
					moveActionsAreDirty = false;
				}
			}
			return actions;
		}
	}

	public Cluster Cluster => cluster;

	public override List<WritableSharedDataItem> SharedData
	{
		get
		{
			List<WritableSharedDataItem> sharedData = base.SharedData;
			WritableSharedDataItem item = new WritableSharedDataItem("CLUSTER_NAME", requiresCallback: true);
			sharedData.Add(item);
			return sharedData;
		}
	}

	public PropertyPageCollection PropertyPages
	{
		get
		{
			PropertyPageCollection propertyPageCollection = new PropertyPageCollection();
			ClusterPropertyPage clusterPropertyPage = new ClusterPropertyPage();
			clusterPropertyPage.SetControl(new ClusterGeneralPropertiesPage(this));
			propertyPageCollection.Add(clusterPropertyPage);
			clusterPropertyPage = new ClusterPropertyPage();
			clusterPropertyPage.SetControl(new ClusterResourceTypesPropertiesPage(this));
			propertyPageCollection.Add(clusterPropertyPage);
			clusterPropertyPage = new ClusterPropertyPage();
			clusterPropertyPage.SetControl(new ClusterAutoBalancerPropertiesPage(this));
			propertyPageCollection.Add(clusterPropertyPage);
			return propertyPageCollection;
		}
	}

	private bool MarkedForDelete => !disposable;

	public event EventHandler<ChildDeletedEventArgs> Closed;

	private void CommonConstruct()
	{
		if (cluster == null)
		{
			throw new InvalidOperationException("this.cluster must not be null");
		}
		try
		{
			if (Cluster.ManagementPointType == AdminAccessPoint.None)
			{
				throw new ClusterUnsupportedNoCnoException();
			}
			Utilities.VerifyUserIsAdminOnNodes(cluster);
			base.DisplayName = cluster.FqdnName;
			base.ImageIndex = Icons.ClusterIndex;
			cluster.PropertiesChanged += ClusterPropertiesChanged;
			cluster.ConnectionChanged += ClusterConnectionChanged;
			cluster.QuorumChanged += ClusterQuorumChanged;
			SubscribeToQuorumNotification();
			clusterGroup = cluster.GetCoreClusterGroup();
			clusterGroup.StateChanged += OnClusterGroupStateChanged;
			nodeNotifications = new CollectionNotificationManager<ClusterNode>((GetCollection<ClusterNode>)cluster.GetNodes, (NotificationSubscription<ClusterNode>)Utilities.NodeStateSubscription);
			cluster.NodesChanged += nodeNotifications.OnCollectionChanged;
			nodeNotifications.NotificationRaised += OnClusterQuorumStateChanged;
			IntPtr intPtr = cluster.ClusterHandle.DangerousGetHandle();
			FrameworkCluster = new FailoverClusters.Framework.Cluster
			{
				Adapter = ClusterAdapterType.ClusterApi
			};
			FrameworkCluster.Open(intPtr.ToString(), cluster.ClusterHandle, cluster.Id, (int)cluster.ApiAccessLevel);
			FrameworkCluster.FatalError += OnClusterFatalError;
			FrameworkCluster.Refreshed += OnRefreshed;
			FrameworkCluster.Disconnected += FrameworkClusterDisconnected;
			cluster.FrameworkCluster = (IFrameworkCluster)(object)FrameworkCluster;
			clusterEventsMonitor = new ClusterEventsMonitor(FrameworkCluster);
			clusterEventsMonitor.ClusterEventsChanged += ClusterEventsChanged;
			ServiceContainer.Container.Resolve<IClusterEventsMonitorRegistry>(Array.Empty<object>()).AddClusterEventsMonitor((IClusterEventsMonitor)(object)clusterEventsMonitor);
			Background.QueueWorker((WaitCallback)StartEventSubscriptions);
			ComputeImageIndex();
			AddChildren();
			nodeNotifications.StartNotificationMonitoring();
			resTypeManager = new ResourceTypeManager(cluster);
			ClusterConnectionFactory.AddClusterContextToCache(this);
			ClusterAdministrator.Instance.RegisterShutdownCallback(OnClusterAdministratorShutdown);
			Task.Run(delegate
			{
				IsStorageReplicaInstalledOnAllNodes(out var _);
			});
		}
		catch (ClusterBaseException)
		{
			Dispose();
			throw;
		}
		catch (ClusterException)
		{
			Dispose();
			throw;
		}
		catch (Exception innerException)
		{
			Dispose();
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[1] { Resources.CreateClusterContextFailed_Text });
		}
	}

	private void OnRefreshed(object sender, ClusterRefreshedEventArgs e)
	{
		isStorageReplicaFeatureInstalled = null;
		storageReplicaMissingNodes = null;
		moveActionsAreDirty = true;
		actionsAreDirty = true;
		OnActionsUpdated();
	}

	private void StartEventSubscriptions(object param)
	{
		clusterEventsMonitor.UpdateClusterEvents();
	}

	private void OnClusterAdministratorShutdown()
	{
		CloseClusters();
	}

	internal bool IsStorageReplicaInstalledOnAllNodes(out string[] missingNodes)
	{
		if (isStorageReplicaFeatureInstalled.HasValue)
		{
			missingNodes = storageReplicaMissingNodes;
			return isStorageReplicaFeatureInstalled.Value;
		}
		lock (isStorageReplicaInstalledLock)
		{
			if (isStorageReplicaFeatureInstalled.HasValue)
			{
				missingNodes = storageReplicaMissingNodes;
				return isStorageReplicaFeatureInstalled.Value;
			}
			List<string> missingNodesList = new List<string>();
			Parallel.ForEach(cluster.GetNodes(), delegate(ClusterNode node)
			{
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				try
				{
					if ((node.State == NodeState.Up || node.State == NodeState.Paused) && !new WindowsFeature(node.FqdnName).IsInstalled(ServerRoleId.StorageReplica))
					{
						missingNodesList.Add(node.Name.ToLower(CultureInfo.CurrentCulture));
					}
				}
				catch (ApplicationException caughtException)
				{
					ExceptionHelp.LogException(caughtException, "There was an error if the Storage Replica feature is installedon node '{0}'", node.Name);
				}
				catch (UnauthorizedAccessException caughtException2)
				{
					ExceptionHelp.LogException(caughtException2, "There was an error if the Storage Replica feature is installedon node '{0}'", node.Name);
				}
			});
			storageReplicaMissingNodes = (missingNodes = missingNodesList.ToArray());
			isStorageReplicaFeatureInstalled = storageReplicaMissingNodes.Length == 0;
			return isStorageReplicaFeatureInstalled.Value;
		}
	}

	private void CloseClusters()
	{
		if (Interlocked.CompareExchange(ref closingCluster, 1, 0) != 0)
		{
			return;
		}
		TimeSpan timeout = TimeSpan.FromSeconds(5.0);
		if (FrameworkCluster != null)
		{
			FrameworkCluster.FatalError -= OnClusterFatalError;
			FrameworkCluster.Refreshed -= OnRefreshed;
			FrameworkCluster.Disconnected -= FrameworkClusterDisconnected;
			AutoResetEvent shutdownEvent = new AutoResetEvent(initialState: false);
			try
			{
				FrameworkCluster.Close(delegate
				{
					try
					{
						shutdownEvent.Set();
					}
					catch (ObjectDisposedException)
					{
					}
				});
				shutdownEvent.WaitOne(timeout);
			}
			finally
			{
				if (shutdownEvent != null)
				{
					((IDisposable)shutdownEvent).Dispose();
				}
			}
			FrameworkCluster = null;
		}
		moveGroupCommand = null;
		moveGroupToBestCommand = null;
		moveGroupToSelectedCommand = null;
		AutoResetEvent shutdownEvent2 = new AutoResetEvent(initialState: false);
		try
		{
			Worker.Start(delegate
			{
				cluster?.Close();
				try
				{
					shutdownEvent2.Set();
				}
				catch (ObjectDisposedException)
				{
				}
			});
			shutdownEvent2.WaitOne(timeout);
		}
		finally
		{
			if (shutdownEvent2 != null)
			{
				((IDisposable)shutdownEvent2).Dispose();
			}
		}
		cluster.FrameworkCluster = null;
		cluster = null;
	}

	private void OnClusterGroupStateChanged(object sender, EventArgs e)
	{
		moveActionsAreDirty = true;
		actionsAreDirty = true;
		OnActionsUpdated();
	}

	internal void AddChildren()
	{
		AddChildContext(ContextFactory.CreateWpfRolesContext(this), delayedAdd: true);
		AddChildContext(ContextFactory.CreateWpfAllNodesContext(this), delayedAdd: true);
		AddChildContext(new StorageRootContext(this), delayedAdd: true);
		AddChildContext(ContextFactory.CreateWpfAllNetworksContext(this), delayedAdd: true);
		AddChildContext(ContextFactory.CreateClusterEventsContext(cluster), delayedAdd: true);
	}

	private void CreateStateBasedActions()
	{
		UpdateStateBasedActions();
	}

	private void ClusterQuorumChanged(object sender, EventArgs e)
	{
		SubscribeToQuorumNotification();
		OnClusterQuorumStateChanged(sender, e);
	}

	private void SubscribeToQuorumNotification()
	{
		if (quorumResource != null)
		{
			quorumResource.StateChanged -= OnClusterQuorumStateChanged;
		}
		quorumResource = cluster.GetQuorumResource();
		if (quorumResource != null)
		{
			quorumResource.StateChanged += OnClusterQuorumStateChanged;
		}
	}

	private void FrameworkClusterDisconnected(object sender, ClusterDisconnectedEventArgs e)
	{
		ClusterLog.LogVerbose((LogSubcategory)10, "Framework invoked a cluster disconnected");
		DisconectAndLoadDownClusterPage((FailoverClusters.Framework.Cluster)sender);
	}

	private void DisconectAndLoadDownClusterPage(FailoverClusters.Framework.Cluster frameworkCluster)
	{
		Close();
		if (frameworkCluster != null)
		{
			ClusterLog.AdminEvents.WriteClusterDisconnectedEvent(frameworkCluster.Name);
			ClusterLog.LogVerbose((LogSubcategory)10, "Cluster has been disconnected, creating down cluster view");
			ClusterAdministrator.CreateDownClusterView(frameworkCluster.FullyQualifiedDomainName);
			ClusterAdministrator.SetStatusBarProgressMessage(string.Format(CultureInfo.CurrentCulture, Resources.ClusterDisconnectedFormat_Text, frameworkCluster.Name));
		}
	}

	private void OnClusterFatalError(object sender, ClusterEventArgs e)
	{
		throw new ClusterErrorException(cluster.FqdnName, e.Error);
	}

	private void OnClusterQuorumStateChanged(object sender, EventArgs e)
	{
		moveActionsAreDirty = true;
		actionsAreDirty = true;
		OnActionsUpdated();
		isClusterFailureSoon = IsClusterFailureSoon(out var _);
		ComputeImageIndex();
	}

	private void ComputeImageIndex()
	{
		if (isClusterFailureSoon)
		{
			base.ImageIndex = Icons.ClusterAlertIndex;
		}
		else if (clusterEventsMonitor.ClusterEventsCount != 0)
		{
			base.ImageIndex = Icons.ClusterCriticalEventIndex;
		}
		else
		{
			base.ImageIndex = Icons.ClusterIndex;
		}
	}

	private void ClusterEventsChanged(object sender, EventArgs e)
	{
		ComputeImageIndex();
	}

	public bool IsClusterFailureSoon(out string failureNode)
	{
		failureNode = string.Empty;
		if (cluster.GetNodeCount() == 1)
		{
			return true;
		}
		ClusterResource clusterResource = cluster.GetQuorumResource();
		if (clusterResource != null && clusterResource.WillOfflineLoseQuorum())
		{
			return true;
		}
		foreach (ClusterNode node in cluster.GetNodes())
		{
			if (node.SafeNodeState == NodeState.Up)
			{
				if (node.WillDownLoseQuorum())
				{
					if (cluster.DynamicQuorumEnabled && node.NodeWeight == 1)
					{
						failureNode = node.Name;
					}
					return true;
				}
				break;
			}
		}
		return false;
	}

	private ClusterContext()
		: base(new Guid(ClusterAdministrator.ClusterContextGuid.ToString()), ExpandIconOptions.DoNotShow)
	{
	}

	internal ClusterContext(Cluster cluster)
		: this()
	{
		Exceptions.ThrowIfNull((object)cluster, "cluster");
		this.cluster = cluster;
		CommonConstruct();
	}

	internal ClusterContext(string clusterName)
		: this()
	{
		Exceptions.ThrowIfNull((object)clusterName, "clusterName");
		if (!NetworkHelper.CanPing(clusterName))
		{
			throw new ClusterRpcConnectionException(clusterName);
		}
		ClusterItem.CachingDisabled = true;
		cluster = Cluster.Open(clusterName, ClusterAccessRights.GenericAll);
		try
		{
			CommonConstruct();
		}
		catch
		{
			MS.Internal.ServerClusters.Utilities.BackgroundDisposeObject(cluster);
			throw;
		}
	}

	private void ClusterConnectionChanged(object sender, ClusterConnectionEventArgs e)
	{
		if (!MarkedForDelete && reportConnectionChanges)
		{
			switch (e.ConnectionState)
			{
			case ClusterConnectionState.Disconnected:
				ClusterLog.LogVerbose((LogSubcategory)10, "Cluster has been disconnected");
				DisconectAndLoadDownClusterPage(FrameworkCluster);
				break;
			case ClusterConnectionState.Reconnected:
				ClusterLog.LogVerbose((LogSubcategory)10, "Cluster has been reconnected");
				ClusterAdministrator.SetStatusBarProgressMessage(string.Format(CultureInfo.CurrentCulture, Resources.ClusterReconnectedFormat_Text, cluster.Name));
				ClusterLog.AdminEvents.WriteClusterReconnectedEvent(cluster.Name);
				Refresh();
				OnActionsUpdated();
				break;
			case ClusterConnectionState.Closing:
				ClusterLog.LogVerbose((LogSubcategory)10, "Cluster has been closed");
				break;
			default:
				DebugLog.LogWarning("Unknown cluster connection: " + e.ConnectionState);
				break;
			}
		}
	}

	private void ClusterPropertiesChanged(object sender, EventArgs e)
	{
		base.DisplayName = cluster.FqdnName;
		UpdateStateBasedActions();
	}

	public override void ClearActions()
	{
		lock (base.ActionsLock)
		{
			destroyClusterActionPaneItem?.Dispose();
			destroyClusterActionPaneItem = null;
			destroyClusterCommand = null;
			shutdownClusterActionPaneItem?.Dispose();
			shutdownClusterActionPaneItem = null;
			shutdownClusterCommand = null;
			if (actions != null)
			{
				actions = Utilities.DisposeActions(actions);
			}
			moveActionsAreDirty = true;
			actionsAreDirty = true;
		}
		base.ClearActions();
	}

	protected override ActionsPaneItem[] CreateMoveActions()
	{
		List<ActionsPaneItem> list = new List<ActionsPaneItem>();
		try
		{
			FailoverClusters.Framework.Cluster frameworkCluster = FrameworkCluster;
			if (frameworkCluster != null)
			{
				AverageGroup obj = frameworkCluster.CoreGroup as AverageGroup;
				if (obj == null)
				{
					throw new InvalidOperationException("Core cluster group must be an average group.");
				}
				ActionGroup actionGroup = new ActionGroup(CommandResources.MoveClusterGroupActionGroup_Text, Resources.MoveClusterGroupActionGroupDescription_Text);
				list.Add(actionGroup);
				AverageGroupMoveCommand averageGroupMoveCommand = obj.AverageGroupMoveCommand;
				moveGroupToBestCommand = averageGroupMoveCommand.AverageGroupMoveToBestCommand;
				MmcActionPaneItem mmcActionPaneItem = new MoveGroupActionPaneItem(obj, moveGroupToBestCommand, null, moveToBest: true);
				actionGroup.Items.Add(mmcActionPaneItem.Action);
				moveGroupCommand = averageGroupMoveCommand;
				moveGroupToSelectedCommand = averageGroupMoveCommand.AverageGroupMoveToSelectedCommand;
				MmcActionPaneItem mmcActionPaneItem2 = new MoveGroupActionPaneItem(obj, moveGroupToSelectedCommand, moveGroupCommand, moveToBest: false);
				actionGroup.Items.Add(mmcActionPaneItem2.Action);
			}
		}
		catch (ClusterObjectNotFoundException ex)
		{
			ClusterLog.LogException((Exception)ex, "There was an error using the core cluster group.");
		}
		return list.ToArray();
	}

	private ActionsPaneItemCollection CreateActions()
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_00a1: Expected O, but got Unknown
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Expected O, but got Unknown
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Expected O, but got Unknown
		//IL_0111: Expected O, but got Unknown
		lock (base.ActionsLock)
		{
			CreateStateBasedActions();
			ActionGroup actionGroup = new ActionGroup(StringExtensions.ReplaceAccelerator(CommandResources.MoreActions), Resources.MoreActionsActionsGroupDescription_Text, Icons.BlueArrowIndex);
			UICommand val = new UICommand("DestroyCluster", (UICommandId)14, (Action<object>)delegate
			{
			}, (Predicate<object>)((object x) => true));
			((ClusterCommand)val).Text = ActionFactory.GenerateDisplayName(CommandResources.DestroyClusterAction_Text);
			((ClusterCommand)val).Description = ActionFactory.GenerateDisplayName(Resources.DestroyClusterAction_Description_Text);
			destroyClusterCommand = val;
			UICommand val2 = new UICommand("ShutdownCluster", (UICommandId)15, (Action<object>)delegate
			{
			}, (Predicate<object>)((object x) => true));
			((ClusterCommand)val2).Text = ActionFactory.GenerateDisplayName(CommandResources.ShutdownClusterAction_Text);
			((ClusterCommand)val2).Description = ActionFactory.GenerateDisplayName(Resources.ShutdownClusterAction_Description_Text);
			shutdownClusterCommand = val2;
			destroyClusterActionPaneItem = new DestroyClusterActionPaneItem((ICommand)destroyClusterCommand, this);
			shutdownClusterActionPaneItem = new ShutdownClusterActionPaneItem((ICommand)shutdownClusterCommand, this);
			List<ActionsPaneItem> list = new List<ActionsPaneItem>();
			CopyClusterRolesAction = ActionFactory.CreateAction(CommandResources.MigrateClusterAction_Text, Extensions.FormatCurrentCulture(Resources.MigrateClusterAction_Description_Text, (object)BrandingResources.WINDOWS_SERVER_CURRENT_VERSION), Icons.MigrateClusterIndex, OnMigrateCluster);
			list.AddRange(new ActionsPaneItem[7]
			{
				ActionFactory.CreateAction(CommandResources.ConfigureQuorumAction_Text, Resources.ConfigureQuorumAction_Description_Text, Icons.StorageIndex, OnConfigureQuorum),
				new ActionSeparator(),
				CopyClusterRolesAction,
				new ActionSeparator(),
				shutdownClusterActionPaneItem.Action,
				new ActionSeparator(),
				destroyClusterActionPaneItem.Action
			});
			ActionsPaneItem[] array = base.MoveActions;
			if (array != null && array.Length != 0)
			{
				try
				{
					ActionsPaneItem item = array[0];
					list.Add(new ActionSeparator());
					list.Add(item);
				}
				catch (IndexOutOfRangeException)
				{
				}
			}
			ClusterAwareUpdatingAction = ActionFactory.CreateAction(CommandResources.UpdateCluster, Resources.UpdateCluster_Description, Icons.UpdatesIndex, OnUpdateCluster);
			list.AddRange(new ActionsPaneItem[2]
			{
				new ActionSeparator(),
				ClusterAwareUpdatingAction
			});
			actionGroup.Items.AddRange(list.ToArray());
			ActionsPaneItemCollection actionsPaneItemCollection = new ActionsPaneItemCollection();
			ConfigureRoleAction = SharedActions.CreateNewServiceOrApplicationAction(OnMakeAppOrServiceHa);
			ValidateClusterAction = SharedActions.CreateValidationAction(CommandResources.ValidateConfigurationBPAAction_Text, OnValidateCluster);
			AddNodeAction = SharedActions.CreateAddNodesAction(OnAddNode);
			actionsPaneItemCollection.AddRange(new ActionsPaneItem[11]
			{
				ConfigureRoleAction,
				ValidateClusterAction,
				SharedActions.CreateViewValidationReportAction(OnViewValidationReport),
				new ActionSeparator(),
				AddNodeAction,
				new ActionSeparator(),
				SharedActions.CreateCloseConnectionAction(OnCloseConnection),
				new ActionSeparator(),
				SharedActions.CreateResetRecentEventsAction(cluster),
				new ActionSeparator(),
				actionGroup
			});
			return actionsPaneItemCollection;
		}
	}

	private bool OnlyCoreGroupsPresent()
	{
		if (cluster.GetGroupCount() == 0L)
		{
			return true;
		}
		foreach (ClusterGroup group in cluster.GetGroups())
		{
			if (group.GroupType != GroupType.ClusterStoragePool)
			{
				return false;
			}
		}
		return true;
	}

	internal byte[] GetClusterNameBytes()
	{
		return Encoding.Unicode.GetBytes(Cluster.ConnectedTo + "\0");
	}

	private void OnCloseConnection(object sender, SnapinActionEventArgs e)
	{
		Background.QueueWorker((WaitCallback)delegate
		{
			Close();
		});
	}

	public void ResetRecentEvents()
	{
		using CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.ResetRecentEvents_Text, Resources.ResettingRecentEventsTime_Text);
		cluadminWaitDialog.ShowDialog(ClusterAdministrator.NotifyUser, delegate
		{
			DateTime currentTime = DateTime.UtcNow;
			try
			{
				PropertyCollection commonProperties = Cluster.GetCommonProperties(PropertyCollectionSet.ReadWrite);
				if (commonProperties.TryGetProperty("RecentEventsResetTime", out var property))
				{
					property.Value = currentTime;
					commonProperties.SaveChanges();
				}
			}
			catch (ApplicationException ex)
			{
				ClusterLog.LogException((Exception)ex, "There was an error saving the cluster common properties for the last events reset property");
			}
			catch (ClusterBaseException ex2)
			{
				ClusterLog.LogException((Exception)ex2, "There was an error saving the cluster common properties for the last events reset property");
			}
			Worker.Start(delegate
			{
				clusterEventsMonitor.UpdateClusterEvents(currentTime);
			});
		});
	}

	private void OnAddNode(object sender, SnapinActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		SharedActions.PerformAddNodesAction(cluster, notifyUserFromSender);
	}

	private void OnMakeAppOrServiceHa(object sender, SnapinActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		SharedActions.PerformNewServiceOrApplicationAction(cluster, notifyUserFromSender);
	}

	private void OnViewValidationReport(object sender, SnapinActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		SharedActions.PerformViewValidationReportAction(cluster, notifyUserFromSender, e.Action.DisplayName);
	}

	private void OnValidateCluster(object sender, SnapinActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		SharedActions.PerformValidateClusterConfiguration(Cluster, notifyUserFromSender);
	}

	private void OnMigrateCluster(object sender, SnapinActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		SharedActions.PerformCopyClusterRoles(cluster, notifyUserFromSender);
	}

	private void OnConfigureQuorum(object sender, SnapinActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		SharedActions.PerformConfigureClusterQuorumSettings(cluster, notifyUserFromSender);
	}

	internal void DestoryCluster(object sender, ActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		CluadminWaitDialog cluadminWaitDialog = e.CreateWaitDialog(Resources.DeterimeClusterDestroyStatus_Text);
		bool flag;
		using (cluadminWaitDialog)
		{
			flag = cluadminWaitDialog.ShowDialog<object, bool>(notifyUserFromSender, DetermineClusterDestroyStatus, null);
			if (cluadminWaitDialog.IsCanceled)
			{
				return;
			}
		}
		if (flag)
		{
			if (notifyUserFromSender.ShowWindowsCodePackDialog(Resources.DestroyCluster_ConfrimationDialogCaption, Extensions.FormatCurrentCulture(Resources.ConfirmClusterDestructionFormat_Text, (object)cluster.CachedName), string.Empty) != DialogResult.Yes)
			{
				return;
			}
			string text = string.Format(CultureInfo.InvariantCulture, "{0}.htm", Path.GetTempFileName());
			bool flag2 = false;
			waitDialog = e.CreateWaitDialog(Resources.DestroyingCluster_Text, cluster.CachedName);
			waitDialog.DisplayDelay = new TimeSpan(0L);
			waitDialog.CylonTime = new TimeSpan(0, 1, 0);
			try
			{
				waitDialog.ShowDialog(notifyUserFromSender, (BackgroundWaitDialogOperation<string, object>)PerformClusterDestruction, text);
				if (waitDialog.IsCanceled)
				{
					return;
				}
			}
			catch (Exception)
			{
				flag2 = true;
			}
			finally
			{
				waitDialog.Dispose();
				waitDialog = null;
			}
			if (flag2)
			{
				ShowDestroyReport(notifyUserFromSender, text);
			}
		}
		else
		{
			notifyUserFromSender.ShowWindowsCodePackError(Resources.CannotDestroyClusterContainingNonCoreGroups_Text);
		}
	}

	private void OnUpdateCluster(object sender, SnapinActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		SharedActions.PerformClusterAwareUpdating(cluster, notifyUserFromSender, e.Action.DisplayName);
	}

	private bool DetermineClusterDestroyStatus(CluadminWaitDialog cluadminWaitDialog, object data)
	{
		return OnlyCoreGroupsPresent();
	}

	private static void ShowDestroyReport(INotifyUser notifyUser, string reportFileName)
	{
		if (notifyUser.ShowYesNoQuestionWindowsCodePackDialog(MessageBoxDefaultButton.Button1, Resources.DestroyCluster_Failed_Text, Array.Empty<object>()) == DialogResult.Yes)
		{
			ReportViewer.LaunchReportViewer(reportFileName);
		}
	}

	private string PerformClusterDestruction(CluadminWaitDialog cluadminWaitDialog, string reportFileName)
	{
		cluadminWaitDialog.TotalWork = 110;
		string tempFileName = Path.GetTempFileName();
		ReportBuilder val = ReportBuilder.CreateXmlReportBuilder(tempFileName);
		try
		{
			report = val.PrimaryChannel;
			try
			{
				report.ReportItem((CommonReportItem)22, base.DisplayName);
				cluster.Destroy(cleanupAD: false, DestroyCallback);
			}
			catch (Exception ex)
			{
				ExceptionHelp.LogException(ex, "There was an error destroying the cluster.");
				report.ReportFail(ex.Message);
				throw;
			}
			finally
			{
				report.Close();
				report = null;
				Close();
				Delete();
			}
		}
		finally
		{
			val.Close();
			XmlReportRenderer.TransformStandardHtmlReport(tempFileName, reportFileName);
			try
			{
				File.Delete(tempFileName);
			}
			catch (Exception caughtException)
			{
				ExceptionHelp.LogException(caughtException, "Error deleting {0}", tempFileName);
			}
		}
		return null;
	}

	private bool DestroyCallback(ClusterSetupPhase clusterSetupPhase, ClusterSetupPhaseType phaseType, ClusterSetupPhaseSeverity phaseSeverity, int percentComplete, string objectName, int status)
	{
		if (waitDialog.IsCanceled)
		{
			return false;
		}
		try
		{
			waitDialog.WorkProcessed = percentComplete;
		}
		catch (OperationCanceledException)
		{
			return false;
		}
		if (phaseType == ClusterSetupPhaseType.Start || (phaseType == ClusterSetupPhaseType.End && phaseSeverity != ClusterSetupPhaseSeverity.Informational))
		{
			string message = FormatHelp.FormatPhaseMessage(clusterSetupPhase, objectName, status);
			ReportPhaseMessage(phaseSeverity, message);
		}
		return true;
	}

	private void ReportPhaseMessage(ClusterSetupPhaseSeverity phaseSeverity, string message)
	{
		switch (phaseSeverity)
		{
		case ClusterSetupPhaseSeverity.Informational:
			report.ReportInfo(message);
			break;
		case ClusterSetupPhaseSeverity.Fatal:
			report.ReportFail(message);
			break;
		case ClusterSetupPhaseSeverity.Warning:
			report.ReportWarn(message);
			break;
		default:
			DebugLog.LogWarning("Unknown severity level: " + phaseSeverity);
			break;
		}
	}

	internal void ShutdownCluster(object sender, ActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		if (notifyUserFromSender.ShowWindowsCodePackDialog(Resources.ShutdownCluster_ConfirmationDialogCaption, Extensions.FormatCurrentCulture(Resources.ConfirmClusterShutdownFormat_Text, (object)cluster.CachedName), string.Empty) != DialogResult.Yes)
		{
			return;
		}
		CluadminWaitDialog cluadminWaitDialog = e.CreateWaitDialog(Resources.ShuttingDownCluster_Text, cluster.CachedName);
		cluadminWaitDialog.DisplayDelay = new TimeSpan(0L);
		cluadminWaitDialog.CylonTime = new TimeSpan(0, 1, 0);
		string clusterName = string.Empty;
		ClusterNodeCollection nodes = null;
		try
		{
			MarkForDeletion();
			Close();
			cluadminWaitDialog.ShowDialog(notifyUserFromSender, delegate
			{
				clusterName = Cluster.Name;
				nodes = Cluster.GetNodes();
				PerformShutdownCluster();
				UnregisterCluster();
			});
			_ = cluadminWaitDialog.IsCanceled;
		}
		catch (Exception caughtException)
		{
			ClusterLog.AdminEvents.WriteShutdownErrorEvent(clusterName);
			ExceptionHelp.LogException(caughtException, "Failed shutting down the cluster");
		}
		finally
		{
			cluadminWaitDialog.Dispose();
			if (nodes != null)
			{
				ClusterAdministrator.CreateDownClusterView(new List<string>(nodes.ConvertAll((ClusterNode node) => node.FqdnName)));
			}
		}
	}

	private void PerformShutdownCluster()
	{
		try
		{
			cluster.BeginShutdown();
		}
		finally
		{
			Background.QueueWorker((WaitCallback)delegate
			{
				Delete();
			});
		}
	}

	internal void Close()
	{
		this.Closed.SafeCall(this, new ChildDeletedEventArgs(base.DisplayName));
	}

	public override void Refresh()
	{
		cluster.Refresh();
		resTypeManager.Refresh();
		base.DisplayName = cluster.FqdnName;
		ComputeImageIndex();
		moveActionsAreDirty = true;
		actionsAreDirty = true;
		UpdateStateBasedActions();
		FailoverClusters.Framework.Cluster frameworkCluster = FrameworkCluster;
		if (frameworkCluster != null)
		{
			frameworkCluster.Refresh(targeted: false);
		}
		signalViewModel.SafeCall(ViewModelDataSignal.TargetedRefresh);
	}

	public override void Dispose()
	{
		if (!isDisposed)
		{
			destroyClusterActionPaneItem?.Dispose();
			destroyClusterActionPaneItem = null;
			destroyClusterCommand = null;
			shutdownClusterActionPaneItem?.Dispose();
			shutdownClusterActionPaneItem = null;
			shutdownClusterCommand = null;
			base.Dispose();
			if (!MarkedForDelete)
			{
				UnregisterCluster();
			}
			GC.SuppressFinalize(this);
		}
	}

	private void UnregisterCluster()
	{
		if (cluster != null)
		{
			cluster.PropertiesChanged -= ClusterPropertiesChanged;
			cluster.ConnectionChanged -= ClusterConnectionChanged;
			if (clusterGroup != null)
			{
				clusterGroup.StateChanged -= OnClusterGroupStateChanged;
				clusterGroup = null;
			}
			if (quorumResource != null)
			{
				quorumResource.StateChanged -= OnClusterQuorumStateChanged;
			}
			if (nodeNotifications != null)
			{
				nodeNotifications.StopNotificationMonitoring();
				cluster.NodesChanged -= nodeNotifications.OnCollectionChanged;
				nodeNotifications.Dispose();
			}
			if (clusterEventsMonitor != null)
			{
				clusterEventsMonitor.ClusterEventsChanged -= ClusterEventsChanged;
				clusterEventsMonitor.Close();
				ServiceContainer.Container.Resolve<IClusterEventsMonitorRegistry>(Array.Empty<object>()).RemoveClusterEventsMonitor(FrameworkCluster.CacheId.ToString());
			}
			ClusterConnectionFactory.RemoveClusterContextFromCache(this);
			CloseClusters();
			ClusterAdministrator.Instance.UnregisterShutdownCallback(OnClusterAdministratorShutdown);
		}
	}

	private void MarkForDeletion()
	{
		disposable = false;
	}

	private void Delete()
	{
		disposable = true;
		Dispose();
	}

	protected override void UpdateStateBasedActions()
	{
	}
}

