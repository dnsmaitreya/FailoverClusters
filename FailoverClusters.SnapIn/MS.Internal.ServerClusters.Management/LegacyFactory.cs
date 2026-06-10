using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FailoverClusters.Framework;
using FailoverClusters.SnapIn;
using FailoverClusters.UI.Common;
using ManagementConsole;
using MS.Internal.ServerClusters.Wizards;

namespace MS.Internal.ServerClusters.Management;

public static class LegacyFactory
{
	private struct SetDriveLetterArgs
	{
		public readonly uint PartitionNumber;

		public readonly uint DriveLetterMask;

		public readonly ClusterResource Resource;

		public SetDriveLetterArgs(uint partitionNumber, uint driveLetterMask, ClusterResource resource)
		{
			PartitionNumber = partitionNumber;
			DriveLetterMask = driveLetterMask;
			Resource = resource;
		}
	}

	public static void RunNewResourceWizard(Guid clusterId, string resourceType, string groupName)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		Exceptions.ThrowIfNull((object)groupName, "groupName");
		Exceptions.ThrowIfNull((object)resourceType, "resourceType");
		try
		{
			ClusterGroup legacyGroup = GetLegacyGroup(clusterId, groupName);
			if (legacyGroup != null)
			{
				NewResourceWizard val = new NewResourceWizard();
				try
				{
					val.SetResourceType(resourceType, legacyGroup);
					ClusterAdministrator.NotifyUser.ShowDialog((Form)(object)val);
					return;
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Could not create new wizard form.");
			ClusterAdministrator.NotifyUser.ShowError(ex);
		}
	}

	public static void RunNewReplicationWizard(Guid clusterId, Guid resourceId, Guid replicatedParentResourceId, ReplicationWizardSetup replicationSetup)
	{
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!CheckStorageReplicaFeatureInstall(clusterId, out var missingNodes))
			{
				throw new ClusterReplicationFeatureNotInstalledException(string.Join(Environment.NewLine, from x in missingNodes.OrderBy((string _) => _).Select((string x, int i) => new
					{
						Index = i,
						Value = x
					})
					group x by x.Index / 3 into x
					select string.Join(",", x.Select(v => v.Value))));
			}
			ClusterResource legacyResource = GetLegacyResource(clusterId, resourceId);
			ClusterResource clusterResource = ((replicatedParentResourceId == Guid.Empty) ? null : GetLegacyResource(clusterId, replicatedParentResourceId));
			if (legacyResource != null)
			{
				NewReplicationWizard val = new NewReplicationWizard();
				try
				{
					val.SetParameters(legacyResource, clusterResource, replicationSetup);
					ClusterAdministrator.NotifyUser.ShowDialog((Form)(object)val);
					return;
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Could not create new replication wizard form.");
			if (ex is ClusterDialogException)
			{
				ClusterDialogException.ShowTaskDialogAsync(ex);
			}
			else
			{
				ClusterAdministrator.NotifyUser.ShowError(ex);
			}
		}
	}

	public static string GetResourceDisplayName(ClusterResource clusterResource)
	{
		Exceptions.ThrowIfNull((object)clusterResource, "clusterResource");
		string displayName = null;
		try
		{
			using CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.RetrievingItem_Text, string.Empty);
			cluadminWaitDialog.DisplayDelay = TimeSpan.FromMilliseconds(500.0);
			cluadminWaitDialog.ShowDialog(ClusterAdministrator.NotifyUser, delegate
			{
				displayName = clusterResource.DisplayName;
			});
			if (cluadminWaitDialog.IsCanceled)
			{
				return null;
			}
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Could not get DisplayName from a resource.");
			ClusterAdministrator.NotifyUser.ShowError(ex);
		}
		return displayName;
	}

