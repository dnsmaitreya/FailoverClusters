using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace KDDSL.FailoverClusters.Framework;

internal abstract class PGroup : PClusterObject<Group>
{
	private Group proxyGroup;

	private bool isHidden;

	private static readonly ConcurrentDictionary<GroupType, Func<PCluster, Guid, string, PGroup>> FactoryDictionary;

	private static readonly ConcurrentDictionary<GroupType, Func<Cluster, Group>> FactoryProxyDictionary;

	[DefaultValue(null)]
	public GroupState? GroupState { get; internal set; }

	[DefaultValue(null)]
	public Priority? Priority { get; internal set; }

	public GroupType GroupType { get; internal set; }

	public PNode OwnerNode { get; internal set; }

	[DefaultValue(null)]
	public bool? IsCore { get; internal set; }

	[DefaultValue(null)]
	public GroupFlags? Flags { get; internal set; }

	public List<Guid> PreferredOwners { get; internal set; }

	public bool IsHidden
	{
		get
		{
			return isHidden;
		}
		internal set
		{
			isHidden = value;
			RouteEvent(new ClusterWrapperEventArgs(EventType.Hidden, new ClusterIsHiddenEventArgs(base.Id, isHidden, null)));
			base.Cluster.RealtimeCollections.Change(this, "IsHidden");
		}
	}

	public override ClusterIdentityType IdentityType => ClusterIdentityType.Group;

	static PGroup()
	{
		FactoryDictionary = new ConcurrentDictionary<GroupType, Func<PCluster, Guid, string, PGroup>>();
		FactoryProxyDictionary = new ConcurrentDictionary<GroupType, Func<Cluster, Group>>();
		CreateFactoryDictionaries();
	}

	protected PGroup(PCluster cluster, Guid id, string name, GroupType groupType)
		: base(cluster)
	{
		base.Id = id;
		base.Name = name;
		GroupType = groupType;
		ReloadReplicationRole();
	}

	public void ReloadReplicationRole()
	{
		if (base.Cluster.CacheManager.ReplicatedGroups.TryGetValue(base.Id, out var value))
		{
			IsHidden = value.Role == ReplicationGroupRole.Secondary;
		}
		else
		{
			IsHidden = false;
		}
	}

	public static PGroup Constructor(PCluster cluster, Guid id, string name, GroupType groupType)
	{
		if (FactoryDictionary.TryGetValue(groupType, out var value))
		{
			return value(cluster, id, name);
		}
		return new POtherGroup(cluster, id, name);
	}

	public IEnumerable<string> GetPreferredOwners()
	{
		return ProtectedScope(() => ReleaseExecuteAndReacquire(delegate
		{
			PCluster cluster = base.Cluster;
			return (cluster != null) ? cluster.Server.Group.GetPreferredOwners(base.Id) : new List<string>();
		}));
	}

