using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace KDDSL.FailoverClusters.Framework;

internal class PResource : PClusterObject<Resource>
{
	private static readonly ConcurrentDictionary<ResourceKind, Func<PCluster, Guid, string, PResource>> FactoryDictionary;

	private static readonly ConcurrentDictionary<ResourceKind, Func<Cluster, Resource>> FactoryProxyDictionary;

	private Resource proxyResource;

	private bool isChild;

	private bool isChildLoaded;

	private bool isQuorum;

	[DefaultValue(null)]
	public ResourceState? ResourceState { get; internal set; }

	public PGroup OwnerGroup { get; internal set; }

	[DefaultValue(null)]
	public ResourceFlags? Flags { get; internal set; }

	[DefaultValue(null)]
	public Characteristics? Characteristics { get; internal set; }

	public ResourceClass Class { get; internal set; }

	public ResourceSubclass Subclass { get; internal set; }

	public PResourceType ResourceType { get; internal set; }

	public string DependencyRelationship { get; internal set; }

	public RequiredDependencies RequiredDependencies { get; internal set; }

	public List<Guid> PossibleOwners { get; internal set; }

	public ClusterDisk DiskInfo { get; internal set; }

	public override ClusterIdentityType IdentityType => ClusterIdentityType.Resource;

	public bool IsQuorum
	{
		get
		{
			return isQuorum;
		}
		internal set
		{
			if (isQuorum != value)
			{
				isQuorum = value;
				RouteEvent(new ClusterWrapperEventArgs(EventType.ResourceIsQuorumChanged, new ClusterResourceIsQuorumChangedEventArgs(base.Id, isQuorum)));
			}
		}
	}

	public bool? IsChildUnderlineValue
	{
		get
		{
			if (!isChildLoaded)
			{
				return null;
			}
			return isChild;
		}
	}

	public bool IsChild
	{
		get
		{
			if (!isChildLoaded)
			{
				isChild = IsChildResource();
				isChildLoaded = true;
			}
			return isChild;
		}
	}

	public IEnumerable<Guid> Dependencies { get; internal set; }

	public IEnumerable<Guid> Dependents { get; internal set; }

	static PResource()
	{
		FactoryDictionary = new ConcurrentDictionary<ResourceKind, Func<PCluster, Guid, string, PResource>>();
		FactoryProxyDictionary = new ConcurrentDictionary<ResourceKind, Func<Cluster, Resource>>();
		CreateFactoryDictionaries();
	}

	protected PResource(PCluster cluster, Guid id, string name, PResourceType resourceType)
		: base(cluster)
	{
		Exceptions.ThrowIfNull(cluster, "cluster");
		base.Id = id;
		base.Name = name;
		ResourceType = resourceType;
		if (cluster.QuorumConfiguration != null && cluster.QuorumConfiguration.QuorumResourceId == base.Id)
		{
			IsQuorum = true;
		}
	}

	public static PResource Constructor(PCluster cluster, Guid id, string name, PResourceType resourceType)
	{
		Exceptions.ThrowIfNull(resourceType, "resourceType");
		PResource pResource = null;
		pResource = (FactoryDictionary.TryGetValue(resourceType.ResourceKind, out var value) ? value(cluster, id, name) : ((resourceType.Class != ResourceClass.Storage) ? ((PResource)new POtherResource(cluster, id, name, resourceType.Name)) : ((PResource)new PStorageResource(cluster, id, name, resourceType))));
		pResource.Class = resourceType.Class;
		pResource.Subclass = resourceType.Subclass;
		pResource.Characteristics = resourceType.Characteristics;
		if (pResource is PStorageResource pStorageResource)
		{
			pStorageResource.ReloadReplicationRole();
			pStorageResource.AssignStorageCaps();
		}
		return pResource;
	}

	public void ReloadReplicationRole()
	{
		if (this is PStorageResource pStorageResource)
		{
			pStorageResource.ReplicationDiskType = (base.Cluster.CacheManager.ReplicatedResources.TryGetValue(pStorageResource.Id, out var value) ? value.Role : ReplicationDiskType.None);
		}
	}

	protected virtual void OnStateChanged(object sender, ClusterResourceStateEventArgs e)
	{
	}

	protected virtual void OnOwnerGroupChanged(object sender, ClusterResourceOwnerGroupEventArgs e)
	{
	}

	public virtual bool IsChildResource()
	{
		return false;
	}

