using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace KDDSL.FailoverClusters.Framework;

internal class PResourceType : PClusterObject<ResourceType>
{
	private static readonly ConcurrentDictionary<string, ResourceKind> StringResourceKindDictionary;

	private static readonly ConcurrentDictionary<ResourceKind, string> ResourceKindStringDictionary;

	private ResourceType proxyResourceType;

	public override ClusterIdentityType IdentityType => ClusterIdentityType.ResourceType;

	public PResourceType ActualResourceType { get; private set; }

	public bool? IsStorage { get; set; }

	public ResourceKind ResourceKind { get; set; }

	public ResourceClass Class { get; set; }

	public ResourceSubclass Subclass { get; set; }

	[DefaultValue(null)]
	public Characteristics? Characteristics { get; set; }

	public List<Guid> PossibleOwners { get; internal set; }

	static PResourceType()
	{
		StringResourceKindDictionary = new ConcurrentDictionary<string, ResourceKind>(StringComparer.OrdinalIgnoreCase);
		ResourceKindStringDictionary = new ConcurrentDictionary<ResourceKind, string>();
		StringResourceKindDictionary.TryAdd("Generic Application", ResourceKind.GenericApplication);
		StringResourceKindDictionary.TryAdd("Generic Service", ResourceKind.GenericService);
		StringResourceKindDictionary.TryAdd("Generic Script", ResourceKind.GenericScript);
		StringResourceKindDictionary.TryAdd("IP Address", ResourceKind.IPAddress);
		StringResourceKindDictionary.TryAdd("Network Name", ResourceKind.NetworkName);
		StringResourceKindDictionary.TryAdd("Distributed Network Name", ResourceKind.DistributedNetworkName);
		StringResourceKindDictionary.TryAdd("IPv6 Address", ResourceKind.IPv6Address);
		StringResourceKindDictionary.TryAdd("IPv6 Tunnel Address", ResourceKind.IPv6TunnelAddress);
		StringResourceKindDictionary.TryAdd("Volume Shadow Copy Service Task", ResourceKind.VolumeShadowCopyServiceTask);
		StringResourceKindDictionary.TryAdd("WINS Service", ResourceKind.WinsService);
		StringResourceKindDictionary.TryAdd("DHCP Service", ResourceKind.DhcpService);
		StringResourceKindDictionary.TryAdd("MSMQ", ResourceKind.Msmsq);
		StringResourceKindDictionary.TryAdd("MSMQTriggers", ResourceKind.MsmsqTrigger);
		StringResourceKindDictionary.TryAdd("Distributed Transaction Coordinator", ResourceKind.Dtc);
		StringResourceKindDictionary.TryAdd("NFS Share", ResourceKind.NfsShare);
		StringResourceKindDictionary.TryAdd("iSNS", ResourceKind.IScsiNameService);
		StringResourceKindDictionary.TryAdd("iSCSI Target Server", ResourceKind.IScsiTarget);
		StringResourceKindDictionary.TryAdd("cluster file system", ResourceKind.ClusterFileSystem);
		StringResourceKindDictionary.TryAdd("Physical Disk", ResourceKind.PhysicalDisk);
		StringResourceKindDictionary.TryAdd("File Share Witness", ResourceKind.FileShareWitness);
		StringResourceKindDictionary.TryAdd("Cloud Witness", ResourceKind.CloudWitness);
		StringResourceKindDictionary.TryAdd("File Server", ResourceKind.FileServer);
		StringResourceKindDictionary.TryAdd("Scale Out File Server", ResourceKind.ScaleOutFileServer);
		StringResourceKindDictionary.TryAdd("Distributed File System", ResourceKind.DistributedFileSystem);
		StringResourceKindDictionary.TryAdd("DFS Replicated Folder", ResourceKind.DfsReplicatedFolder);
		StringResourceKindDictionary.TryAdd("Virtual Machine", ResourceKind.VirtualMachine);
		StringResourceKindDictionary.TryAdd("Virtual Machine Replication Broker", ResourceKind.VirtualMachineReplicationBroker);
		StringResourceKindDictionary.TryAdd("Virtual Machine Configuration", ResourceKind.VirtualMachineConfiguration);
		StringResourceKindDictionary.TryAdd("Network File System", ResourceKind.NetworkFileSystem);
		StringResourceKindDictionary.TryAdd("Storage Pool", ResourceKind.StoragePool);
		StringResourceKindDictionary.TryAdd("Task Scheduler", ResourceKind.TaskScheduler);
		StringResourceKindDictionary.TryAdd("clusterawareupdatingresource", ResourceKind.ClusterAwareUpdating);
		StringResourceKindDictionary.TryAdd("Provider Address", ResourceKind.HyperVNetworkVirtualizationProviderAddress);
		StringResourceKindDictionary.TryAdd("Nat", ResourceKind.NetworkAddressTranslator);
		StringResourceKindDictionary.TryAdd("Disjoint IPv4 Address", ResourceKind.DisjointIPv4Address);
		StringResourceKindDictionary.TryAdd("Disjoint IPv6 Address", ResourceKind.DisjointIPv6Address);
		StringResourceKindDictionary.TryAdd("Storage Replica", ResourceKind.StorageReplica);
		StringResourceKindDictionary.TryAdd("Health Service", ResourceKind.HealthService);
		StringResourceKindDictionary.TryAdd("Virtual Machine Cluster WMI", ResourceKind.HyperVClusterWmi);
		StringResourceKindDictionary.TryAdd("Storage QoS Policy Manager", ResourceKind.StorageQoS);
		StringResourceKindDictionary.TryAdd("Virtual Machine Replication Coordinator", ResourceKind.VirtualMachineReplicationCoordinator);
		StringResourceKindDictionary.TryAdd("SDDC Management", ResourceKind.SDDCManagement);
		StringResourceKindDictionary.TryAdd("Cross Cluster Dependency Orchestrator", ResourceKind.CrossClusterOrchestrator);
		StringResourceKindDictionary.TryAdd("Other", ResourceKind.Other);
		StringResourceKindDictionary.ForEach(delegate(KeyValuePair<string, ResourceKind> keyValuePair)
		{
			ResourceKindStringDictionary.TryAdd(keyValuePair.Value, keyValuePair.Key);
		});
	}