	public void SetPreferredOwners(IEnumerable<string> nodes)
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster?.Server.Group.SetPreferredOwners(base.Id, nodes);
			});
		});
	}

	public void SetPriority(Priority newPriority)
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster?.Server.Group.SetPriority(base.Id, newPriority);
			});
		});
	}

	public void Move(string nodeName, bool overrideLockState = false)
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster?.Server.Group.Move(base.Id, nodeName, overrideLockState);
			});
		}, (ClusterException ex) => ex, resetIsProcessing: false);
		if (GroupState != FailoverClusters.Framework.GroupState.Pending)
		{
			base.IsProcessing = false;
		}
	}

	public void Online(bool overrideLockState = false, bool chooseBestNode = false)
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster?.Server.Group.Online(base.Id, overrideLockState, chooseBestNode);
			});
		}, (ClusterException ex) => ex, resetIsProcessing: false);
		if (GroupState == FailoverClusters.Framework.GroupState.Online)
		{
			base.IsProcessing = false;
		}
	}

	public void Offline(bool overrideLockState = false)
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster?.Server.Group.Offline(base.Id, overrideLockState);
			});
		}, (ClusterException ex) => ex, resetIsProcessing: false);
		if (GroupState == FailoverClusters.Framework.GroupState.Offline)
		{
			base.IsProcessing = false;
		}
	}

	public void Cancel()
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster?.Server.Group.CancelOperation(base.Id);
			});
		});
	}

	public override void Delete()
	{
		Delete(force: false, cleanup: true);
	}

	public void Delete(bool force, bool cleanup)
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster?.Server.Group.Delete(base.Id, force: false, cleanUp: true);
			});
		}, (ClusterException ex) => ex, resetIsProcessing: false);
	}

	public PResource CreateResource(string name, ResourceType resourceType)
	{
		return CreateResource(name, resourceType, separateMonitor: false);
	}

	public PResource CreateResource(string name, ResourceType resourceType, bool separateMonitor)
	{
		PResource newResource = null;
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				PCluster cluster = base.Cluster;
				if (cluster != null)
				{
					newResource = cluster.Server.Resource.Create(this, name, new PResourceType(base.Cluster, resourceType.Name), separateMonitor);
				}
			});
		}, delegate(ClusterException ex)
		{
			if (ex == null && newResource != null)
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.Created, new ClusterAddedEventArgs(newResource.Id, newResource.Name, (int)resourceType.ResourceKind, resourceType.Name, base.Id, null)));
			}
		});
		return newResource;
	}

	public override void Rename(string newName)
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster?.Server.Group.Rename(base.Id, newName);
			});
		}, delegate(ClusterException ex)
		{
			if (ex != null)
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.Renamed, new ClusterRenamedEventArgs(base.Id, base.Name, ex)));
			}
		});
	}

	public override Group GetProxy()
	{
		return GetProxy(ProxyCreateMode.Singleton);
	}

	public override Group GetProxy(ProxyCreateMode createMode)
	{
		if (createMode == ProxyCreateMode.Singleton && proxyGroup != null)
		{
			return proxyGroup;
		}
		if (!FactoryProxyDictionary.TryGetValue(GroupType, out var value))
		{
			throw new NotSupportedException("There is no match for a Proxy group from PGroup {0}".FormatCurrentCulture(GetType()));
		}
		Group group = value(base.Cluster.GetProxy());
		group.TransferInternalData(this, subscribeToEvents: true);
		if (createMode == ProxyCreateMode.Singleton)
		{
			proxyGroup = group;
		}
		return group;
	}

	public override ClusterLoadedEventArgs LoadObject(int loadSelectionNeutral)
	{
		if ((base.LoadedSelection & loadSelectionNeutral) == loadSelectionNeutral)
		{
			return new ClusterLoadedEventArgs(base.Id, loaded: true, base.LoadedSelection, null);
		}
		ClusterLoadedEventArgs loadedArgs = null;
		int currentSelection = base.LoadedSelection;
		GroupLoadSelection loadSelection = (GroupLoadSelection)loadSelectionNeutral;
		ProtectedScope(delegate
		{
			base.Cluster.Server.Group.Load(this, loadSelection);
			if ((loadSelection & GroupLoadSelection.Basic) == GroupLoadSelection.Basic && base.Cluster.CacheManager != null)
			{
				PNode pNode = (PNode)base.Cluster.CacheManager.AddObject(OwnerNode);
				if (pNode.LoadedSelection == 0)
				{
					pNode.LockObject.Writer();
					try
					{
						if (pNode.LoadedSelection == 0)
						{
							pNode.LoadObject(1);
						}
					}
					finally
					{
						if (pNode.LockObject != null)
						{
							pNode.LockObject.UnlockWriter();
						}
					}
				}
				OwnerNode = pNode;
			}
		}, delegate(ClusterException ex)
		{
			if (ex == null)
			{
				int loadSelection2 = currentSelection ^ base.LoadedSelection;
				BroadcastChanges(loadSelection2);
			}
			loadedArgs = new ClusterLoadedEventArgs(base.Id, ex == null, (int)loadSelection, ex);
			RouteEvent(new ClusterWrapperEventArgs(EventType.Loaded, loadedArgs));
		}, resetIsProcessing: true, affectsIsProcessing: false);
		return loadedArgs;
	}

	internal void SendResourceGained(Guid resourceId)
	{
		ClusterGainedEventArgs eventArgs = new ClusterGainedEventArgs(base.Id, base.Name, resourceId);
		RouteEvent(new ClusterWrapperEventArgs(EventType.Gained, eventArgs));
	}

	public override void BroadcastChanges(int loadSelection, bool raiseLoadedEvent = false)
	{
		List<ClusterWrapperEventArgs> list = new List<ClusterWrapperEventArgs>(10);
		if (((GroupLoadSelection)loadSelection).HasFlag(GroupLoadSelection.Basic))
		{
			list.AddRange(new ClusterWrapperEventArgs[5]
			{
				new ClusterWrapperEventArgs(EventType.GroupStateChanged, new ClusterGroupStateEventArgs(base.Id, GroupState, null)),
				new ClusterWrapperEventArgs(EventType.GroupTypeChanged, new ClusterGroupTypeEventArgs(base.Id, GroupType, null)),
				new ClusterWrapperEventArgs(EventType.GroupOwnerNodeChanged, new ClusterGroupOwnerNodeEventArgs(base.Id, OwnerNode.Id, null)),
				new ClusterWrapperEventArgs(EventType.GroupIsCoreChanged, new ClusterGroupIsCoreEventArgs(base.Id, IsCore, null)),
				new ClusterWrapperEventArgs(EventType.GroupFlagsChanged, new ClusterGroupFlagsEventArgs(base.Id, Flags, null))
			});
		}
		if (((GroupLoadSelection)loadSelection).HasFlag(GroupLoadSelection.CommonProperties) || ((GroupLoadSelection)loadSelection).HasFlag(GroupLoadSelection.PrivateProperties))
		{
			list.Add(new ClusterWrapperEventArgs(EventType.PropertiesChanged, new ClusterPropertiesEventArgs(base.Id, base.Name, (int)GroupType, null)
			{
				Properties = base.Properties
			}));
		}
		if (((GroupLoadSelection)loadSelection).HasFlag(GroupLoadSelection.CommonProperties) || ((GroupLoadSelection)loadSelection).HasFlag(GroupLoadSelection.Basic))
		{
			list.Add(new ClusterWrapperEventArgs(EventType.GroupPriorityChanged, new ClusterGroupPriorityEventArgs(base.Id, Priority, null)));
		}
		if (((GroupLoadSelection)loadSelection).HasFlag(GroupLoadSelection.PreferredOwners))
		{
			list.Add(new ClusterWrapperEventArgs(EventType.GroupPreferredOwnersChanged, new ClusterGroupPreferredOwnersChangedEventArgs(base.Id, PreferredOwners, null)));
		}
		if (raiseLoadedEvent)
		{
			ClusterLoadedEventArgs eventArgs = new ClusterLoadedEventArgs(base.Id, loaded: true, base.LoadedSelection, null);
			list.Add(new ClusterWrapperEventArgs(EventType.Loaded, eventArgs));
		}
		RouteEvent(new ClusterWrapperEventArgs(EventType.BatchChanges, new ClusterBatchChangesEventArgs(base.Id, list)));
	}

	public override List<Action> ProcessNotification(Notification notification)
	{
		List<Action> list = base.ProcessNotification(notification);
		if (notification.Payload is ClusterGroupStateEventArgs)
		{
			ClusterGroupStateEventArgs clusterGroupStateEventArgs = (ClusterGroupStateEventArgs)notification.Payload;
			GroupState = clusterGroupStateEventArgs.State;
			list.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.GroupStateChanged, notification.Payload));
				base.Cluster.RealtimeCollections.Change(this, "GroupState");
			});
		}
		else if (notification.Payload is ClusterGroupPriorityEventArgs)
		{
			ClusterGroupPriorityEventArgs clusterGroupPriorityEventArgs = (ClusterGroupPriorityEventArgs)notification.Payload;
			Priority = clusterGroupPriorityEventArgs.Priority;
			list.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.GroupPriorityChanged, notification.Payload));
				base.Cluster.RealtimeCollections.Change(this, "Priority");
			});
		}
		else if (notification.Payload is ClusterGroupFlagsEventArgs)
		{
			ClusterGroupFlagsEventArgs clusterGroupFlagsEventArgs = (ClusterGroupFlagsEventArgs)notification.Payload;
			Flags = clusterGroupFlagsEventArgs.Flags;
			list.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.GroupFlagsChanged, notification.Payload));
				base.Cluster.RealtimeCollections.Change(this, "Flags");
			});
		}
		else if (notification.Payload is ClusterGroupTypeEventArgs)
		{
			ClusterGroupTypeEventArgs args = (ClusterGroupTypeEventArgs)notification.Payload;
			GroupType = args.GroupType;
			list.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.GroupTypeChanged, args));
				base.Cluster.RealtimeCollections.Change(this, "GroupType");
			});
		}
		else if (notification.Payload is ClusterGroupOwnerNodeEventArgs)
		{
			ClusterGroupOwnerNodeEventArgs args2 = (ClusterGroupOwnerNodeEventArgs)notification.Payload;
			ClusterLock clusterLock = base.Cluster.CacheManager.Get(args2.NodeId, ClusterIdentityType.Node, LockAccess.Reader);
			try
			{
				if (clusterLock == null)
				{
					PNode privateNode = (PNode)base.Cluster.CacheManager.AddObject(base.Cluster, ClusterIdentityType.Node, args2.NodeId);
					try
					{
						privateNode.LoadObject(0);
						OwnerNode = privateNode;
						list.Add(delegate
						{
							base.Cluster.RealtimeCollections.Add(privateNode, RTCOperation.Add);
						});
					}
					catch (ClusterObjectNotFoundException)
					{
						base.Cluster.CacheManager.RemoveObject(privateNode);
					}
				}
				else
				{
					OwnerNode = (PNode)clusterLock.Owner;
				}
			}
			finally
			{
				clusterLock?.UnlockReader();
			}
			list.Add(delegate
			{
				ClusterGroupOwnerNodeEventArgs eventArgs = (ClusterGroupOwnerNodeEventArgs)notification.Payload;
				PGroup coreGroup = base.Cluster.CoreGroup;
				if (coreGroup != null && coreGroup.Id == base.Id)
				{
					base.Cluster.CoreGroupOwnerNodeChanged(OwnerNode);
				}
				RouteEvent(new ClusterWrapperEventArgs(EventType.GroupOwnerNodeChanged, eventArgs));
				args2.WaitAndExecuteWhenFinished(delegate
				{
					base.LockObject.Reader();
					try
					{
						base.Cluster.RealtimeCollections.Change(this, "OwnerNode");
					}
					finally
					{
						base.LockObject.UnlockReader();
					}
				});
			});
		}
		else if (notification.Payload is ClusterRemovedEventArgs)
		{
			ClusterRemovedEventArgs args3 = (ClusterRemovedEventArgs)notification.Payload;
			args3.Cluster.CacheManager.CacheLock.EnterWriteLock();
			try
			{
				base.IsRemoved = true;
				args3.Cluster.CacheManager.RemoveObject(this);
				list.Add(delegate
				{
					RouteEvent(new ClusterWrapperEventArgs(EventType.Removed, notification.Payload));
					args3.Cluster.RealtimeCollections.Remove<Group>(this);
				});
			}
			finally
			{
				args3.Cluster.CacheManager.CacheLock.ExitWriteLock();
			}
		}
		else if (notification.Payload is ClusterRenamedEventArgs)
		{
			base.Name = ((ClusterRenamedEventArgs)notification.Payload).NewName;
			list.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.Renamed, notification.Payload));
				base.Cluster.RealtimeCollections.Change(this, "Name");
			});
		}
		else if (notification.Payload is ClusterForwardedEventArgs)
		{
			ClusterForwardedEventArgs payloadArgs = notification.Payload as ClusterForwardedEventArgs;
			list.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.ForwardPayload, payloadArgs));
			});
		}
		else if (notification.Payload is ClusterGainedEventArgs)
		{
			ClusterGainedEventArgs payloadArgs2 = notification.Payload as ClusterGainedEventArgs;
			list.Add(delegate
			{
				PVirtualMachineGroup privateVirtualMachineGroup = this as PVirtualMachineGroup;
				if (privateVirtualMachineGroup != null)
				{
					using ClusterLock clusterLock2 = privateVirtualMachineGroup.Cluster.CacheManager.Get(payloadArgs2.GainedId, ClusterIdentityType.Resource, LockAccess.Reader);
					PVirtualMachineResource virtualMachineResource = clusterLock2.Owner as PVirtualMachineResource;
					if (virtualMachineResource != null)
					{
						virtualMachineResource.LoadObject(4);
						base.Cluster.RealtimeCollections.Change(this, "LoadSelection");
						virtualMachineResource.Properties.Get("VmState", delegate(ClusterPropertyUInt vmStateProperty)
						{
							privateVirtualMachineGroup.AddVirtualMachineChildResource(virtualMachineResource, (VirtualMachineState)vmStateProperty.TypedValue);
						});
					}
				}
				RouteEvent(new ClusterWrapperEventArgs(EventType.Gained, payloadArgs2));
			});
		}
		else if (notification.Payload is ClusterLostEventArgs)
		{
			ClusterLostEventArgs payloadArgs3 = notification.Payload as ClusterLostEventArgs;
			if (this is PVirtualMachineGroup pVirtualMachineGroup)
			{
				pVirtualMachineGroup.RemoveVirtualMachineChildResource(payloadArgs3.LostId);
			}
			list.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.Lost, payloadArgs3));
			});
		}
		else if (notification.Payload is ClusterGroupPreferredOwnersChangedEventArgs)
		{
			ClusterGroupPreferredOwnersChangedEventArgs preferredOwnerArgs = notification.Payload as ClusterGroupPreferredOwnersChangedEventArgs;
			bool flag = true;
			if (PreferredOwners == null || PreferredOwners.Count != preferredOwnerArgs.PreferredNodes.Count)
			{
				flag = false;
			}
			else if (PreferredOwners.Where((Guid t, int i) => t != preferredOwnerArgs.PreferredNodes[i]).Any())
			{
				flag = false;
			}
			if (!flag)
			{
				PreferredOwners = preferredOwnerArgs.PreferredNodes;
				list.Add(delegate
				{
					RouteEvent(new ClusterWrapperEventArgs(EventType.GroupPreferredOwnersChanged, new ClusterGroupPreferredOwnersChangedEventArgs(base.Id, PreferredOwners, null)));
				});
			}
		}
		return list;
	}

	public static bool ProcessNotificationSpecial(Notification notification)
	{
		if (notification.Payload is ClusterAddedEventArgs clusterAddedEventArgs)
		{
			PGroup clusterObject = Constructor(clusterAddedEventArgs.Cluster, clusterAddedEventArgs.Id, clusterAddedEventArgs.Name, (GroupType)clusterAddedEventArgs.ObjectType.Value);
			clusterObject = (PGroup)clusterAddedEventArgs.Cluster.CacheManager.AddObject(clusterObject, clusterAddedEventArgs is ClusterUpsertEventArgs);
			clusterObject.LockObject.Reader();
			try
			{
				clusterAddedEventArgs.Cluster.RealtimeCollections.Add(clusterObject, (clusterAddedEventArgs is ClusterUpsertEventArgs) ? RTCOperation.Replace : RTCOperation.Add);
			}
			finally
			{
				clusterObject.LockObject.UnlockReader();
			}
			return true;
		}
		return false;
	}

	public override void TransferFrom(PClusterObject source, bool cacheIsLocked, int loadSelection)
	{
		PGroup sourceObject = source as PGroup;
		if (sourceObject == null)
		{
			throw new InvalidOperationException("Source and Target must be the same type: ".FormatCurrentCulture(GetType()));
		}
		sourceObject.OwnerNode = (PNode)base.Cluster.CacheManager.AddObject(sourceObject.OwnerNode, cacheIsLocked);
		LockTransferAndBroadCast(source, loadSelection, delegate
		{
			GroupLoadSelection groupLoadSelection = (GroupLoadSelection)loadSelection;
			if (groupLoadSelection.HasFlag(GroupLoadSelection.Basic))
			{
				Flags = sourceObject.Flags;
				GroupState = sourceObject.GroupState;
				GroupType = sourceObject.GroupType;
				IsCore = sourceObject.IsCore;
				Priority = sourceObject.Priority;
				OwnerNode = sourceObject.OwnerNode;
				base.LoadedSelection |= 1;
			}
			if (groupLoadSelection.HasFlag(GroupLoadSelection.CommonProperties) || groupLoadSelection.HasFlag(GroupLoadSelection.PrivateProperties))
			{
				base.Properties.AddOrUpdate(sourceObject.Properties);
				if (groupLoadSelection.HasFlag(GroupLoadSelection.CommonProperties))
				{
					base.LoadedSelection |= 2;
				}
				if (groupLoadSelection.HasFlag(GroupLoadSelection.PrivateProperties))
				{
					base.LoadedSelection |= 4;
				}
			}
			base.LoadedSelection |= 1;
			if (groupLoadSelection.HasFlag(GroupLoadSelection.PreferredOwners))
			{
				PreferredOwners = sourceObject.PreferredOwners;
				base.LoadedSelection |= 8;
			}
		});
	}

	private static void AddFactoryEntry(GroupType groupType, Func<PCluster, Guid, string, PGroup> privateGroupAction, Func<Cluster, Group> proxyGroupAction)
	{
		FactoryDictionary.TryAdd(groupType, privateGroupAction);
		FactoryProxyDictionary.TryAdd(groupType, proxyGroupAction);
	}

	private static void CreateFactoryDictionaries()
	{
		AddFactoryEntry(GroupType.AvailableStorage, (PCluster cluster, Guid id, string name) => new PAvailableStorageGroup(cluster, id, name), (Cluster cluster) => new AvailableStorageGroup(cluster));
		AddFactoryEntry(GroupType.ClusterSharedVolume, (PCluster cluster, Guid id, string name) => new PClusterSharedVolumeGroup(cluster, id, name), (Cluster cluster) => new ClusterSharedVolumeGroup(cluster));
		AddFactoryEntry(GroupType.CoreCluster, (PCluster cluster, Guid id, string name) => new PCoreClusterGroup(cluster, id, name), (Cluster cluster) => new CoreClusterGroup(cluster));
		AddFactoryEntry(GroupType.DhcpServer, (PCluster cluster, Guid id, string name) => new PDhcpServerGroup(cluster, id, name), (Cluster cluster) => new DhcpServerGroup(cluster));
		AddFactoryEntry(GroupType.Dtc, (PCluster cluster, Guid id, string name) => new PDtcGroup(cluster, id, name), (Cluster cluster) => new DtcGroup(cluster));
		AddFactoryEntry(GroupType.FileServer, (PCluster cluster, Guid id, string name) => new PFileServerGroup(cluster, id, name), (Cluster cluster) => new FileServerGroup(cluster));
		AddFactoryEntry(GroupType.GenericApplication, (PCluster cluster, Guid id, string name) => new PGenericApplicationGroup(cluster, id, name), (Cluster cluster) => new GenericApplicationGroup(cluster));
		AddFactoryEntry(GroupType.GenericScript, (PCluster cluster, Guid id, string name) => new PGenericScriptGroup(cluster, id, name), (Cluster cluster) => new GenericScriptGroup(cluster));
		AddFactoryEntry(GroupType.GenericService, (PCluster cluster, Guid id, string name) => new PGenericServiceGroup(cluster, id, name), (Cluster cluster) => new GenericServiceGroup(cluster));
		AddFactoryEntry(GroupType.InfrastructureFileServer, (PCluster cluster, Guid id, string name) => new PInfrastructureFileServerGroup(cluster, id, name), (Cluster cluster) => new InfrastructureFileServerGroup(cluster));
		AddFactoryEntry(GroupType.IScsiNameService, (PCluster cluster, Guid id, string name) => new PIScsiNameServiceGroup(cluster, id, name), (Cluster cluster) => new IScsiNameServiceGroup(cluster));
		AddFactoryEntry(GroupType.IScsiTarget, (PCluster cluster, Guid id, string name) => new PIScsiTargetGroup(cluster, id, name), (Cluster cluster) => new IScsiTargetGroup(cluster));
		AddFactoryEntry(GroupType.Msmq, (PCluster cluster, Guid id, string name) => new PMsmqGroup(cluster, id, name), (Cluster cluster) => new MsmqGroup(cluster));
		AddFactoryEntry(GroupType.ScaleOutFileServer, (PCluster cluster, Guid id, string name) => new PScaleOutFileServerGroup(cluster, id, name), (Cluster cluster) => new ScaleOutFileServerGroup(cluster));
		AddFactoryEntry(GroupType.StandAloneDfs, (PCluster cluster, Guid id, string name) => new PStandAloneDfsGroup(cluster, id, name), (Cluster cluster) => new StandAloneDfsGroup(cluster));
		AddFactoryEntry(GroupType.Temporary, (PCluster cluster, Guid id, string name) => new PTemporaryGroup(cluster, id, name), (Cluster cluster) => new TemporaryGroup(cluster));
		AddFactoryEntry(GroupType.TsSessionBroker, (PCluster cluster, Guid id, string name) => new PTsSessionBrokerGroup(cluster, id, name), (Cluster cluster) => new TsSessionBrokerGroup(cluster));
		AddFactoryEntry(GroupType.VirtualMachine, (PCluster cluster, Guid id, string name) => new PVirtualMachineGroup(cluster, id, name), (Cluster cluster) => new VirtualMachineGroup(cluster));
		AddFactoryEntry(GroupType.VMReplicaBroker, (PCluster cluster, Guid id, string name) => new PVMReplicaBrokerGroup(cluster, id, name), (Cluster cluster) => new VMReplicaBrokerGroup(cluster));
		AddFactoryEntry(GroupType.Wins, (PCluster cluster, Guid id, string name) => new PWinsGroup(cluster, id, name), (Cluster cluster) => new WinsGroup(cluster));
		AddFactoryEntry(GroupType.ClusterStoragePool, (PCluster cluster, Guid id, string name) => new PClusterStoragePoolGroup(cluster, id, name), (Cluster cluster) => new ClusterStoragePoolGroup(cluster));
		AddFactoryEntry(GroupType.TaskScheduler, (PCluster cluster, Guid id, string name) => new PTaskSchedulerGroup(cluster, id, name), (Cluster cluster) => new TaskSchedulerGroup(cluster));
		AddFactoryEntry(GroupType.ClusterAwareUpdating, (PCluster cluster, Guid id, string name) => new PClusterAwareUpdatingGroup(cluster, id, name), (Cluster cluster) => new ClusterAwareUpdatingGroup(cluster));
		AddFactoryEntry(GroupType.StorageReplica, (PCluster cluster, Guid id, string name) => new PStorageReplicaGroup(cluster, id, name), (Cluster cluster) => new StorageReplicaGroup(cluster));
		AddFactoryEntry(GroupType.VirtualMachineReplicationCoordinator, (PCluster cluster, Guid id, string name) => new PVirtualMachineReplicationCoordinatorGroup(cluster, id, name), (Cluster cluster) => new VirtualMachineReplicationCoordinatorGroup(cluster));
		FactoryProxyDictionary.TryAdd(GroupType.Unknown, (Cluster cluster) => new OtherGroup(cluster));
	}
}