	private static void GetResourcePropertyPageInfo(Guid clusterId, string resourceName, out ResourceContext resourceCtx, out string resourceTypeName, out bool? isCsv, out bool? isStorageDisk)
	{
		ResourceContext ctx = null;
		string typeName = string.Empty;
		bool isStorageDiskLocal = false;
		bool isCsvLocal = false;
		ClusterResource resource = GetLegacyResource(clusterId, resourceName);
		if (resource != null)
		{
			using CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.RetrievingItem_Text, string.Empty);
			cluadminWaitDialog.DisplayDelay = TimeSpan.FromMilliseconds(500.0);
			cluadminWaitDialog.ShowDialog(ClusterAdministrator.NotifyUser, delegate
			{
				ctx = ContextFactory.CreateContext(resource);
				isStorageDiskLocal = resource.IsStorageDisk();
				isCsvLocal = resource.IsClusterSharedVolume;
				typeName = resource.ResourceTypeName;
			});
			if (cluadminWaitDialog.IsCanceled)
			{
				resourceCtx = null;
				resourceTypeName = null;
				isStorageDisk = null;
				isCsv = null;
				return;
			}
		}
		resourceCtx = ctx;
		resourceTypeName = typeName;
		isCsv = isCsvLocal;
		isStorageDisk = isStorageDiskLocal;
	}

	public static PropertyPage CreateResourceGeneralPropertyPage(Guid clusterId, string resourceName)
	{
		Exceptions.ThrowIfNull((object)resourceName, "resourceName");
		PropertyPage result = null;
		try
		{
			GetResourcePropertyPageInfo(clusterId, resourceName, out var resourceCtx, out var resourceTypeName, out var isCsv, out var isStorageDisk);
			if (resourceCtx != null && !string.IsNullOrEmpty(resourceTypeName) && isCsv.HasValue && isStorageDisk.HasValue)
			{
				result = CreateResourcePropertyPage(resourceCtx, resourceTypeName, isCsv, isStorageDisk);
			}
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Could not build the property page.");
			ClusterAdministrator.NotifyUser.ShowError(ex);
		}
		return result;
	}

	public static PropertyPage CreateResourceRegistryPropertyPage(Guid clusterId, string resourceName)
	{
		Exceptions.ThrowIfNull((object)resourceName, "resourceName");
		PropertyPageControlBase propertyPageControlBase = null;
		try
		{
			GetResourcePropertyPageInfo(clusterId, resourceName, out var resourceCtx, out var resourceTypeName, out var _, out var _);
			if (resourceCtx != null && !string.IsNullOrEmpty(resourceTypeName) && (ClusterResourceType.CompareResourceTypeName(resourceTypeName, WellKnownResourceType.GenericApplication) == 0 || ClusterResourceType.CompareResourceTypeName(resourceTypeName, WellKnownResourceType.GenericService) == 0))
			{
				propertyPageControlBase = new RegistryReplicationPropertiesPage(resourceCtx);
			}
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Could not build the property page.");
			ClusterAdministrator.NotifyUser.ShowError(ex);
		}
		if (propertyPageControlBase != null)
		{
			ClusterPropertyPage clusterPropertyPage = new ClusterPropertyPage();
			clusterPropertyPage.SetControl(propertyPageControlBase);
			return clusterPropertyPage;
		}
		return null;
	}

	public static void ShowResourceDependencyReport(Guid clusterId, string resourceName)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		Exceptions.ThrowIfNull((object)resourceName, "resourceName");
		try
		{
			ClusterResource resource = GetLegacyResource(clusterId, resourceName);
			if (resource == null)
			{
				return;
			}
			string reportFileName = null;
			DependencyWriter writer = new DependencyWriter();
			using (CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.RetrievingItem_Text, string.Empty))
			{
				cluadminWaitDialog.DisplayDelay = TimeSpan.FromMilliseconds(500.0);
				cluadminWaitDialog.ShowDialog(ClusterAdministrator.NotifyUser, delegate
				{
					reportFileName = writer.GenerateReport(resource);
				});
				if (cluadminWaitDialog.IsCanceled)
				{
					return;
				}
			}
			ReportViewer.LaunchReportViewer(reportFileName);
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Could not build the resource dependency report.");
			ClusterAdministrator.NotifyUser.ShowError(ex);
		}
	}

	public static void ShowGroupDependencyReport(Guid clusterId, string groupName)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		Exceptions.ThrowIfNull((object)groupName, "groupName");
		try
		{
			ClusterGroup group = GetLegacyGroup(clusterId, groupName);
			if (group == null)
			{
				return;
			}
			string reportFileName = null;
			DependencyWriter writer = new DependencyWriter();
			using (CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.RetrievingItem_Text, string.Empty))
			{
				cluadminWaitDialog.DisplayDelay = TimeSpan.FromMilliseconds(500.0);
				cluadminWaitDialog.ShowDialog(ClusterAdministrator.NotifyUser, delegate
				{
					reportFileName = writer.GenerateReport(group);
				});
				if (cluadminWaitDialog.IsCanceled)
				{
					return;
				}
			}
			ReportViewer.LaunchReportViewer(reportFileName);
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Could not build the group dependency report.");
			ClusterAdministrator.NotifyUser.ShowError(ex);
		}
	}

	public static void PoolAddDisks(Guid clusterId, string resourceName, Guid filteringPoolId)
	{
		new AddDisksOperation(clusterId, resourceName, filteringPoolId).Execute();
	}

	public static void AddStorageToGroup(Guid clusterId, string groupName)
	{
		Exceptions.ThrowIfNull((object)groupName, "groupName");
		try
		{
			ClusterGroup legacyGroup = GetLegacyGroup(clusterId, groupName);
			if (legacyGroup == null)
			{
				return;
			}
			using CluadminWaitDialog waitDialog = CluadminWaitDialog.Create(Resources.FindingAvailableStorage_Text, string.Empty);
			ICollection<ClusterResource> collection = AddStorage.InvokeDialog(legacyGroup.Cluster, StorageType.All, ClusterAdministrator.NotifyUser, waitDialog);
			if (collection == null)
			{
				return;
			}
			try
			{
				using CluadminWaitDialog waitDialog2 = CluadminWaitDialog.Create(ActionFactory.GenerateDisplayName(StringExtensions.ReplaceAccelerator(CommandResources.Group_AddStorage)), string.Empty);
				AddStorage.AddDiskDelegate addDiskDelegate = legacyGroup.AddDiskToGroup;
				AddStorage.AddSelectedStorage(collection, addDiskDelegate, ClusterAdministrator.NotifyUser, waitDialog2);
			}
			catch (ApplicationException ex)
			{
				ClusterAdministrator.NotifyUser.ShowError((Exception)ex, ex.Message);
			}
		}
		catch (Exception ex2)
		{
			ExceptionHelp.LogException(ex2, "Could not Add Storage to Group.");
			ClusterAdministrator.NotifyUser.ShowError(ex2);
		}
	}

	public static void ShowGroupCriticalEventsDialog(Guid clusterId, string groupName)
	{
		Exceptions.ThrowIfNull((object)groupName, "groupName");
		try
		{
			ClusterGroup legacyGroup = GetLegacyGroup(clusterId, groupName);
			if (legacyGroup != null)
			{
				ExecuteCriticalEventsDialog(legacyGroup.Cluster, groupName, ClusterAdministrator.GroupContextGuid, delegate(EventLogFilter filter)
				{
					filter.ClusterGroup = groupName;
				}, null);
			}
		}
		catch (Exception ex)
		{
			ClusterAdministrator.NotifyUser.ShowError(ex, Resources.CannotQueryClusterObjectEvents_Text, new object[1] { groupName });
		}
	}

	public static void ShowResourceCriticalEventsDialog(Guid clusterId, string resourceName, Guid[] replicationGroupIds)
	{
		Exceptions.ThrowIfNull((object)resourceName, "resourceName");
		try
		{
			ClusterResource legacyResource = GetLegacyResource(clusterId, resourceName);
			if (legacyResource != null)
			{
				ExecuteCriticalEventsDialog(legacyResource.Cluster, GetResourceDisplayName(legacyResource), ClusterAdministrator.ResourceContextGuid, delegate(EventLogFilter filter)
				{
					filter.ClusterResource = resourceName;
				}, replicationGroupIds);
			}
		}
		catch (Exception ex)
		{
			ClusterAdministrator.NotifyUser.ShowError(ex, Resources.CannotQueryClusterObjectEvents_Text, new object[1] { resourceName });
		}
	}

	public static void ShowNetworkCriticalEventsDialog(Guid clusterId, string networkName)
	{
		Exceptions.ThrowIfNull((object)networkName, "networkName");
		try
		{
			ClusterNetwork legacyNetwork = GetLegacyNetwork(clusterId, networkName);
			if (legacyNetwork != null)
			{
				ExecuteCriticalEventsDialog(legacyNetwork.Cluster, networkName, ClusterAdministrator.NetworksContextGuid, delegate(EventLogFilter filter)
				{
					filter.ClusterNetwork = networkName;
				}, null);
			}
		}
		catch (Exception ex)
		{
			ClusterAdministrator.NotifyUser.ShowError(ex, Resources.CannotQueryClusterObjectEvents_Text, new object[1] { networkName });
		}
	}

	public static void ShowNetworkInterfaceCriticalEventsDialog(Guid clusterId, string networkInterfaceName)
	{
		Exceptions.ThrowIfNull((object)networkInterfaceName, "nodeName");
		try
		{
			ClusterNetworkInterface legacyNetworkInterface = GetLegacyNetworkInterface(clusterId, networkInterfaceName);
			if (legacyNetworkInterface != null)
			{
				ExecuteCriticalEventsDialog(legacyNetworkInterface.Cluster, networkInterfaceName, ClusterAdministrator.NetworksContextGuid, delegate(EventLogFilter filter)
				{
					filter.ClusterNetworkInterface = networkInterfaceName;
				}, null);
			}
		}
		catch (Exception ex)
		{
			ClusterAdministrator.NotifyUser.ShowError(ex, Resources.CannotQueryClusterObjectEvents_Text, new object[1] { networkInterfaceName });
		}
	}

	public static void ShowNodeCriticalEventsDialog(Guid clusterId, string nodeName)
	{
		Exceptions.ThrowIfNull((object)nodeName, "nodeName");
		try
		{
			ClusterNode legacyNode = GetLegacyNode(clusterId, nodeName);
			if (legacyNode != null)
			{
				ExecuteCriticalEventsDialog(legacyNode.Cluster, nodeName, ClusterAdministrator.NodeContextGuid, delegate(EventLogFilter filter)
				{
					filter.ClusterNode = nodeName;
				}, includeCauEvents: true, null);
			}
		}
		catch (Exception ex)
		{
			ClusterAdministrator.NotifyUser.ShowError(ex, Resources.CannotQueryClusterObjectEvents_Text, new object[1] { nodeName });
		}
	}

	internal static void ExecuteCriticalEventsDialog(Cluster cluster, string objectName, Guid guid, ClusterAdministrator.SetEventLogFilterDelegate setEventLogFilterDelegate, Guid[] replicationGroupIds)
	{
		ExecuteCriticalEventsDialog(cluster, objectName, guid, setEventLogFilterDelegate, includeCauEvents: false, replicationGroupIds);
	}

	internal static void ExecuteCriticalEventsDialog(Cluster cluster, string objectName, Guid guid, ClusterAdministrator.SetEventLogFilterDelegate setEventLogFilterDelegate, bool includeCauEvents, Guid[] replicationGroupIds)
	{
		INotifyUser notifyUser = ClusterAdministrator.NotifyUser;
		List<string> nodes = new List<string>();
		using (CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.LookingClusterNodes_Text, string.Empty))
		{
			cluadminWaitDialog.ShowDialog(notifyUser, delegate
			{
				nodes.AddRange(from node in cluster.GetNodes()
					select node.Name);
			});
			if (cluadminWaitDialog.IsCanceled)
			{
				return;
			}
		}
		EventLogDialog eventLogDialog = new EventLogDialog();
		((Control)(object)eventLogDialog).Text = Extensions.FormatCurrentCulture(Resources.ClusterObjectEvents_Text, (object)objectName);
		eventLogDialog.SetInstanceId(guid);
		EventLogFilter eventLogFilter = new EventLogFilter();
		eventLogFilter.Nodes.AddRange(nodes);
		eventLogFilter.Channels.Add(EventLog.SystemChannel);
		eventLogFilter.Channels.Add(EventLog.ClusterChannelOperational);
		if (includeCauEvents)
		{
			eventLogFilter.Channels.Add(EventLog.ClusterAwareUpdatingChannelAdmin);
			eventLogFilter.Channels.Add(EventLog.ClusterAwareUpdatingManagementChannelAdmin);
		}
		if (replicationGroupIds != null && replicationGroupIds.Length != 0)
		{
			eventLogFilter.Channels.Add(EventLog.ReplicationChannelAdmin);
			eventLogFilter.Channels.Add(EventLog.ReplicationChannelOperational);
			eventLogFilter.ReplicationGroupIds = replicationGroupIds;
		}
		eventLogFilter.Level = EventLevel.Critical | EventLevel.Error;
		setEventLogFilterDelegate(eventLogFilter);
		eventLogDialog.ExecuteQuery(eventLogFilter, notifyUser);
	}

	private static ClusterGroup GetLegacyGroup(Guid clusterId, string groupName)
	{
		return (ClusterGroup)GetLegacyCluster(clusterId, (Cluster c) => c.GetGroup(groupName));
	}

	private static bool CheckStorageReplicaFeatureInstall(Guid clusterId, out string[] missingNodes)
	{
		bool isStorageReplicaInstalledOnAllNodes = false;
		List<string> missingNodesList = new List<string>();
		using (CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.RetrievingItem_Text, string.Empty))
		{
			cluadminWaitDialog.DisplayDelay = TimeSpan.FromMilliseconds(500.0);
			cluadminWaitDialog.ShowDialog(ClusterAdministrator.NotifyUser, delegate
			{
				ClusterContext clusterContextFromCache = ClusterConnectionFactory.GetClusterContextFromCache(clusterId);
				isStorageReplicaInstalledOnAllNodes = clusterContextFromCache.IsStorageReplicaInstalledOnAllNodes(out var missingNodes2);
				missingNodesList.AddRange(missingNodes2);
			});
			if (cluadminWaitDialog.IsCanceled)
			{
				isStorageReplicaInstalledOnAllNodes = false;
			}
		}
		missingNodes = missingNodesList.ToArray();
		return isStorageReplicaInstalledOnAllNodes;
	}

	private static ClusterItem GetLegacyCluster(Guid clusterId, Func<Cluster, ClusterItem> getClusterItemFromClusterDelegate)
	{
		ClusterItem clusterItem = null;
		using (CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.RetrievingItem_Text, string.Empty))
		{
			cluadminWaitDialog.DisplayDelay = TimeSpan.FromMilliseconds(500.0);
			cluadminWaitDialog.ShowDialog(ClusterAdministrator.NotifyUser, delegate
			{
				ClusterContext clusterContextFromCache = ClusterConnectionFactory.GetClusterContextFromCache(clusterId);
				clusterItem = clusterContextFromCache.Cluster;
				if (getClusterItemFromClusterDelegate != null)
				{
					clusterItem = getClusterItemFromClusterDelegate(clusterContextFromCache.Cluster);
				}
			});
			if (cluadminWaitDialog.IsCanceled)
			{
				return null;
			}
		}
		return clusterItem;
	}

	public static ClusterResource GetLegacyResource(Guid clusterId, Guid resourceId)
	{
		return (ClusterResource)GetLegacyCluster(clusterId, (Cluster c) => c.GetResource(resourceId));
	}

	public static ClusterResource GetLegacyResource(Guid clusterId, string resourceName)
	{
		return (ClusterResource)GetLegacyCluster(clusterId, (Cluster c) => c.GetResource(resourceName));
	}

	public static ClusterNetwork GetLegacyNetwork(Guid clusterId, string networkName)
	{
		return (ClusterNetwork)GetLegacyCluster(clusterId, (Cluster c) => c.GetNetwork(networkName));
	}

	public static ClusterNetworkInterface GetLegacyNetworkInterface(Guid clusterId, string networkInterfaceName)
	{
		return (ClusterNetworkInterface)GetLegacyCluster(clusterId, (Cluster c) => c.GetNetworkInterface(networkInterfaceName));
	}

	public static ClusterNode GetLegacyNode(Guid clusterId, string nodeName)
	{
		return (ClusterNode)GetLegacyCluster(clusterId, (Cluster c) => c.GetNode(nodeName));
	}

	private static PropertyPage CreateResourcePropertyPage(ResourceContext ctx, string resourceTypeName, bool? isCsv, bool? isStorageDisk)
	{
		PropertyPageControlBase control;
		if (ClusterResourceType.CompareResourceTypeName(resourceTypeName, WellKnownResourceType.NetName) == 0)
		{
			control = new ClientAccessPointGeneralPropertiesPage(ctx);
		}
		else if (ClusterResourceType.CompareResourceTypeName(resourceTypeName, WellKnownResourceType.DistributedNetworkName) == 0)
		{
			control = new DistributedNetworkNamePropertiesPage(ctx);
		}
		else if (ClusterResourceType.CompareResourceTypeName(resourceTypeName, WellKnownResourceType.IPAddress) == 0)
		{
			control = new IpAddressGeneralPropertiesPage(ctx);
		}
		else if (ClusterResourceType.CompareResourceTypeName(resourceTypeName, WellKnownResourceType.IPv6Address) == 0)
		{
			control = new IPv6AddressGeneralPropertiesPage(ctx);
		}
		else if (ClusterResourceType.CompareResourceTypeName(resourceTypeName, WellKnownResourceType.IPv6TunnelAddress) == 0)
		{
			control = new IPv6TunnelAddressGeneralPropertiesPage(ctx);
		}
		else if (ClusterResourceType.CompareResourceTypeName(resourceTypeName, WellKnownResourceType.DhcpService) == 0)
		{
			control = new DhcpServiceGeneralPropertiesPage(ctx);
		}
		else if (ClusterResourceType.CompareResourceTypeName(resourceTypeName, WellKnownResourceType.WinsService) == 0)
		{
			control = new WinsServiceGeneralPropertiesPage(ctx);
		}
		else if (ClusterResourceType.CompareResourceTypeName(resourceTypeName, WellKnownResourceType.GenericApplication) == 0)
		{
			control = new GenericApplicationGeneralPropertiesPage(ctx);
		}
		else if (ClusterResourceType.CompareResourceTypeName(resourceTypeName, WellKnownResourceType.GenericService) == 0)
		{
			control = new GenericServiceGeneralPropertiesPage(ctx);
		}
		else if (ClusterResourceType.CompareResourceTypeName(resourceTypeName, WellKnownResourceType.GenericScript) == 0)
		{
			control = new GenericScriptGeneralPropertiesPage(ctx);
		}
		else if (ClusterResourceType.CompareResourceTypeName(resourceTypeName, WellKnownResourceType.DfsReplicatedFolder) == 0)
		{
			control = new DfsrGeneralPropertiesPage(ctx);
		}
		else
		{
			if (!isStorageDisk.HasValue || !isStorageDisk.Value)
			{
				return null;
			}
			control = ((!isCsv.HasValue || !isCsv.Value) ? ((ResourceGeneralPropertiesPage)new PhysicalDiskGeneralPropertiesPage(ctx)) : ((ResourceGeneralPropertiesPage)new ClusterSharedVolumeGeneralProperitesPage(ctx)));
		}
		ClusterPropertyPage clusterPropertyPage = new ClusterPropertyPage();
		clusterPropertyPage.SetControl(control);
		return clusterPropertyPage;
	}

	public static void ChangeDriveLetter(Guid clusterId, string resourceName, uint partitionNumber)
	{
		Exceptions.ThrowIfNull((object)resourceName, "nodeName");
		try
		{
			INotifyUser notifyUser = ClusterAdministrator.NotifyUser;
			ClusterResource resource = GetLegacyResource(clusterId, resourceName);
			if (resource == null)
			{
				return;
			}
			ClusterDisk disk = null;
			ClusterGroup ownerGroup = null;
			uint availableDriveLettersMask;
			using (CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(StringExtensions.RemoveAccelerator(CommandResources.ChangeDriveLetterAction_Text), Resources.LookingForAvailableDriveLetters_Text))
			{
				try
				{
					availableDriveLettersMask = cluadminWaitDialog.ShowDialog<object, uint>(notifyUser, delegate
					{
						ownerGroup = resource.GetOwnerGroup();
						disk = resource.Storage_GetDiskInfo(includeMountPoints: false);
						return ClusterUtilities.GetAvailableDriveLettersMask(ClusterConnectionFactory.GetClusterContextFromCache(clusterId).Cluster);
					}, null);
				}
				catch (ClusterAvailableDriveLettersNodeDownException ex)
				{
					notifyUser.ShowError((Exception)ex);
					return;
				}
				if (cluadminWaitDialog.IsCanceled)
				{
					return;
				}
			}
			ClusterDiskPartition clusterDiskPartition = disk.Partitions.FirstOrDefault((ClusterDiskPartition p) => partitionNumber == uint.MaxValue || p.PartitionNumber == partitionNumber);
			if (clusterDiskPartition == null)
			{
				throw new InvalidOperationException("The disk has no partitions");
			}
			DriveLetterDialog driveLetterDialog = new DriveLetterDialog(ownerGroup, clusterDiskPartition.DriveLetterMask, availableDriveLettersMask);
			if (notifyUser.ShowDialog((Form)(object)driveLetterDialog) != DialogResult.OK)
			{
				return;
			}
			using CluadminWaitDialog cluadminWaitDialog2 = CluadminWaitDialog.Create(StringExtensions.RemoveAccelerator(CommandResources.ChangeDriveLetterAction_Text), Resources.ChangingDriveLetter_Text);
			cluadminWaitDialog2.ShowDialog(notifyUser, SetDriveLetter, new SetDriveLetterArgs(clusterDiskPartition.PartitionNumber, driveLetterDialog.DriveLetterMask, resource));
		}
		catch (Exception ex2)
		{
			ExceptionHelp.LogException(ex2, "Could not build the resource dependency report.");
			ClusterAdministrator.NotifyUser.ShowError(ex2);
		}
	}

	private static object SetDriveLetter(CluadminWaitDialog waitDialog, SetDriveLetterArgs args)
	{
		try
		{
			args.Resource.Storage_SetDriveLetter(args.PartitionNumber, args.DriveLetterMask);
		}
		catch (ApplicationException ex)
		{
			ExceptionHelp.LogException(ex, "Could not set drive letter.");
			ClusterAdministrator.NotifyUser.ShowError((Exception)ex);
		}
		return null;
	}

	public static void RepairStorage(Guid clusterId, string resourceName)
	{
		Exceptions.ThrowIfNull((object)resourceName, "nodeName");
		try
		{
			ClusterResource resource = GetLegacyResource(clusterId, resourceName);
			if (resource == null)
			{
				return;
			}
			string realResourceName = resourceName;
			using (CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.RetrievingResourceName_Text, Resources.RepairDiskWaitDialogInitialState_Text))
			{
				cluadminWaitDialog.DisplayDelay = TimeSpan.FromSeconds(5.0);
				cluadminWaitDialog.ShowDialog(ClusterAdministrator.NotifyUser, delegate
				{
					realResourceName = resource.DisplayName;
				});
				if (cluadminWaitDialog.IsCanceled)
				{
					return;
				}
			}
			ClusterableDisks clusterableDisks = AddDiskDialog.RepairDiskDialog(resource.Cluster, ClusterAdministrator.NotifyUser, realResourceName);
			if (clusterableDisks == null || clusterableDisks.AvailableDisks.Count == 0)
			{
				return;
			}
			using (CluadminWaitDialog cluadminWaitDialog2 = CluadminWaitDialog.Create(Extensions.FormatCurrentCulture(Resources.RepairDiskWaitDialogTitleFormat_Text, (object)realResourceName), Resources.RepairDiskWaitDialogInitialState_Text))
			{
				cluadminWaitDialog2.DisplayDelay = new TimeSpan(0L);
				cluadminWaitDialog2.ShowDialog(ClusterAdministrator.NotifyUser, delegate
				{
					foreach (ClusterDisk availableDisk in clusterableDisks.AvailableDisks)
					{
						availableDisk.ConfigureDiskResource(resource);
					}
				});
				if (cluadminWaitDialog2.IsCanceled)
				{
					return;
				}
			}
			ClusterAdministrator.NotifyUser.ShowInformational(Extensions.FormatCurrentCulture(Resources.RepairDiskCompleteMessageFormat_Text, (object)realResourceName));
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Could not remove the storage resource.");
			ClusterAdministrator.NotifyUser.ShowError(ex);
		}
	}

	public static void ConfigureRole(Guid clusterId)
	{
		SharedActions.PerformNewServiceOrApplicationAction((Cluster)GetLegacyCluster(clusterId, null), ClusterAdministrator.NotifyUser);
	}

	public static void ValidateClusterConfiguration(Guid clusterId)
	{
		SharedActions.PerformValidateClusterConfiguration((Cluster)GetLegacyCluster(clusterId, null), ClusterAdministrator.NotifyUser);
	}

	public static void ViewValidationReport(Guid clusterId)
	{
		SharedActions.PerformViewValidationReportAction((Cluster)GetLegacyCluster(clusterId, null), ClusterAdministrator.NotifyUser, Resources.ViewValidationReportWaitDialog_Text);
	}

	public static void AddNode(Guid clusterId)
	{
		SharedActions.PerformAddNodesAction((Cluster)GetLegacyCluster(clusterId, null), ClusterAdministrator.NotifyUser);
	}

	public static void CloseConnection(Guid clusterId)
	{
		SharedActions.FindClusterContext((Cluster)GetLegacyCluster(clusterId, null))?.Close();
	}

	public static void ConfigureClusterQuorumSettings(Guid clusterId)
	{
		SharedActions.PerformConfigureClusterQuorumSettings((Cluster)GetLegacyCluster(clusterId, null), ClusterAdministrator.NotifyUser);
	}

	public static void CopyClusterRoles(Guid clusterId)
	{
		SharedActions.PerformCopyClusterRoles((Cluster)GetLegacyCluster(clusterId, null), ClusterAdministrator.NotifyUser);
	}

	public static void ClusterAwareUpdating(Guid clusterId)
	{
		SharedActions.PerformClusterAwareUpdating((Cluster)GetLegacyCluster(clusterId, null), ClusterAdministrator.NotifyUser, Resources.UpdateCluster_Description);
	}
}