	public void AddRegistryCheckpoint(string checkpoint)
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.AddRegistryCheckpoint(base.Id, checkpoint);
			});
		});
	}

	public void RemoveRegistryCheckpoint(string checkpoint)
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.RemoveRegistryCheckpoint(base.Id, checkpoint);
			});
		});
	}

	public IEnumerable<string> GetRegistryCheckpoints()
	{
		return ProtectedScope(() => ReleaseExecuteAndReacquire(() => base.Cluster.Server.Resource.GetRegistryCheckpoints(base.Id)));
	}

	public void AddCryptoCheckpoint(string checkpoint)
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.AddCryptoCheckpoint(base.Id, checkpoint);
			});
		});
	}

	public void RemoveCryptoCheckpoint(string checkpoint)
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.RemoveCryptoCheckpoint(base.Id, checkpoint);
			});
		});
	}

	public IEnumerable<string> GetCryptoCheckpoints()
	{
		return ProtectedScope(() => ReleaseExecuteAndReacquire(() => base.Cluster.Server.Resource.GetCryptoCheckpoints(base.Id)));
	}

	public IEnumerable<string> GetPossibleOwners()
	{
		return ProtectedScope(() => ReleaseExecuteAndReacquire(() => base.Cluster.Server.Resource.GetPossibleOwners(base.Name)));
	}

	public void AddPossibleOwner(string node)
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.AddPossibleOwner(base.Id, node);
			});
		});
	}

	public void RemovePossibleOwner(string node)
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.RemovePossibleOwner(base.Id, node);
			});
		});
	}

	public void SetPossibleOwners(IEnumerable<Guid> nodes)
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.SetPossibleOwners(base.Id, nodes);
			});
		});
	}

	public override void Delete()
	{
		Delete(cleanup: true);
	}

	public void Delete(bool cleanup)
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				try
				{
					base.Cluster.Server.Resource.Delete(base.Id, cleanup);
				}
				catch (ClusterResourceLockedException)
				{
					base.IsProcessing = false;
				}
			});
		}, (ClusterException ex) => ex, resetIsProcessing: false);
	}

	public void AddDependency(PResource dependOnResource)
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.AddDependency(base.Id, dependOnResource.Id);
			});
		});
	}

	public void RemoveDependency(PResource dependOnResource)
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.RemoveDependency(base.Id, dependOnResource.Id);
			});
		});
	}

	public override void Rename(string newName)
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.Rename(base.Id, newName);
			});
		}, delegate(ClusterException ex)
		{
			if (ex != null)
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.Renamed, new ClusterRenamedEventArgs(base.Id, base.Name, ex)));
			}
		});
	}

	public void Online(bool overrideLockState = false, bool chooseBestNode = false)
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.Online(base.Id, overrideLockState, chooseBestNode);
			});
		}, (ClusterException ex) => ex, resetIsProcessing: false);
		if (ResourceState == FailoverClusters.Framework.ResourceState.Online)
		{
			base.IsProcessing = false;
		}
	}

	public virtual void Offline(bool overrideLockState = false)
	{
		ProtectedScope(delegate
		{
			ClusterLog.LogVerbose(LogSubcategory.FxPrivateObject, "Offlining resource {0}", base.Name);
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.Offline(base.Id, overrideLockState);
			});
		}, (ClusterException ex) => ex, resetIsProcessing: false);
		if (ResourceState == FailoverClusters.Framework.ResourceState.Offline)
		{
			base.IsProcessing = false;
		}
	}

	public void Fail()
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.Fail(base.Id);
			});
		});
	}

	public void Move(Guid groupId)
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.MoveToGroup(base.Id, groupId);
			});
		}, (ClusterException ex) => ex, resetIsProcessing: false);
	}

	public void SetDependencyRelationship(string relationship)
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.SetDependencyRelationship(base.Id, relationship);
			});
		});
	}

	public override void BroadcastChanges(int loadSelection, bool raiseLoadedEvent = false)
	{
		List<ClusterWrapperEventArgs> list = new List<ClusterWrapperEventArgs>(10);
		if (((ResourceLoadSelection)loadSelection).HasFlag(ResourceLoadSelection.Basic))
		{
			list.AddRange(new ClusterWrapperEventArgs[4]
			{
				new ClusterWrapperEventArgs(EventType.ResourceStateChanged, new ClusterResourceStateEventArgs(base.Id, ResourceState, null)),
				new ClusterWrapperEventArgs(EventType.ResourceOwnerGroupChanged, new ClusterResourceOwnerGroupEventArgs(base.Id, OwnerGroup.Id, null)),
				new ClusterWrapperEventArgs(EventType.ResourceFlagsChanged, new ClusterResourceFlagsEventArgs(base.Id, Flags, null)),
				new ClusterWrapperEventArgs(EventType.ResourceCharacteristicsChanged, new ClusterCharacteristicsEventArgs(base.Id, Characteristics, null))
			});
			if (this is PDistributedNetworkNameResource pDistributedNetworkNameResource)
			{
				list.Add(new ClusterWrapperEventArgs(EventType.NetworkInterfacesChanged, new ClusterNetworkChangedEventArgs(base.Id, pDistributedNetworkNameResource.ClusterNetworkInfos, null)));
			}
		}
		if (((ResourceLoadSelection)loadSelection).HasFlag(ResourceLoadSelection.CommonProperties) || ((ResourceLoadSelection)loadSelection).HasFlag(ResourceLoadSelection.PrivateProperties))
		{
			list.Add(new ClusterWrapperEventArgs(EventType.PropertiesChanged, new ClusterPropertiesEventArgs(base.Id, base.Name, null, null)
			{
				Properties = base.Properties
			}));
		}
		if (((ResourceLoadSelection)loadSelection).HasFlag(ResourceLoadSelection.Storage))
		{
			list.Add(new ClusterWrapperEventArgs(EventType.ResourceStoragePropertiesChanged, new ClusterStoragePropertiesChangedEventArgs(base.Id, DiskInfo, reload: false, null)));
		}
		if (((ResourceLoadSelection)loadSelection).HasFlag(ResourceLoadSelection.Dependencies))
		{
			list.Add(new ClusterWrapperEventArgs(EventType.ResourceDependenciesChanged, new ClusterDependenciesEventArgs(base.Id, Dependencies, null)));
		}
		if (((ResourceLoadSelection)loadSelection).HasFlag(ResourceLoadSelection.Dependents))
		{
			list.Add(new ClusterWrapperEventArgs(EventType.ResourceDependentsChanged, new ClusterDependentsEventArgs(base.Id, Dependents, null, IsChild)));
		}
		if (((ResourceLoadSelection)loadSelection).HasFlag(ResourceLoadSelection.DependenciesRelation))
		{
			list.Add(new ClusterWrapperEventArgs(EventType.ResourceDependencyRelationshipChanged, new ClusterDependencyRelationshipEventArgs(base.Id, DependencyRelationship, null)));
		}
		if (((ResourceLoadSelection)loadSelection).HasFlag(ResourceLoadSelection.RequiredDependencies))
		{
			list.Add(new ClusterWrapperEventArgs(EventType.ResourceRequiredDependenciesChanged, new ClusterRequiredDependenciesEventArgs(base.Id, RequiredDependencies, null)));
		}
		if (((ResourceLoadSelection)loadSelection).HasFlag(ResourceLoadSelection.PossibleOwners))
		{
			list.Add(new ClusterWrapperEventArgs(EventType.ResourcePossibleOwnersChanged, new ClusterResourcePossibleOwnersChangedEventArgs(base.Id, PossibleOwners, null)));
		}
		if (raiseLoadedEvent)
		{
			ClusterLoadedEventArgs eventArgs = new ClusterLoadedEventArgs(base.Id, loaded: true, base.LoadedSelection, null);
			list.Add(new ClusterWrapperEventArgs(EventType.Loaded, eventArgs));
		}
		RouteEvent(new ClusterWrapperEventArgs(EventType.BatchChanges, new ClusterBatchChangesEventArgs(base.Id, list)));
	}

	public override Resource GetProxy()
	{
		return GetProxy(ProxyCreateMode.Singleton);
	}

	public override Resource GetProxy(ProxyCreateMode createMode)
	{
		if (createMode == ProxyCreateMode.Singleton && proxyResource != null)
		{
			return proxyResource;
		}
		if (!FactoryProxyDictionary.TryGetValue(ResourceType.ResourceKind, out var value))
		{
			if (ResourceType.Class == ResourceClass.Storage)
			{
				value = (Cluster cluster) => new StorageResource(cluster);
			}
			else
			{
				if (ResourceType.ResourceKind != ResourceKind.Other)
				{
					throw new NotSupportedException("There is no match for a Proxy resource from PResource {0}".FormatCurrentCulture(GetType()));
				}
				value = (Cluster cluster) => new OtherResource(cluster);
			}
		}
		Resource resource = value(base.Cluster.GetProxy());
		resource.ResourceType = ResourceType.GetProxy();
		resource.TransferInternalData(this, subscribeToEvents: true);
		if (createMode == ProxyCreateMode.Singleton)
		{
			proxyResource = resource;
		}
		return resource;
	}

	public override ClusterLoadedEventArgs LoadObject(int loadSelectionNeutral)
	{
		if ((loadSelectionNeutral & 0x20000000) == 536870912)
		{
			loadSelectionNeutral ^= 0x20000000;
			base.LoadedSelection &= ~loadSelectionNeutral;
		}
		if ((base.LoadedSelection & loadSelectionNeutral) == loadSelectionNeutral)
		{
			return new ClusterLoadedEventArgs(base.Id, loaded: true, base.LoadedSelection, null);
		}
		ClusterLoadedEventArgs loadedArgs = null;
		int currentSelection = base.LoadedSelection;
		ResourceLoadSelection loadSelection = (ResourceLoadSelection)loadSelectionNeutral;
		ProtectedScope(delegate
		{
			base.Cluster.Server.Resource.Load(this, loadSelection);
			if ((loadSelection & ResourceLoadSelection.Basic) == ResourceLoadSelection.Basic && base.Cluster.CacheManager != null)
			{
				PGroup pGroup = (PGroup)base.Cluster.CacheManager.AddObject(OwnerGroup);
				if (pGroup.LoadedSelection == 0)
				{
					pGroup.LockObject.Writer();
					try
					{
						if (pGroup.LoadedSelection == 0)
						{
							pGroup.LoadObject(1);
						}
					}
					finally
					{
						if (pGroup.LockObject != null)
						{
							pGroup.LockObject.UnlockWriter();
						}
					}
				}
				OwnerGroup = pGroup;
			}
		}, delegate(ClusterException ex)
		{
			int loadSelection2 = currentSelection ^ base.LoadedSelection;
			if (ex == null)
			{
				BroadcastChanges(loadSelection2);
			}
			loadedArgs = new ClusterLoadedEventArgs(base.Id, ex == null, loadSelection2, ex);
			RouteEvent(new ClusterWrapperEventArgs(EventType.Loaded, loadedArgs));
		}, resetIsProcessing: true, affectsIsProcessing: false);
		return loadedArgs;
	}

	public override void TransferFrom(PClusterObject source, bool cacheIsLocked, int loadSelection)
	{
		PResource sourceObject = source as PResource;
		if (sourceObject == null)
		{
			throw new InvalidOperationException("Source and Target must be the same type: ".FormatCurrentCulture(GetType()));
		}
		sourceObject.OwnerGroup = (PGroup)base.Cluster.CacheManager.AddObject(sourceObject.OwnerGroup, cacheIsLocked);
		sourceObject.ResourceType = (PResourceType)base.Cluster.CacheManager.AddObject(sourceObject.ResourceType, cacheIsLocked);
		LockTransferAndBroadCast(source, loadSelection, delegate
		{
			ResourceLoadSelection resourceLoadSelection = (ResourceLoadSelection)loadSelection;
			if (resourceLoadSelection.HasFlag(ResourceLoadSelection.Basic))
			{
				Characteristics = sourceObject.Characteristics;
				Class = sourceObject.Class;
				Subclass = sourceObject.Subclass;
				Flags = sourceObject.Flags;
				ResourceState = sourceObject.ResourceState;
				ResourceType = sourceObject.ResourceType;
				OwnerGroup = sourceObject.OwnerGroup;
				base.LoadedSelection |= 1;
				PStorageResource pStorageResource = sourceObject as PStorageResource;
				PStorageResource pStorageResource2 = this as PStorageResource;
				if (pStorageResource != null && pStorageResource2 != null)
				{
					pStorageResource2.StorageCaps = pStorageResource.StorageCaps;
				}
			}
			if (resourceLoadSelection.HasFlag(ResourceLoadSelection.CommonProperties) || resourceLoadSelection.HasFlag(ResourceLoadSelection.PrivateProperties))
			{
				base.Properties.AddOrUpdate(sourceObject.Properties);
				if (resourceLoadSelection.HasFlag(ResourceLoadSelection.CommonProperties))
				{
					base.LoadedSelection |= 2;
				}
				if (resourceLoadSelection.HasFlag(ResourceLoadSelection.PrivateProperties))
				{
					base.LoadedSelection |= 4;
				}
			}
			if (resourceLoadSelection.HasFlag(ResourceLoadSelection.Dependencies))
			{
				Dependencies = sourceObject.Dependencies;
				base.LoadedSelection |= 8;
			}
			if (resourceLoadSelection.HasFlag(ResourceLoadSelection.Dependents))
			{
				Dependents = sourceObject.Dependents;
				base.LoadedSelection |= 0x10;
			}
			if (resourceLoadSelection.HasFlag(ResourceLoadSelection.DependenciesRelation))
			{
				DependencyRelationship = sourceObject.DependencyRelationship;
				base.LoadedSelection |= 0x20;
			}
			if (resourceLoadSelection.HasFlag(ResourceLoadSelection.RequiredDependencies))
			{
				RequiredDependencies = sourceObject.RequiredDependencies;
				base.LoadedSelection |= 0x40;
			}
			if (resourceLoadSelection.HasFlag(ResourceLoadSelection.PossibleOwners))
			{
				PossibleOwners = sourceObject.PossibleOwners;
				base.LoadedSelection |= 0x80;
			}
		});
	}

	public override List<Action> ProcessNotification(Notification notification)
	{
		List<Action> list = base.ProcessNotification(notification);
		if (notification.Payload is ClusterResourceStateEventArgs)
		{
			ClusterResourceStateEventArgs args = (ClusterResourceStateEventArgs)notification.Payload;
			ResourceState = args.State;
			OnStateChanged(this, args);
			list.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.ResourceStateChanged, args));
				base.Cluster.RealtimeCollections.Change(this, "ResourceState");
			});
		}
		else if (notification.Payload is ClusterResourceOwnerGroupEventArgs)
		{
			ClusterResourceOwnerGroupEventArgs args2 = (ClusterResourceOwnerGroupEventArgs)notification.Payload;
			ClusterLock clusterLock = base.Cluster.CacheManager.Get(args2.GroupId, ClusterIdentityType.Group, LockAccess.Writer);
			try
			{
				if (clusterLock == null)
				{
					PGroup privateGroup = (PGroup)base.Cluster.CacheManager.AddObject(base.Cluster, ClusterIdentityType.Group, args2.GroupId);
					try
					{
						privateGroup.LoadObject(0);
					}
					catch (ClusterObjectNotFoundException)
					{
						base.Cluster.CacheManager.RemoveObject(privateGroup);
						throw;
					}
					OwnerGroup = privateGroup;
					list.Add(delegate
					{
						base.Cluster.RealtimeCollections.Add(privateGroup, RTCOperation.Add);
					});
				}
				else
				{
					OwnerGroup = (PGroup)clusterLock.Owner;
				}
			}
			finally
			{
				clusterLock?.UnlockWriter();
			}
			OnOwnerGroupChanged(this, args2);
			list.Add(delegate
			{
				ClusterResourceOwnerGroupEventArgs eventArgs = notification.Payload as ClusterResourceOwnerGroupEventArgs;
				RouteEvent(new ClusterWrapperEventArgs(EventType.ResourceOwnerGroupChanged, eventArgs));
				args2.WaitAndExecuteWhenFinished(delegate
				{
					base.LockObject.Reader();
					try
					{
						base.Cluster.RealtimeCollections.Change(this, "OwnerGroup");
					}
					finally
					{
						base.LockObject.UnlockReader();
					}
				});
			});
		}
		else if (notification.Payload is ClusterStoragePropertiesChangedEventArgs)
		{
			ClusterStoragePropertiesChangedEventArgs args3 = (ClusterStoragePropertiesChangedEventArgs)notification.Payload;
			if (args3.Reload)
			{
				base.LoadedSelection &= -257;
			}
			DiskInfo = args3.Disk;
			list.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.ResourceStoragePropertiesChanged, args3));
			});
		}
		else if (notification.Payload is ClusterResourceFlagsEventArgs)
		{
			ClusterResourceFlagsEventArgs args4 = (ClusterResourceFlagsEventArgs)notification.Payload;
			Flags = args4.Flags;
			list.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.ResourceFlagsChanged, args4));
				base.Cluster.RealtimeCollections.Change(this, "Flags");
			});
		}
		else if (notification.Payload is ClusterCharacteristicsEventArgs)
		{
			ClusterCharacteristicsEventArgs args5 = (ClusterCharacteristicsEventArgs)notification.Payload;
			Characteristics = args5.Characteristics;
			list.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.ResourceCharacteristicsChanged, args5));
				base.Cluster.RealtimeCollections.Change(this, "Characteristics");
			});
		}
		else if (notification.Payload is ClusterRemovedEventArgs)
		{
			ClusterRemovedEventArgs args6 = (ClusterRemovedEventArgs)notification.Payload;
			args6.Cluster.CacheManager.CacheLock.EnterWriteLock();
			try
			{
				base.IsRemoved = true;
				args6.Cluster.CacheManager.RemoveObject(this);
				list.Add(delegate
				{
					RouteEvent(new ClusterWrapperEventArgs(EventType.Removed, notification.Payload));
					args6.Cluster.RealtimeCollections.Remove<Resource>(this);
				});
			}
			finally
			{
				args6.Cluster.CacheManager.CacheLock.ExitWriteLock();
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
		else if (notification.Payload is ClusterDependentsEventArgs)
		{
			ClusterDependentsEventArgs clusterDependentsEventArgs = (ClusterDependentsEventArgs)notification.Payload;
			Dependents = clusterDependentsEventArgs.Dependents;
			isChildLoaded = false;
			clusterDependentsEventArgs.IsChild = IsChild;
			list.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.ResourceDependentsChanged, notification.Payload));
				base.Cluster.RealtimeCollections.Change(this, "Dependents");
			});
		}
		else if (notification.Payload is ClusterDependenciesEventArgs)
		{
			ClusterDependenciesEventArgs clusterDependenciesEventArgs = (ClusterDependenciesEventArgs)notification.Payload;
			Dependencies = clusterDependenciesEventArgs.Dependencies;
			list.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.ResourceDependenciesChanged, notification.Payload));
				base.Cluster.RealtimeCollections.Change(this, "Dependencies");
			});
		}
		else if (notification.Payload is ClusterDependencyRelationshipEventArgs)
		{
			ClusterDependencyRelationshipEventArgs clusterDependencyRelationshipEventArgs = (ClusterDependencyRelationshipEventArgs)notification.Payload;
			DependencyRelationship = clusterDependencyRelationshipEventArgs.DependencyRelationship;
			list.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.ResourceDependencyRelationshipChanged, notification.Payload));
			});
		}
		else if (notification.Payload is ClusterRequiredDependenciesEventArgs)
		{
			ClusterRequiredDependenciesEventArgs clusterRequiredDependenciesEventArgs = (ClusterRequiredDependenciesEventArgs)notification.Payload;
			RequiredDependencies = clusterRequiredDependenciesEventArgs.RequiredDependencies;
			list.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.ResourceRequiredDependenciesChanged, notification.Payload));
			});
		}
		else if (notification.Payload is ClusterResourcePossibleOwnersChangedEventArgs)
		{
			ClusterResourcePossibleOwnersChangedEventArgs possibleOwnersArgs = notification.Payload as ClusterResourcePossibleOwnersChangedEventArgs;
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
				list.Add(delegate
				{
					RouteEvent(new ClusterWrapperEventArgs(EventType.ResourcePossibleOwnersChanged, new ClusterResourcePossibleOwnersChangedEventArgs(base.Id, PossibleOwners, null)));
				});
			}
		}
		return list;
	}

	public static bool ProcessNotificationSpecial(Notification notification)
	{
		if (notification.Payload is ClusterAddedEventArgs clusterAddedEventArgs)
		{
			Exceptions.ThrowIfNull(clusterAddedEventArgs.ObjectType, "ObjectType");
			PResourceType resourceType = clusterAddedEventArgs.Cluster.GetResourceType(clusterAddedEventArgs.ObjectTypeName);
			PResource pResource = Constructor(clusterAddedEventArgs.Cluster, clusterAddedEventArgs.Id, clusterAddedEventArgs.Name, resourceType);
			if (clusterAddedEventArgs is ClusterUpsertEventArgs)
			{
				pResource.LoadObject(5);
			}
			else
			{
				PGroup pGroup = null;
				using (ClusterLock clusterLock = clusterAddedEventArgs.Cluster.CacheManager.Get(clusterAddedEventArgs.ParentId, ClusterIdentityType.Group, LockAccess.Reader))
				{
					if (clusterLock != null)
					{
						pGroup = (PGroup)clusterLock.Owner;
					}
				}
				PResourceType pResourceType = null;
				using (ClusterLock clusterLock2 = clusterAddedEventArgs.Cluster.CacheManager.Get(pResource.ResourceType.Id, pResource.ResourceType.IdentityType, LockAccess.Reader))
				{
					if (clusterLock2 != null)
					{
						pResourceType = (PResourceType)clusterLock2.Owner;
					}
				}
				if (pGroup != null && pResourceType != null && (pResourceType.LoadedSelection & 1) == 1)
				{
					pResource.OwnerGroup = pGroup;
					pResource.ResourceType = pResourceType;
					pResource.ResourceState = FailoverClusters.Framework.ResourceState.Offline;
					pResource.Flags = ResourceFlags.None;
					pResource.Class = pResource.ResourceType.Class;
					pResource.Subclass = pResource.ResourceType.Subclass;
					pResource.Characteristics = pResource.ResourceType.Characteristics;
					pResource.LoadedSelection |= 1;
				}
			}
			pResource = (PResource)clusterAddedEventArgs.Cluster.CacheManager.AddObject(pResource, cacheIsLocked: false, clusterAddedEventArgs is ClusterUpsertEventArgs);
			pResource.LockObject.Reader();
			try
			{
				clusterAddedEventArgs.Cluster.RealtimeCollections.Add(pResource, (clusterAddedEventArgs is ClusterUpsertEventArgs) ? RTCOperation.Replace : RTCOperation.Add);
			}
			finally
			{
				pResource.LockObject.UnlockReader();
			}
			return true;
		}
		if (notification.Payload is ClusterPropertiesEventArgs clusterPropertiesEventArgs)
		{
			Exceptions.ThrowIfNull(clusterPropertiesEventArgs.ObjectType, "ObjectType");
			PResource clusterObject;
			if (clusterPropertiesEventArgs.ObjectType.Value != 9999)
			{
				clusterObject = Constructor(clusterPropertiesEventArgs.Cluster, clusterPropertiesEventArgs.Id, clusterPropertiesEventArgs.Name, new PResourceType(clusterPropertiesEventArgs.Cluster, (ResourceKind)clusterPropertiesEventArgs.ObjectType.Value));
			}
			else
			{
				string type = clusterPropertiesEventArgs.Cluster.Server.Resource.GetType(clusterPropertiesEventArgs.Id, clusterPropertiesEventArgs.Name);
				clusterObject = Constructor(clusterPropertiesEventArgs.Cluster, clusterPropertiesEventArgs.Id, clusterPropertiesEventArgs.Name, new PResourceType(clusterPropertiesEventArgs.Cluster, type));
			}
			PResource privateClusterObject = (PResource)clusterPropertiesEventArgs.Cluster.CacheManager.AddObject(clusterObject);
			clusterPropertiesEventArgs.Cluster.RealtimeCollections.Add(privateClusterObject, RTCOperation.Add);
			return true;
		}
		return false;
	}

	private static void AddFactoryEntry(ResourceKind resourceKind, Func<PCluster, Guid, string, PResource> privateResourceAction, Func<Cluster, Resource> proxyResourceAction)
	{
		FactoryDictionary.TryAdd(resourceKind, privateResourceAction);
		FactoryProxyDictionary.TryAdd(resourceKind, proxyResourceAction);
	}

	private static void CreateFactoryDictionaries()
	{
		AddFactoryEntry(ResourceKind.DistributedFileSystem, (PCluster cluster, Guid id, string name) => new PDfsNamespaceResource(cluster, id, name), (Cluster cluster) => new DfsNamespaceResource(cluster));
		AddFactoryEntry(ResourceKind.DfsReplicatedFolder, (PCluster cluster, Guid id, string name) => new PDfsReplicatedFolderResource(cluster, id, name), (Cluster cluster) => new DfsReplicatedFolderResource(cluster));
		AddFactoryEntry(ResourceKind.DhcpService, (PCluster cluster, Guid id, string name) => new PDhcpServiceResource(cluster, id, name), (Cluster cluster) => new DhcpServiceResource(cluster));
		AddFactoryEntry(ResourceKind.ScaleOutFileServer, (PCluster cluster, Guid id, string name) => new PDistributedFileServerResource(cluster, id, name), (Cluster cluster) => new DistributedFileServerResource(cluster));
		AddFactoryEntry(ResourceKind.DistributedNetworkName, (PCluster cluster, Guid id, string name) => new PDistributedNetworkNameResource(cluster, id, name), (Cluster cluster) => new DistributedNetworkNameResource(cluster));
		AddFactoryEntry(ResourceKind.Dtc, (PCluster cluster, Guid id, string name) => new PDtcResource(cluster, id, name), (Cluster cluster) => new DtcResource(cluster));
		AddFactoryEntry(ResourceKind.FileServer, (PCluster cluster, Guid id, string name) => new PFileServerResource(cluster, id, name), (Cluster cluster) => new FileServerResource(cluster));
		AddFactoryEntry(ResourceKind.FileShareWitness, (PCluster cluster, Guid id, string name) => new PFileShareWitnessResource(cluster, id, name), (Cluster cluster) => new FileShareWitnessResource(cluster));
		AddFactoryEntry(ResourceKind.CloudWitness, (PCluster cluster, Guid id, string name) => new PCloudWitnessResource(cluster, id, name), (Cluster cluster) => new CloudWitnessResource(cluster));
		AddFactoryEntry(ResourceKind.GenericApplication, (PCluster cluster, Guid id, string name) => new PGenericApplicationResource(cluster, id, name), (Cluster cluster) => new GenericApplicationResource(cluster));
		AddFactoryEntry(ResourceKind.GenericScript, (PCluster cluster, Guid id, string name) => new PGenericScriptResource(cluster, id, name), (Cluster cluster) => new GenericScriptResource(cluster));
		AddFactoryEntry(ResourceKind.GenericService, (PCluster cluster, Guid id, string name) => new PGenericServiceResource(cluster, id, name), (Cluster cluster) => new GenericServiceResource(cluster));
		AddFactoryEntry(ResourceKind.IPAddress, (PCluster cluster, Guid id, string name) => new PIPAddressResource(cluster, id, name), (Cluster cluster) => new IPAddressResource(cluster));
		AddFactoryEntry(ResourceKind.IPv6Address, (PCluster cluster, Guid id, string name) => new PIPv6AddressResource(cluster, id, name), (Cluster cluster) => new IPv6AddressResource(cluster));
		AddFactoryEntry(ResourceKind.IPv6TunnelAddress, (PCluster cluster, Guid id, string name) => new PIPv6TunnelAddressResource(cluster, id, name), (Cluster cluster) => new IPv6TunnelAddressResource(cluster));
		AddFactoryEntry(ResourceKind.IScsiNameService, (PCluster cluster, Guid id, string name) => new PIScsiNameServiceResource(cluster, id, name), (Cluster cluster) => new IScsiNameServiceResource(cluster));
		AddFactoryEntry(ResourceKind.IScsiTarget, (PCluster cluster, Guid id, string name) => new PIScsiTargetResource(cluster, id, name), (Cluster cluster) => new IScsiTargetResource(cluster));
		AddFactoryEntry(ResourceKind.Msmsq, (PCluster cluster, Guid id, string name) => new PMsmqResource(cluster, id, name), (Cluster cluster) => new MsmqResource(cluster));
		AddFactoryEntry(ResourceKind.MsmsqTrigger, (PCluster cluster, Guid id, string name) => new PMsmqTriggerResource(cluster, id, name), (Cluster cluster) => new MsmqTriggerResource(cluster));
		AddFactoryEntry(ResourceKind.NetworkName, (PCluster cluster, Guid id, string name) => new PNetNameResource(cluster, id, name), (Cluster cluster) => new NetNameResource(cluster));
		AddFactoryEntry(ResourceKind.NfsShare, (PCluster cluster, Guid id, string name) => new PNfsShareResource(cluster, id, name), (Cluster cluster) => new NfsShareResource(cluster));
		AddFactoryEntry(ResourceKind.ClusterFileSystem, (PCluster cluster, Guid id, string name) => new PCsvVolumeResource(cluster, id, name), (Cluster cluster) => new CsvVolumeResource(cluster));
		AddFactoryEntry(ResourceKind.PhysicalDisk, (PCluster cluster, Guid id, string name) => new PStorageResource(cluster, id, name), (Cluster cluster) => new StorageResource(cluster));
		AddFactoryEntry(ResourceKind.VirtualMachine, (PCluster cluster, Guid id, string name) => new PVirtualMachineResource(cluster, id, name), (Cluster cluster) => new VirtualMachineResource(cluster));
		AddFactoryEntry(ResourceKind.VirtualMachineReplicationBroker, (PCluster cluster, Guid id, string name) => new PVMReplicaBrokerResource(cluster, id, name), (Cluster cluster) => new VMReplicaBrokerResource(cluster));
		AddFactoryEntry(ResourceKind.VirtualMachineConfiguration, (PCluster cluster, Guid id, string name) => new PVirtualMachineConfigurationResource(cluster, id, name), (Cluster cluster) => new VirtualMachineConfigurationResource(cluster));
		AddFactoryEntry(ResourceKind.VolumeShadowCopyServiceTask, (PCluster cluster, Guid id, string name) => new PVolumeShadowCopyServiceTaskResource(cluster, id, name), (Cluster cluster) => new VolumeShadowCopyServiceTaskResource(cluster));
		AddFactoryEntry(ResourceKind.WinsService, (PCluster cluster, Guid id, string name) => new PWinsServiceResource(cluster, id, name), (Cluster cluster) => new WinsServiceResource(cluster));
		AddFactoryEntry(ResourceKind.NetworkFileSystem, (PCluster cluster, Guid id, string name) => new PNetworkFileSystemResource(cluster, id, name), (Cluster cluster) => new NetworkFileSystemResource(cluster));
		AddFactoryEntry(ResourceKind.StoragePool, (PCluster cluster, Guid id, string name) => new PStoragePoolResource(cluster, id, name), (Cluster cluster) => new StoragePoolResource(cluster));
		AddFactoryEntry(ResourceKind.TaskScheduler, (PCluster cluster, Guid id, string name) => new PTaskSchedulerResource(cluster, id, name), (Cluster cluster) => new TaskSchedulerResource(cluster));
		AddFactoryEntry(ResourceKind.ClusterAwareUpdating, (PCluster cluster, Guid id, string name) => new PClusterAwareUpdatingResource(cluster, id, name), (Cluster cluster) => new ClusterAwareUpdatingResource(cluster));
		AddFactoryEntry(ResourceKind.HyperVNetworkVirtualizationProviderAddress, (PCluster cluster, Guid id, string name) => new PHyperVNetworkVirtualizationPAResource(cluster, id, name), (Cluster cluster) => new HyperVNetworkVirtualizationPAResource(cluster));
		AddFactoryEntry(ResourceKind.NetworkAddressTranslator, (PCluster cluster, Guid id, string name) => new PNetworkAddressTranslatorResource(cluster, id, name), (Cluster cluster) => new NetworkAddressTranslatorResource(cluster));
		AddFactoryEntry(ResourceKind.DisjointIPv4Address, (PCluster cluster, Guid id, string name) => new PVirtualIPv4AddressResource(cluster, id, name), (Cluster cluster) => new VirtualIPv4AddressResource(cluster));
		AddFactoryEntry(ResourceKind.DisjointIPv6Address, (PCluster cluster, Guid id, string name) => new PVirtualIPv6AddressResource(cluster, id, name), (Cluster cluster) => new VirtualIPv6AddressResource(cluster));
		AddFactoryEntry(ResourceKind.StorageReplica, (PCluster cluster, Guid id, string name) => new PStorageReplicaResource(cluster, id, name), (Cluster cluster) => new StorageReplicaResource(cluster));
		AddFactoryEntry(ResourceKind.HealthService, (PCluster cluster, Guid id, string name) => new PHealthServiceResource(cluster, id, name), (Cluster cluster) => new HealthServiceResource(cluster));
		AddFactoryEntry(ResourceKind.StorageQoS, (PCluster cluster, Guid id, string name) => new PStorageQoSResource(cluster, id, name), (Cluster cluster) => new StorageQoSResource(cluster));
		AddFactoryEntry(ResourceKind.HyperVClusterWmi, (PCluster cluster, Guid id, string name) => new PHyperVClusterWmiResource(cluster, id, name), (Cluster cluster) => new HyperVClusterWmiResource(cluster));
		AddFactoryEntry(ResourceKind.SDDCManagement, (PCluster cluster, Guid id, string name) => new PSDDCManagementResource(cluster, id, name), (Cluster cluster) => new SDDCManagementResource(cluster));
		AddFactoryEntry(ResourceKind.CrossClusterOrchestrator, (PCluster cluster, Guid id, string name) => new PCrossClusterDependencyOrchestratorResource(cluster, id, name), (Cluster cluster) => new CrossClusterDependencyOrchestratorResource(cluster));
		AddFactoryEntry(ResourceKind.VirtualMachineReplicationCoordinator, (PCluster cluster, Guid id, string name) => new PVirtualMachineReplicationCoordinatorResource(cluster, id, name), (Cluster cluster) => new VirtualMachineReplicationCoordinatorResource(cluster));
	}
}