	public PResourceType(PCluster cluster, ResourceKind resourceKind, PResourceType actualResourceType = null)
		: base(cluster)
	{
		switch (resourceKind)
		{
		case ResourceKind.Other:
			throw new ArgumentException("resourceKind cannot be third party resource for this call");
		case ResourceKind.ClusterFileSystem:
			if (actualResourceType == null)
			{
				throw new ArgumentException("resourceKind CsvVolume must also supply actualResoruceType for this call");
			}
			break;
		}
		base.Name = ResourceKindToString(resourceKind);
		ResourceKind = resourceKind;
		base.Id = IdFromName(base.Name);
		ActualResourceType = actualResourceType;
		if (actualResourceType != null)
		{
			Class = actualResourceType.Class;
			Characteristics = actualResourceType.Characteristics;
		}
	}

	public PResourceType(PCluster cluster, string resourceTypeName)
		: base(cluster)
	{
		base.Name = resourceTypeName;
		ResourceKind = StringToResourceKind(resourceTypeName);
		base.Id = IdFromName(base.Name);
	}

	public override void Rename(string newName)
	{
	}

	public override void Delete()
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.ResourceType.Delete(base.Name);
			});
		});
	}

	public override ResourceType GetProxy()
	{
		return GetProxy(ProxyCreateMode.Singleton);
	}

	public override ResourceType GetProxy(ProxyCreateMode createMode)
	{
		if (createMode == ProxyCreateMode.Singleton && proxyResourceType != null)
		{
			return proxyResourceType;
		}
		ResourceType resourceType = new ResourceType(base.Cluster.GetProxy(), ResourceKind);
		resourceType.TransferInternalData(this, subscribeToEvents: true);
		if (ActualResourceType != null)
		{
			resourceType.ActualResourceType = new ResourceType(base.Cluster.GetProxy(), ActualResourceType.ResourceKind);
			resourceType.ActualResourceType.TransferInternalData(ActualResourceType, subscribeToEvents: true);
		}
		if (createMode == ProxyCreateMode.Singleton)
		{
			proxyResourceType = resourceType;
		}
		return resourceType;
	}

	public IEnumerable<string> GetPossibleOwners()
	{
		return ProtectedScope(() => ReleaseExecuteAndReacquire(() => base.Cluster.Server.ResourceType.GetPossibleOwners(base.Name)));
	}

	public override ClusterLoadedEventArgs LoadObject(int loadSelectionNeutral)
	{
		if ((base.LoadedSelection & loadSelectionNeutral) == loadSelectionNeutral)
		{
			return new ClusterLoadedEventArgs(base.Id, loaded: true, base.LoadedSelection, null);
		}
		ClusterLoadedEventArgs loadedArgs = null;
		int currentSelection = base.LoadedSelection;
		ResourceTypeLoadSelection loadSelection = (ResourceTypeLoadSelection)loadSelectionNeutral;
		ProtectedScope(delegate
		{
			if (ResourceKind == ResourceKind.ClusterFileSystem)
			{
				base.Name = ActualResourceType.Name;
				try
				{
					base.Cluster.Server.ResourceType.Load(this, loadSelection);
					return;
				}
				finally
				{
					base.Name = ResourceKindToString(ResourceKind.ClusterFileSystem);
				}
			}
			base.Cluster.Server.ResourceType.Load(this, loadSelection);
		}, delegate(ClusterException ex)
		{
			if (ex == null)
			{
				int loadSelection2 = currentSelection ^ base.LoadedSelection;
				BroadcastChanges(loadSelection2);
			}
			loadedArgs = new ClusterLoadedEventArgs(base.Id, ex == null, base.LoadedSelection, ex);
			RouteEvent(new ClusterWrapperEventArgs(EventType.Loaded, loadedArgs));
		});
		return loadedArgs;
	}

	public override void BroadcastChanges(int loadSelection, bool raiseLoadedEvent = false)
	{
		List<ClusterWrapperEventArgs> list = new List<ClusterWrapperEventArgs>(2);
		if (((ResourceTypeLoadSelection)loadSelection).HasFlag(ResourceTypeLoadSelection.Basic))
		{
			list.AddRange(new ClusterWrapperEventArgs[1]
			{
				new ClusterWrapperEventArgs(EventType.ResourceTypeIsStorageChanged, new ClusterResourceTypeIsStorageEventArgs(base.Id, IsStorage, null))
			});
		}
		if (((ResourceTypeLoadSelection)loadSelection).HasFlag(ResourceTypeLoadSelection.CommonProperties) || ((ResourceTypeLoadSelection)loadSelection).HasFlag(ResourceTypeLoadSelection.PrivateProperties))
		{
			list.Add(new ClusterWrapperEventArgs(EventType.PropertiesChanged, new ClusterPropertiesEventArgs(base.Id, base.Name, null, null)
			{
				Properties = base.Properties
			}));
		}
		if (((ResourceTypeLoadSelection)loadSelection).HasFlag(ResourceTypeLoadSelection.PossibleOwners))
		{
			list.Add(new ClusterWrapperEventArgs(EventType.ResourceTypePossibleOwnersChanged, new ClusterResourceTypePossibleOwnersChangedEventArgs(base.Id, PossibleOwners, null)));
		}
		if (raiseLoadedEvent)
		{
			ClusterLoadedEventArgs eventArgs = new ClusterLoadedEventArgs(base.Id, loaded: true, base.LoadedSelection, null);
			list.Add(new ClusterWrapperEventArgs(EventType.Loaded, eventArgs));
		}
		RouteEvent(new ClusterWrapperEventArgs(EventType.BatchChanges, new ClusterBatchChangesEventArgs(base.Id, list)));
	}

	public override void TransferFrom(PClusterObject source, bool cacheIsLocked, int loadSelection)
	{
		PResourceType sourceObject = source as PResourceType;
		if (sourceObject == null)
		{
			throw new InvalidOperationException("Source and Target must be the same type: ".FormatCurrentCulture(GetType()));
		}
		LockTransferAndBroadCast(source, loadSelection, delegate
		{
			ResourceTypeLoadSelection resourceTypeLoadSelection = (ResourceTypeLoadSelection)loadSelection;
			if (resourceTypeLoadSelection.HasFlag(ResourceTypeLoadSelection.Basic))
			{
				Characteristics = sourceObject.Characteristics;
				Class = sourceObject.Class;
				Subclass = sourceObject.Subclass;
				IsStorage = sourceObject.IsStorage;
				base.LoadedSelection |= 1;
			}
		});
	}

	public override List<Action> ProcessNotification(Notification notification)
	{
		List<Action> executeOnReaderCallbacks = base.ProcessNotification(notification);
		ClusterResourceTypePossibleOwnersChangedEventArgs possibleOwnersArgs = notification.Payload as ClusterResourceTypePossibleOwnersChangedEventArgs;
		if (possibleOwnersArgs != null)
		{
			bool flag = true;
			if (PossibleOwners == null || PossibleOwners.Count != possibleOwnersArgs.PossibleNodes.Count)
			{
				flag = false;
			}
			else if (PossibleOwners.Where((Guid t, int i) => t != possibleOwnersArgs.PossibleNodes[i]).Any())
			{
				flag = false;
			}
			if (!flag)
			{
				PossibleOwners = possibleOwnersArgs.PossibleNodes;
				ClusterWrapperEventArgs wrapperEvent = new ClusterWrapperEventArgs(EventType.ResourceTypePossibleOwnersChanged, new ClusterResourceTypePossibleOwnersChangedEventArgs(base.Id, PossibleOwners, null));
				executeOnReaderCallbacks.Add(delegate
				{
					RouteEvent(wrapperEvent);
				});
			}
		}
		ClusterRemovedEventArgs clusterRemovedEventArgs = notification.Payload as ClusterRemovedEventArgs;
		if (clusterRemovedEventArgs != null)
		{
			clusterRemovedEventArgs.Cluster.CacheManager.CacheLock.EnterWriteLock();
			try
			{
				base.IsRemoved = true;
				clusterRemovedEventArgs.Cluster.CacheManager.RemoveObject(this);
				executeOnReaderCallbacks.Add(delegate
				{
					RouteEvent(new ClusterWrapperEventArgs(EventType.Removed, notification.Payload));
					clusterRemovedEventArgs.Cluster.RealtimeCollections.Remove<ResourceType>(this);
					clusterRemovedEventArgs.Cluster.SendEventToProxy(new ClusterWrapperEventArgs(EventType.ResourceTypeChanged, clusterRemovedEventArgs));
				});
			}
			finally
			{
				clusterRemovedEventArgs.Cluster.CacheManager.CacheLock.ExitWriteLock();
			}
		}
		if (notification.Payload is ClusterResourceTypeReplicationEventArgs clusterResourceTypeReplicationEventArgs)
		{
			clusterResourceTypeReplicationEventArgs.Cluster.CacheManager.CacheLock.EnterWriteLock();
			try
			{
				CacheManager cacheManager = clusterResourceTypeReplicationEventArgs.Cluster.CacheManager;
				ClusterResourceTypeReplicationGroupDeletedEventArgs replicationDeleted = clusterResourceTypeReplicationEventArgs as ClusterResourceTypeReplicationGroupDeletedEventArgs;
				if (replicationDeleted != null)
				{
					List<ReplicationGroupInfo> replicationGroupsInCache = (from _ in cacheManager.ReplicatedGroups
						where _.Value.ReplicationGroupId == replicationDeleted.Payload.ReplicationGroupId
						select _.Value).ToList();
					List<NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK> replicationResourcesInCache2 = new List<NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK>();
					foreach (ReplicationGroupInfo item in replicationGroupsInCache)
					{
						cacheManager.ReplicatedGroups.TryRemove(item.ClusterGroupId, out var _);
						foreach (NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK item2 in from _ in cacheManager.ReplicatedResources
							where _.Value.ReplicationGroupId == replicationDeleted.Payload.ReplicationGroupId
							select _.Value)
						{
							if (cacheManager.ReplicatedResources.TryRemove(item2.ClusterResourceId, out var value3))
							{
								replicationResourcesInCache2.Add(value3);
							}
						}
					}
					executeOnReaderCallbacks.Add(delegate
					{
						foreach (ReplicationGroupInfo item3 in replicationGroupsInCache)
						{
							using ClusterLock clusterLock6 = cacheManager.Get(item3.ClusterGroupId, ClusterIdentityType.Group, LockAccess.Reader);
							if (clusterLock6 != null && clusterLock6.Owner is PGroup pGroup3)
							{
								pGroup3.ReloadReplicationRole();
							}
						}
						foreach (NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK item4 in replicationResourcesInCache2)
						{
							using ClusterLock clusterLock7 = cacheManager.Get(item4.ClusterResourceId, ClusterIdentityType.Resource, LockAccess.Reader);
							if (clusterLock7 != null && clusterLock7.Owner is PResource pResource3)
							{
								pResource3.ReloadReplicationRole();
							}
						}
					});
				}
				ClusterResourceTypeReplicationGroupModifiedEventArgs replicationModified = clusterResourceTypeReplicationEventArgs as ClusterResourceTypeReplicationGroupModifiedEventArgs;
				if (replicationModified != null)
				{
					ReplicationGroupInfo replicationGroupInfo = new ReplicationGroupInfo(replicationModified.Payload.ClusterGroupId, replicationModified.Payload.ReplicationGroupId, (ReplicationGroupRole)replicationModified.Payload.GroupType);
					cacheManager.ReplicatedGroups.AddOrUpdate(replicationGroupInfo.ClusterGroupId, replicationGroupInfo, (Guid key, ReplicationGroupInfo value) => replicationGroupInfo);
					ConcurrentDictionary<Guid, NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK> replicationResourcesInCacheToRemove = new ConcurrentDictionary<Guid, NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK>(from _ in cacheManager.ReplicatedResources
						where _.Value.ReplicationGroupId == replicationModified.Payload.ReplicationGroupId
						select (_));
					foreach (NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK resourceTypeReplicatedDisk2 in replicationModified.ReplicatedDisks)
					{
						cacheManager.ReplicatedResources.AddOrUpdate(resourceTypeReplicatedDisk2.ClusterResourceId, resourceTypeReplicatedDisk2, (Guid key, NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK value) => resourceTypeReplicatedDisk2);
						replicationResourcesInCacheToRemove.TryRemove(resourceTypeReplicatedDisk2.ClusterResourceId, out var _);
					}
					foreach (KeyValuePair<Guid, NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK> item5 in replicationResourcesInCacheToRemove)
					{
						cacheManager.ReplicatedResources.TryRemove(item5.Key, out var _);
					}
					executeOnReaderCallbacks.Add(delegate
					{
						using ClusterLock clusterLock5 = cacheManager.Get(replicationGroupInfo.ClusterGroupId, ClusterIdentityType.Group, LockAccess.Reader);
						if (clusterLock5 != null && clusterLock5.Owner is PGroup pGroup2)
						{
							pGroup2.ReloadReplicationRole();
						}
					});
					executeOnReaderCallbacks.Add(delegate
					{
						foreach (NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK item6 in replicationModified.ReplicatedDisks.Union(replicationResourcesInCacheToRemove.Values))
						{
							using ClusterLock clusterLock4 = cacheManager.Get(item6.ClusterResourceId, ClusterIdentityType.Resource, LockAccess.Reader);
							if (clusterLock4 != null && clusterLock4.Owner is PResource pResource2)
							{
								pResource2.ReloadReplicationRole();
							}
						}
					});
				}
				ClusterResourceTypeReplicationPartnershipEventArgs partnership = clusterResourceTypeReplicationEventArgs as ClusterResourceTypeReplicationPartnershipEventArgs;
				if (partnership != null && (partnership.EventType == NativeMethods.WVR_EVENT_TYPE.WvrEventTypePartnershipCreated || partnership.EventType == NativeMethods.WVR_EVENT_TYPE.WvrEventTypeRoleSwitched))
				{
					Action<Guid, ReplicationGroupRole> obj = delegate(Guid groupId, ReplicationGroupRole groupRole)
					{
						if (cacheManager.ReplicatedGroups.TryGetValue(groupId, out var value6))
						{
							ReplicationGroupInfo replicationGroupInfo2 = new ReplicationGroupInfo(groupId, value6.ReplicationGroupId, groupRole);
							cacheManager.ReplicatedGroups.AddOrUpdate(replicationGroupInfo2.ClusterGroupId, replicationGroupInfo2, (Guid key, ReplicationGroupInfo value) => replicationGroupInfo2);
							executeOnReaderCallbacks.Add(delegate
							{
								using ClusterLock clusterLock3 = cacheManager.Get(replicationGroupInfo2.ClusterGroupId, ClusterIdentityType.Group, LockAccess.Reader);
								if (clusterLock3 != null && clusterLock3.Owner is PGroup pGroup)
								{
									pGroup.ReloadReplicationRole();
								}
							});
						}
					};
					obj(partnership.Payload.SourceClusterGroupId, (ReplicationGroupRole)partnership.Payload.SourceGroupType);
					obj(partnership.Payload.TargetClusterGroupId, (ReplicationGroupRole)partnership.Payload.TargetGroupType);
					foreach (NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK resourceTypeReplicatedDisk in partnership.ReplicatedDisks)
					{
						cacheManager.ReplicatedResources.AddOrUpdate(resourceTypeReplicatedDisk.ClusterResourceId, resourceTypeReplicatedDisk, (Guid key, NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK value) => resourceTypeReplicatedDisk);
					}
					executeOnReaderCallbacks.Add(delegate
					{
						foreach (NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK replicatedDisk in partnership.ReplicatedDisks)
						{
							using ClusterLock clusterLock2 = cacheManager.Get(replicatedDisk.ClusterResourceId, ClusterIdentityType.Resource, LockAccess.Reader);
							if (clusterLock2 != null && clusterLock2.Owner is PResource pResource)
							{
								pResource.ReloadReplicationRole();
							}
						}
					});
				}
				ClusterResourceTypeReplicationStateEventArgs replicationState = clusterResourceTypeReplicationEventArgs as ClusterResourceTypeReplicationStateEventArgs;
				if (replicationState != null)
				{
					List<ReplicationGroupInfo> source = (from _ in cacheManager.ReplicatedGroups
						where _.Value.ReplicationGroupId == replicationState.Payload.ReplicationGroupId
						select _.Value).ToList();
					List<NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK> replicationResourcesInCache = source.SelectMany((ReplicationGroupInfo groupInfo) => from _ in cacheManager.ReplicatedResources
						where _.Value.ReplicationGroupId == groupInfo.ReplicationGroupId
						select _.Value).ToList();
					executeOnReaderCallbacks.Add(delegate
					{
						foreach (NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK item7 in replicationResourcesInCache)
						{
							using ClusterLock clusterLock = cacheManager.Get(item7.ClusterResourceId, ClusterIdentityType.Resource, LockAccess.Reader);
							if (clusterLock != null && clusterLock.Owner is PStorageResource pStorageResource)
							{
								IList<ReplicationStatusInfo> replicationStatus = pStorageResource.ReplicationStatus;
								if (replicationStatus != null)
								{
									ReplicationStatusInfo replicationStatusInfo = replicationStatus.FirstOrDefault((ReplicationStatusInfo _) => _.PartitionId == replicationState.Payload.PartitionId);
									if (replicationStatusInfo != null)
									{
										ReplicationStatus replicationState2 = (ReplicationStatus)replicationState.Payload.ReplicationState;
										replicationStatusInfo.ReplicationStatus = replicationState2;
										replicationStatusInfo.PercentageRecovered = replicationState.Payload.PercentageRecovered;
										pStorageResource.ReplicationStatus = pStorageResource.ReplicationStatus;
									}
								}
							}
						}
					});
				}
			}
			finally
			{
				clusterResourceTypeReplicationEventArgs.Cluster.CacheManager.CacheLock.ExitWriteLock();
			}
		}
		return executeOnReaderCallbacks;
	}

	public static bool ProcessNotificationSpecial(Notification notification)
	{
		if (notification.Payload is ClusterAddedEventArgs clusterAddedEventArgs)
		{
			PResourceType clusterObject = new PResourceType(clusterAddedEventArgs.Cluster, clusterAddedEventArgs.Name);
			clusterObject = (PResourceType)clusterAddedEventArgs.Cluster.CacheManager.AddObject(clusterObject);
			clusterObject.LockObject.Reader();
			try
			{
				clusterAddedEventArgs.Cluster.RealtimeCollections.Add(clusterObject, RTCOperation.Add);
				clusterAddedEventArgs.Cluster.SendEventToProxy(new ClusterWrapperEventArgs(EventType.ResourceTypeChanged, clusterAddedEventArgs));
			}
			finally
			{
				clusterObject.LockObject.UnlockReader();
			}
			return true;
		}
		return false;
	}

	internal static ResourceKind StringToResourceKind(string resourceType)
	{
		if (StringResourceKindDictionary.TryGetValue(resourceType, out var value))
		{
			return value;
		}
		return ResourceKind.Other;
	}

	internal static string ResourceKindToString(ResourceKind resourceKind)
	{
		if (ResourceKindStringDictionary.TryGetValue(resourceKind, out var value))
		{
			return value;
		}
		throw new InvalidCastException(ExceptionResources.ResourceTypeNotFound.FormatCurrentCulture(resourceKind));
	}

	internal static Guid IdFromName(string name)
	{
		return FormatHelper.UIntToGuid(FormatHelper.StringHash(name.ToLowerInvariant()));
	}

	public override string ToString()
	{
		return string.Concat("ResourceType:", base.Id, ":", base.Name, ":", base.IsOpen.ToString());
	}
}

