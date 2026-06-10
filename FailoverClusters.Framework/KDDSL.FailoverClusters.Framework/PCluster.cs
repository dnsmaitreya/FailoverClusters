using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using FailoverClusters.UI.Common.Services;

namespace KDDSL.FailoverClusters.Framework;

internal class PCluster : PClusterObject<Cluster>
{
	private readonly Cluster proxyCluster;

	private readonly NotificationManager notificationManager;

	private readonly object openCloseLock = new object();

	private static readonly List<string> OptimizedQueries;

	private List<PGroup> coreGroups = new List<PGroup>();

	private int refreshing;

	private QuorumConfigurationPrivate quorumConfiguration;

	public Guid CacheId { get; private set; }

	public ClusterObjectLoader ClusterObjectLoader { get; private set; }

	public IConnectionAdapter Server { get; private set; }

	public PGroup AvailableStorage { get; private set; }

	public PGroup CoreGroup { get; private set; }

	public IVirtualizationAdapter Virtualization { get; private set; }

	public IFileShareAdapter FileServer { get; private set; }

	public ClusterGC ClusterGc { get; private set; }

	public RealtimeCollections RealtimeCollections { get; private set; }

	public string FqdnName { get; internal set; }

	public CacheManager CacheManager { get; internal set; }

	public ClusterAccessRights ClusterAccessRights { get; internal set; }

	public ManagementPointType ManagementPointType { get; internal set; }

	public short MajorVersion { get; internal set; }

	public short MinorVersion { get; internal set; }

	public short BuildNumber { get; internal set; }

	public string Version { get; internal set; }

	public OSVersion OSVersion { get; internal set; }

	public string VendorId { get; internal set; }

	public string CSDVersion { get; internal set; }

	public int ClusterHighestVersion { get; internal set; }

	public int ClusterLowestVersion { get; internal set; }

	public int Flags { get; internal set; }

	public int Reserved { get; internal set; }

	public string SharedVolumesRootPath { get; internal set; }

	public uint FunctionalLevel { get; internal set; }

	public string HostNode { get; internal set; }

	public IEnumerable<PGroup> CoreGroups => coreGroups;

	public override ClusterIdentityType IdentityType => ClusterIdentityType.Cluster;

	public string ConnectedTo { get; internal set; }

	internal QuorumConfigurationPrivate QuorumConfiguration
	{
		get
		{
			return quorumConfiguration;
		}
		set
		{
			if (QuorumConfigurationPrivateExtensions.Equals(quorumConfiguration, value))
			{
				return;
			}
			QuorumConfigurationPrivate quorumConfigurationPrivate = quorumConfiguration;
			quorumConfiguration = value;
			if (quorumConfigurationPrivate != null && quorumConfigurationPrivate.QuorumResourceId != Guid.Empty)
			{
				ClusterLock clusterLock = CacheManager.Get(quorumConfigurationPrivate.QuorumResourceId, ClusterIdentityType.Resource, LockAccess.Writer);
				if (clusterLock != null)
				{
					try
					{
						if (clusterLock.Owner is PResource pResource)
						{
							pResource.IsQuorum = false;
						}
					}
					finally
					{
						clusterLock.UnlockWriter();
					}
				}
			}
			if (value == null || !(value.QuorumResourceId != Guid.Empty))
			{
				return;
			}
			ClusterLock clusterLock2 = CacheManager.Get(value.QuorumResourceId, ClusterIdentityType.Resource, LockAccess.Writer);
			if (clusterLock2 == null)
			{
				CacheManager.AddObject(this, ClusterIdentityType.Resource, value.QuorumResourceId);
				clusterLock2 = CacheManager.Get(value.QuorumResourceId, ClusterIdentityType.Resource, LockAccess.Writer);
			}
			if (clusterLock2 == null)
			{
				return;
			}
			try
			{
				if (clusterLock2.Owner is PResource pResource2)
				{
					pResource2.LoadObject(1);
					pResource2.IsQuorum = true;
				}
			}
			finally
			{
				clusterLock2.UnlockWriter();
			}
		}
	}

	static PCluster()
	{
		OptimizedQueries = new List<string>();
		OptimizedQueries.Add(CreateRegExQuery("select [Type], [Id], [Name] from resource\r\n"));
		OptimizedQueries.Add(CreateRegExQuery("select [type], [id], [name] from resource\r\n where ([resourceclass] = 1)\r\n"));
		OptimizedQueries.Add(CreateRegExQuery("select [type], [id], [name] from resource\r\n where (([resourceclass] = 1) and ([type] <> 'storage pool'))\r\n"));
		OptimizedQueries.Add(CreateRegExQuery("select [type], [id], [name] from resource\r\n where (([ownergroup] = 'ANY') and (([type] = 'network name') or ([type] = 'distributed network name')))"));
		OptimizedQueries.Add(CreateRegExQuery("select [Type], [Id], [Name] from resource\r\n where (([ownergroup] = 'ANY') and ([type] = 'virtual machine configuration'))\r\n"));
		OptimizedQueries.Add(CreateRegExQuery("select [Type], [Id], [Name] FROM Resource\r\n WHERE (([OwnerGroup] = 'ANY') AND (((([Type] = 'Network Name') OR ([Type] = 'Distributed Network Name')) OR ([Type] = 'Scale Out File Server')) OR ([Resourceclass] = 1)))\r\n ORDER BY [Type] ASC\r\n"));
		OptimizedQueries.Add(CreateRegExQuery("SELECT [Type], [Id], [Name] FROM Resource\r\n WHERE (([OwnerGroup] = 'ANY') AND (([Type] = 'File Server') OR ([Type] = 'Scale Out File Server')))\r\n"));
		OptimizedQueries.Add(CreateRegExQuery("SELECT [Type], [Id], [Name] FROM Resource\r\n WHERE ((([OwnerGroup] = 'ANY') AND ([IsChild] = 0)) OR ''[Id])\r\n"));
		OptimizedQueries.Add(CreateRegExQuery("SELECT [Type], [Id], [Name] FROM Resource\r\n WHERE (([OwnerGroup] = 'ANY') AND ([type] = 'Virtual Machine'))\r\n"));
		OptimizedQueries.Add(CreateRegExQuery("SELECT [Type], [Id], [Name] FROM Resource\r\n WHERE ((([OwnerGroup] = 'ANY') AND ([IsChild] = 0)) OR '[ANY]'[Id])\r\n"));
		OptimizedQueries.Add(CreateRegExQuery("SELECT [Type], [Id], [Name] FROM Resource\r\n WHERE (([Type] = 'Physical Disk') AND ([PoolId] = 'ANY'))\r\n ORDER BY [Name] ASC\r\n"));
		OptimizedQueries.Add(CreateRegExQuery("SELECT [Id], [Name] FROM Cluster\r\n"));
	}

	public PCluster(Guid id)
		: base((PCluster)null)
	{
		base.IsOpen = false;
		base.Id = id;
		CacheId = Guid.NewGuid();
		Virtualization = ServiceContainer.Container.Resolve<IVirtualizationAdapter>(new object[1] { this });
		FileServer = ServiceContainer.Container.Resolve<IFileShareAdapter>(Array.Empty<object>());
		notificationManager = ServiceContainer.Container.Resolve<NotificationManager>(new object[1] { this });
		Server = ServiceContainer.Container.Resolve<IConnectionAdapter>(new object[1] { this });
		proxyCluster = new Cluster(Server.Adapter);
		proxyCluster.TransferInternalData(this, subscribeToEvents: true);
	}

	public PNode AddNode(string name)
	{
		PNode newNode = null;
		ProtectedScope(delegate
		{
			newNode = base.Cluster.Server.Node.Add(name);
		}, delegate(ClusterException ex)
		{
			if (ex == null)
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.Created, new ClusterAddedEventArgs(newNode.Id, newNode.Name, null, null)));
			}
		});
		return newNode;
	}

	public void AddVirtualMachine(Guid vmId, string ownerNodeName)
	{
		ProtectedScope(delegate
		{
			base.Cluster.Server.Cluster.AddVirtualMachine(vmId, ownerNodeName);
		});
	}

	public override void Delete()
	{
		ProtectedScope(delegate
		{
			Server.Cluster.Destroy(deleteComputerObjects: false);
		}, delegate(ClusterException ex)
		{
			if (ex != null)
			{
				base.IsProcessing = false;
			}
		}, resetIsProcessing: false);
	}

	public void RefreshReplication()
	{
		if (Interlocked.CompareExchange(ref refreshing, 1, 0) == 0)
		{
			try
			{
				LoadReplicationInformation();
			}
			finally
			{
				Interlocked.Decrement(ref refreshing);
			}
		}
	}

	public override void Refresh(bool targeted)
	{
		if (Interlocked.CompareExchange(ref refreshing, 1, 0) != 0)
		{
			return;
		}
		try
		{
			ClusterGc.Collect();
			base.LoadedSelection = 0;
			RouteEvent(new ClusterWrapperEventArgs(EventType.Refreshed, new ClusterRefreshedEventArgs(base.Id, base.Name, targeted)));
			LoadObject(4111);
			Server.PauseNotifications();
			try
			{
				if (targeted)
				{
					CacheManager.Invalidate();
					LoadReplicationInformation();
					PopulateCache();
				}
				else
				{
					CacheManager.Obliterate(delegate
					{
						RealtimeCollections.RunOperation(RTCMaintenance.Clean);
					});
					LoadReplicationInformation();
					PopulateCache();
				}
				SetupCoreGroups();
			}
			finally
			{
				Server.ResetNotifications();
				Server.ResumeNotifications();
			}
			ClusterGc.Collect();
			RealtimeCollections.RunOperation(RTCMaintenance.Refresh);
			RouteEvent(new ClusterWrapperEventArgs(EventType.ClusterRefresh, new ClusterRefreshedEventArgs(base.Id, base.Name, targeted)));
		}
		finally
		{
			Interlocked.Decrement(ref refreshing);
		}
	}

	public void Open(string clusterName, params object[] parameters)
	{
		lock (openCloseLock)
		{
			if (base.IsOpen)
			{
				return;
			}
			Guid newId = Guid.Empty;
			ProtectedScope(delegate
			{
				if (parameters != null && parameters.Length != 0 && parameters[0] is SafeClusterHandle && parameters[2] is int && Server is ClusApiAdapter)
				{
					ClusApiAdapter clusApiAdapter = Server as ClusApiAdapter;
					newId = clusApiAdapter.Cluster.Open((SafeClusterHandle)parameters[0]);
					ClusterAccessRights = (ClusterAccessRights)parameters[2];
				}
				else
				{
					newId = Server.Cluster.Open(clusterName, ClusterAccessRights.GenericAll, out var grantedAccess);
					ClusterAccessRights = grantedAccess;
				}
				CacheManager = new CacheManager();
				RealtimeCollections = new RealtimeCollections();
				ClusterGc = new ClusterGC(this);
				ClusterObjectLoader = new ClusterObjectLoader(base.Name);
				base.IsOpen = true;
				LoadObject(4111);
				base.Properties.Get("SharedVolumesRoot", delegate(ClusterPropertyString property)
				{
					SharedVolumesRootPath = property.TypedValue;
				});
				base.Properties.Get("ClusterFunctionalLevel", delegate(ClusterPropertyUInt property)
				{
					FunctionalLevel = property.TypedValue;
				});
				if (FunctionalLevel == 0)
				{
					FunctionalLevel = NativeMethods.NT9_MAJOR_VERSION;
				}
				base.Properties.GetOrNull("AdminAccessPoint", delegate(ClusterPropertyUInt property)
				{
					ManagementPointType = (ManagementPointType)(property?.TypedValue ?? 1);
				});
				HostNode = null;
				notificationManager.Subscribe(Server);
				Server.PauseNotifications();
				SetupCoreGroups();
				LoadReplicationInformation();
				Server.ResumeNotifications();
				Task.Factory.StartNew(PopulateCache);
			}, delegate(ClusterException ex)
			{
				if (ex == null)
				{
					RouteEvent(new ClusterWrapperEventArgs(EventType.IdChanged, new ClusterIdChangedEventArgs(base.Id, newId, null)));
					RouteEvent(new ClusterWrapperEventArgs(EventType.Renamed, new ClusterRenamedEventArgs(base.Id, clusterName, null)));
					RouteEvent(new ClusterWrapperEventArgs(EventType.Opened, new ClusterOpenEventArgs(newId, base.IsOpen, null)));
					RouteEvent(new ClusterWrapperEventArgs(EventType.PropertiesChanged, new ClusterPropertiesEventArgs(base.Id, base.Name, null, null)
					{
						Cluster = this,
						Properties = base.Properties
					}));
					ServiceContainer.Container.Resolve<IClusterDataChangedService>(Array.Empty<object>()).NotifyClustersDataChanged(this, ClusterDataChangedType.Addition);
				}
				else
				{
					base.IsOpen = false;
					RouteEvent(new ClusterWrapperEventArgs(EventType.Opened, new ClusterOpenEventArgs(newId, base.IsOpen, ex)));
				}
			});
		}
	}

	private void LoadReplicationInformation()
	{
		CacheManager.ReplaceReplicatedResources(Server.ResourceType.GetReplicationResources());
		CacheManager.ReplaceReplicatedGroups(Server.ResourceType.GetReplicationGroups());
	}

	private void SetupCoreGroups()
	{
		coreGroups.Clear();
		Server.Cluster.CoreGroups.ForEach(delegate(Guid coreGroupGuid)
		{
			PGroup pGroup = (PGroup)CacheManager.AddObject(this, ClusterIdentityType.Group, coreGroupGuid);
			coreGroups.Add(pGroup);
			pGroup.LoadObject(15);
			if (pGroup.GroupType == GroupType.AvailableStorage)
			{
				AvailableStorage = pGroup;
			}
			else if (pGroup.GroupType == GroupType.CoreCluster)
			{
				CoreGroup = pGroup;
				HostNode = pGroup.OwnerNode.Name;
			}
		});
	}

	public bool Close()
	{
		lock (openCloseLock)
		{
			if (!base.IsOpen)
			{
				return true;
			}
			ProtectedScope(delegate
			{
				try
				{
					Thread.Sleep(100);
					notificationManager.Unsubscribe();
				}
				catch (Exception)
				{
				}
				try
				{
					Thread.Sleep(100);
					ClusterGc.Unload();
				}
				catch (Exception)
				{
				}
				try
				{
					Thread.Sleep(100);
					ClusterObjectLoader.Unload();
				}
				catch (Exception)
				{
				}
				try
				{
					Thread.Sleep(100);
					RealtimeCollections.Dispose();
				}
				catch (Exception)
				{
				}
				Thread.Sleep(100);
				Server.Close();
			}, delegate(ClusterException ex)
			{
				base.IsOpen = ex != null && ex.InnerException != null && !(ex.InnerException is ThreadAbortException);
				if (!base.IsOpen)
				{
					ServiceContainer.Container.Resolve<IClusterDataChangedService>(Array.Empty<object>()).NotifyClustersDataChanged(this, ClusterDataChangedType.Removal);
				}
				RouteEvent(new ClusterWrapperEventArgs(EventType.Opened, new ClusterOpenEventArgs(base.Id, base.IsOpen, ex)));
			});
		}
		return true;
	}

	public override Cluster GetProxy()
	{
		return GetProxy(ProxyCreateMode.Singleton);
	}

	public override Cluster GetProxy(ProxyCreateMode createMode)
	{
		return proxyCluster;
	}

	public override ClusterLoadedEventArgs LoadObject(int loadSelectionNeutral)
	{
		if ((base.LoadedSelection & loadSelectionNeutral) == loadSelectionNeutral)
		{
			return new ClusterLoadedEventArgs(base.Id, loaded: true, base.LoadedSelection, null);
		}
		ClusterLoadedEventArgs loadedArgs = null;
		int currentSelection = base.LoadedSelection;
		ClusterLoadSelection loadSelection = (ClusterLoadSelection)loadSelectionNeutral;
		ProtectedScope(delegate
		{
			base.Cluster.Server.Cluster.Load(this, loadSelection);
		}, delegate(ClusterException ex)
		{
			if (ex == null)
			{
				int loadSelection2 = currentSelection ^ base.LoadedSelection;
				BroadcastChanges(loadSelection2);
			}
			loadedArgs = new ClusterLoadedEventArgs(base.Id, ex == null, (int)loadSelection, ex);
			RouteEvent(new ClusterWrapperEventArgs(EventType.Loaded, loadedArgs));
		}, resetIsProcessing: false, affectsIsProcessing: false);
		loadedArgs = new ClusterLoadedEventArgs(base.Id, loaded: true, base.LoadedSelection, null);
		return loadedArgs;
	}

	public void Shutdown()
	{
		ProtectedScope(delegate
		{
			Server.Cluster.Shutdown();
		}, delegate(ClusterException ex)
		{
			if (ex != null)
			{
				base.IsProcessing = false;
			}
		}, resetIsProcessing: false);
	}

	internal IEnumerable<PResource> CreateDiskResources(IEnumerable<ClusterDisk> clusterableDisks)
	{
		return Server.Cluster.CreateDiskResources(clusterableDisks);
	}

	internal IEnumerable<ClusterDisk> GetAvailableDisks(Guid poolId)
	{
		return GetAvailableDisks(poolId, all: false);
	}

	internal IEnumerable<ClusterDisk> GetAvailableDisks(Guid poolId, bool all)
	{
		return Server.Cluster.GetAvailableDisks(poolId, all);
	}

	internal void GetClusterablePools(Action<ClusterableStoragePool> onNext, Action<Exception> onError, Action onCompleted)
	{
		Server.Cluster.GetClusterableStoragePools(onNext, onError, onCompleted);
	}

	internal void AddStoragePool(ClusterableStoragePool pool, Action<Exception> onError, Action onCompleted)
	{
		Server.Cluster.AddStoragePoolToCluster(pool, onError, onCompleted);
	}

	public override void BroadcastChanges(int loadSelection, bool raiseLoadedEvent = false)
	{
		List<ClusterWrapperEventArgs> list = new List<ClusterWrapperEventArgs>(10);
		if (((ClusterLoadSelection)loadSelection).HasFlag(ClusterLoadSelection.CommonProperties) || ((ClusterLoadSelection)loadSelection).HasFlag(ClusterLoadSelection.PrivateProperties))
		{
			list.Add(new ClusterWrapperEventArgs(EventType.PropertiesChanged, new ClusterPropertiesEventArgs(base.Id, base.Name, null, null)
			{
				Properties = base.Properties
			}));
		}
		if (raiseLoadedEvent)
		{
			ClusterLoadedEventArgs eventArgs = new ClusterLoadedEventArgs(base.Id, loaded: true, base.LoadedSelection, null);
			list.Add(new ClusterWrapperEventArgs(EventType.Loaded, eventArgs));
		}
		if (((ClusterLoadSelection)loadSelection).HasFlag(ClusterLoadSelection.Basic))
		{
			list.Add(new ClusterWrapperEventArgs(EventType.ConnectedToNodeChanged, new ClusterConnectedToNodeChangedEventArgs(base.Id, ConnectedTo)));
		}
		if (((ClusterLoadSelection)loadSelection).HasFlag(ClusterLoadSelection.QuorumConfiguration))
		{
			list.Add(new ClusterWrapperEventArgs(EventType.QuorumConfigurationChanged, new ClusterQuorumChangedEventArgs(base.Id, quorumConfiguration)));
		}
		RouteEvent(new ClusterWrapperEventArgs(EventType.BatchChanges, new ClusterBatchChangesEventArgs(base.Id, list)));
	}

	public override void Rename(string newName)
	{
		ProtectedScope(delegate
		{
			Server.Cluster.Rename(newName);
		}, delegate(ClusterException ex)
		{
			RouteEvent(new ClusterWrapperEventArgs(EventType.Renamed, new ClusterRenamedEventArgs(base.Id, newName, ex)));
		});
	}

	public PGroup CreateGroup(string name, GroupType groupType)
	{
		PGroup newGroup = null;
		ProtectedScope(delegate
		{
			newGroup = Server.Group.Create(name, groupType);
		}, delegate(ClusterException ex)
		{
			if (ex == null && newGroup != null)
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.Created, new ClusterAddedEventArgs(newGroup.Id, newGroup.Name, (int)groupType, null)));
			}
		});
		return newGroup;
	}

	private IList<T> PopulateCacheFromObjectType<T>(IEnumerable<T> allObjectsEnumeration, Action onSuccess) where T : PClusterObject
	{
		string text = typeof(T).Name;
		ClusterLog.LogVerbose(LogSubcategory.FxCache, "Filling cache for {0} started".FormatCurrentCulture(text));
		bool flag = true;
		List<T> list = new List<T>();
		try
		{
			foreach (T item in allObjectsEnumeration)
			{
				if (!base.IsOpen)
				{
					ClusterLog.LogVerbose(LogSubcategory.FxCache, "Fill of the cache for {0} has been aborted since the cluster has been closed".FormatCurrentCulture(text));
					break;
				}
				if (item == null)
				{
					flag = false;
					continue;
				}
				try
				{
					list.Add((T)CacheManager.AddObject(item));
				}
				catch (ClusterException exception)
				{
					flag = false;
					ClusterLog.LogException(exception, "Failed to add '{0}' '{1} to cache.", item.Name, text);
				}
			}
			if (flag)
			{
				ClusterLog.LogVerbose(LogSubcategory.FxCache, "Filling cache for {0} completed successfully.".FormatCurrentCulture(text));
				onSuccess.SafeCall();
			}
			else
			{
				ClusterLog.LogVerbose(LogSubcategory.FxCache, "Filling cache for {0} partially completed.".FormatCurrentCulture(text));
			}
			return list;
		}
		catch (ClusterException ex)
		{
			ClusterLog.LogException(ex, "There was an error populating the {0} cache".FormatCurrentCulture(text));
			if (ex is ClusterClosedException)
			{
				throw;
			}
		}
		catch (Win32Exception exception2)
		{
			ClusterLog.LogException(exception2, "There was an error populating the {0} cache".FormatCurrentCulture(text));
		}
		return null;
	}

	public PResourceType CreateResourceType(string name, string displayName, string pathDll)
	{
		PResourceType newResourceType = null;
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				newResourceType = base.Cluster.Server.ResourceType.Create(name, displayName, pathDll);
			});
		}, delegate(ClusterException ex)
		{
			if (ex == null && newResourceType != null)
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.Created, new ClusterAddedEventArgs(newResourceType.Id, newResourceType.Name, (int)newResourceType.ResourceKind, newResourceType.Name, base.Id, null)));
			}
		});
		return newResourceType;
	}

	private void NotifyToProxies<T>(IEnumerable<T> cacheObjects) where T : PClusterObject
	{
		if (cacheObjects == null)
		{
			ClusterLog.LogVerbose(LogSubcategory.FxCache, "{0} collection is empty, node cache was not loaded...".FormatCurrentCulture(typeof(T)));
			return;
		}
		ClusterLog.LogVerbose(LogSubcategory.FxCache, "Notify {0} Proxy object about changes started".FormatCurrentCulture(typeof(T)));
		foreach (T cacheObject in cacheObjects)
		{
			if (!base.IsOpen)
			{
				ClusterLog.LogVerbose(LogSubcategory.FxCache, "Notify {0} Proxy object about changes has been aborted since the cluster has been closed".FormatCurrentCulture(typeof(T)));
				break;
			}
			if (Global.DefaultDispatcherPendingOperations > 10)
			{
				Thread.Sleep(Global.DefaultDispatcherPendingOperations / 10);
			}
			if ((cacheObject.LoadedSelection & 1) == 1)
			{
				cacheObject.BroadcastChanges(1, raiseLoadedEvent: true);
			}
		}
		ClusterLog.LogVerbose(LogSubcategory.FxCache, "Notify {0} Proxy object about changes completed".FormatCurrentCulture(typeof(T)));
	}

	private void PopulateVirtualMachineResourcesInGroups(IList<PResource> resourcesInCache)
	{
		try
		{
			if (resourcesInCache == null)
			{
				ClusterLog.LogVerbose(LogSubcategory.FxCache, "Resources collection is empty, Virtual Machine resources were not loaded in groups");
				return;
			}
			ClusterLog.LogVerbose(LogSubcategory.FxCache, "Filling Virtual Machine resources in each Virtual Machine Group and notify about the changes");
			if (Server is ClusApiAdapter clusApiAdapter)
			{
				clusApiAdapter.PopulateVirtualMachineStateProperties(resourcesInCache);
			}
			foreach (PResource item in resourcesInCache)
			{
				if (item.ResourceType.ResourceKind != ResourceKind.VirtualMachine)
				{
					continue;
				}
				PVirtualMachineResource pVirtualMachineResource = (PVirtualMachineResource)item;
				if (!(item.OwnerGroup is PVirtualMachineGroup pVirtualMachineGroup))
				{
					continue;
				}
				pVirtualMachineResource.LockObject.Writer();
				ClusterPropertyUInt clusterPropertyUInt;
				try
				{
					if (!pVirtualMachineResource.Properties.Contains("VmState"))
					{
						pVirtualMachineResource.LoadObject(4);
					}
					if (!pVirtualMachineResource.Properties.Contains("ResourceSpecificData2"))
					{
						pVirtualMachineResource.LoadObject(2);
					}
					clusterPropertyUInt = (ClusterPropertyUInt)pVirtualMachineResource.Properties["VmState"];
				}
				finally
				{
					pVirtualMachineResource.LockObject.UnlockWriter();
				}
				if (clusterPropertyUInt != null)
				{
					pVirtualMachineGroup.AddVirtualMachineChildResource(pVirtualMachineResource, (VirtualMachineState)clusterPropertyUInt.TypedValue);
					ClusterGainedEventArgs eventArgs = new ClusterGainedEventArgs(pVirtualMachineGroup.Id, pVirtualMachineGroup.Name, pVirtualMachineResource.Id);
					pVirtualMachineGroup.SendEventToProxy(new ClusterWrapperEventArgs(EventType.Gained, eventArgs));
				}
				pVirtualMachineResource.Cluster.RealtimeCollections.Change(pVirtualMachineResource, "LoadSelection");
				ClusterPropertiesEventArgs forwardedPayload = new ClusterPropertiesEventArgs(pVirtualMachineResource.Id, pVirtualMachineResource.Name, null, null)
				{
					Properties = pVirtualMachineResource.Properties
				};
				Server.EnqueueNotification(new GroupNotification(new ClusterForwardedEventArgs(pVirtualMachineGroup.Id, pVirtualMachineResource.ResourceType, forwardedPayload, 3)));
			}
			ClusterLog.LogVerbose(LogSubcategory.FxCache, "Filling Virtual Machine resources in each Virtual Machine Group completed");
		}
		catch (ClusterException ex)
		{
			ClusterLog.LogVerbose(LogSubcategory.FxCache, "Error filling Virtual Machine resources in groups: " + ex.Message);
			ClusterLog.LogException(ex, "There was an error populating the virtual machines resource in groups");
		}
	}

	private void PopulateResourceStates<TResource>(IEnumerable<PResource> resourcesInCache, ResourceKind kind, params ResourceLoadSelection[] loadSelections) where TResource : PResource
	{
		if (resourcesInCache == null)
		{
			ClusterLog.LogVerbose(LogSubcategory.FxCache, "Resources collection is empty, storage resource status were not loaded");
			return;
		}
		IEnumerable<PResource> resourcesToLoad = resourcesInCache.Where((PResource resource) => resource.ResourceType.ResourceKind == kind);
		Worker.Start(delegate
		{
			resourcesToLoad.Cast<TResource>().ForEach(delegate(TResource resource)
			{
				resource.LockObject.Writer();
				try
				{
					ResourceLoadSelection resourceLoadSelection = ResourceLoadSelection.None;
					ResourceLoadSelection[] array = loadSelections;
					foreach (ResourceLoadSelection resourceLoadSelection2 in array)
					{
						if (((uint)resource.LoadedSelection & (uint)resourceLoadSelection2) != (uint)resourceLoadSelection2)
						{
							resourceLoadSelection |= resourceLoadSelection2;
						}
					}
					if (resourceLoadSelection != 0)
					{
						try
						{
							resource.LoadObject((int)resourceLoadSelection);
						}
						catch (ClusterException exception)
						{
							ClusterLog.LogException(exception);
						}
						catch (Win32Exception exception2)
						{
							ClusterLog.LogException(exception2);
						}
					}
				}
				finally
				{
					resource.LockObject.UnlockWriter();
				}
				resource.Cluster.RealtimeCollections.Change(resource, "LoadSelection");
			});
		}, resourcesToLoad, null);
	}

	private void PopulateCache()
	{
		try
		{
			long num = 0L;
			ElapsedTime elapsedTime = new ElapsedTime();
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			bool isVirtualMachineToolsInstalled = VirtualMachineResource.IsVirtualMachineToolsInstalled;
			ClusterLog.LogVerbose(LogSubcategory.FxCache, "Virtual Machine Tools are {0} installed and value is cached".FormatCurrentCulture(isVirtualMachineToolsInstalled ? string.Empty : "not"));
			Server.PauseNotifications();
			IList<PNode> parameter;
			IList<PGroup> list;
			IList<PResource> parameter2;
			try
			{
				CacheManager.StartRefresh();
				elapsedTime.Start();
				PopulateCacheFromObjectType(Server.ResourceType.GetAll(new string[3] { "name", "id", "characteristics" }, nullElementOnError: true), delegate
				{
					CacheManager.ResourceTypeCacheLoaded = true;
				});
				num += elapsedTime.StopAndOutput("Populated Resource Type Cache for '{0}' in:".FormatCurrentCulture(base.Name));
				if (!base.IsOpen)
				{
					return;
				}
				elapsedTime.Start();
				parameter = PopulateCacheFromObjectType(Server.Node.GetAll(nullElementOnError: true), delegate
				{
					CacheManager.NodeCacheLoaded = true;
				});
				num += elapsedTime.StopAndOutput("Populated Node Cache for '{0}' in:".FormatCurrentCulture(base.Name));
				if (!base.IsOpen)
				{
					return;
				}
				elapsedTime.Start();
				list = PopulateCacheFromObjectType(Server.Group.GetAll(nullElementOnError: true), delegate
				{
					CacheManager.GroupCacheLoaded = true;
				});
				num += elapsedTime.StopAndOutput("Populated Group Cache for '{0}' in:".FormatCurrentCulture(base.Name));
				if (!base.IsOpen || list == null)
				{
					return;
				}
				elapsedTime.Start();
				parameter2 = PopulateCacheFromObjectType(Server.Resource.GetAll(nullElementOnError: true), delegate
				{
					CacheManager.ResourceCacheLoaded = true;
				});
				num += elapsedTime.StopAndOutput("Populated Resource Cache for '{0}' in:".FormatCurrentCulture(base.Name));
				if (!base.IsOpen)
				{
					return;
				}
				elapsedTime.Start();
				PopulateCacheFromObjectType(Server.Network.GetAll(nullElementOnError: true), delegate
				{
					CacheManager.NetworkCacheLoaded = true;
				});
				num += elapsedTime.StopAndOutput("Populated Network Cache for '{0}' in:".FormatCurrentCulture(base.Name));
				if (!base.IsOpen)
				{
					return;
				}
				CacheManager.StopRefresh();
			}
			finally
			{
				Server.ResumeNotifications();
			}
			elapsedTime.Start();
			Task.WaitAll(CreateTaskAndMeasureTime(NotifyToProxies, parameter, "Broadcast Node Cache to proxies for '{0}' in:".FormatCurrentCulture(base.Name)), CreateTaskAndMeasureTime(NotifyToProxies, list, "Broadcast Groups Cache to proxies for '{0}' in:".FormatCurrentCulture(base.Name)), CreateTaskAndMeasureTime(NotifyToProxies, parameter2, "Broadcast Resources Cache to proxies for '{0}' in:".FormatCurrentCulture(base.Name)), CreateTaskAndMeasureTime(PopulateVirtualMachineResourcesInGroups, parameter2, "Broadcast All Virtual Machines in each group for '{0}' in:".FormatCurrentCulture(base.Name)), CreateTaskAndMeasureTime(delegate(IList<PResource> resources)
			{
				PopulateResourceStates<PStorageResource>(resources, ResourceKind.ClusterFileSystem, new ResourceLoadSelection[2]
				{
					ResourceLoadSelection.Basic,
					ResourceLoadSelection.PrivateProperties
				});
			}, parameter2, "Populate CSV Resource Basic Payload and Private Properties for '{0}' in:".FormatCurrentCulture(base.Name)), CreateTaskAndMeasureTime(delegate(IList<PResource> resources)
			{
				PopulateResourceStates<PStorageResource>(resources, ResourceKind.PhysicalDisk, new ResourceLoadSelection[2]
				{
					ResourceLoadSelection.Basic,
					ResourceLoadSelection.PrivateProperties
				});
			}, parameter2, "Populate Physical Disk Resource Basic Payload and Private Properties for '{0}' in:".FormatCurrentCulture(base.Name)), CreateTaskAndMeasureTime(delegate(IList<PResource> resources)
			{
				PopulateResourceStates<PStoragePoolResource>(resources, ResourceKind.StoragePool, Array.Empty<ResourceLoadSelection>());
			}, parameter2, "Populate Storage Pool states for '{0}' in:".FormatCurrentCulture(base.Name)));
			num += elapsedTime.StopAndOutput(null);
			Utilities.UnreferencedParameter(num);
		}
		finally
		{
			Thread.CurrentThread.Priority = ThreadPriority.Normal;
		}
	}

	internal void CoreGroupOwnerNodeChanged(PNode node)
	{
		HostNode = node?.Name;
		ClusterHostNodeChangedEventArgs eventArgs = new ClusterHostNodeChangedEventArgs(base.Id, HostNode);
		RouteEvent(new ClusterWrapperEventArgs(EventType.ClusterHostNodeChanged, eventArgs));
	}

	private Task CreateTaskAndMeasureTime<T>(Action<T> action, T parameter, string outputMessage)
	{
		Task task = new Task(delegate
		{
			ElapsedTime elapsedTime = ElapsedTime.CreateAndStart();
			action.SafeCall(parameter);
			elapsedTime.StopAndOutput(outputMessage);
		}, parameter);
		task.Start();
		return task;
	}

	private bool CanQueryFromCache(QueryInfo queryInfo)
	{
		if ((typeof(FailoverClusters.Framework.Group).IsAssignableFrom(queryInfo.Source) && CacheManager.GroupCacheLoaded) || (typeof(Node).IsAssignableFrom(queryInfo.Source) && CacheManager.NodeCacheLoaded) || (typeof(Network).IsAssignableFrom(queryInfo.Source) && CacheManager.NetworkCacheLoaded) || (typeof(NetworkInterface).IsAssignableFrom(queryInfo.Source) && CacheManager.NetworkInterfaceCacheLoaded) || (typeof(ResourceType).IsAssignableFrom(queryInfo.Source) && CacheManager.ResourceTypeCacheLoaded) || typeof(Cluster).IsAssignableFrom(queryInfo.Source))
		{
			return true;
		}
		if (typeof(Resource).IsAssignableFrom(queryInfo.Source) && CacheManager.ResourceCacheLoaded)
		{
			return OptimizedQueries.Any((string optimizedQuery) => new Regex(optimizedQuery, RegexOptions.IgnoreCase).IsMatch(queryInfo.QueryText));
		}
		return false;
	}

	public PResourceType GetResourceType(string resourceTypeName)
	{
		using (ClusterLock clusterLock = CacheManager.Get(PResourceType.IdFromName(resourceTypeName), ClusterIdentityType.ResourceType, LockAccess.Reader))
		{
			if (clusterLock != null)
			{
				return (PResourceType)clusterLock.Owner;
			}
		}
		PResourceType pResourceType;
		if (PResourceType.StringToResourceKind(resourceTypeName) == ResourceKind.ClusterFileSystem)
		{
			PResourceType actualResourceType = Server.ResourceType.Open(PResourceType.ResourceKindToString(ResourceKind.PhysicalDisk));
			pResourceType = new PResourceType(this, ResourceKind.ClusterFileSystem, actualResourceType);
		}
		else
		{
			pResourceType = Server.ResourceType.Open(resourceTypeName);
		}
		CacheManager.AddObject(pResourceType);
		return pResourceType;
	}

	public IEnumerable<PClusterObject> Select(QueryInfo queryInfo)
	{
		bool queryFromAdapter = true;
		IEnumerable<PClusterObject> enumerable;
		if (CanQueryFromCache(queryInfo))
		{
			enumerable = CacheManager.Select(queryInfo);
			queryFromAdapter = false;
		}
		else
		{
			enumerable = Server.Select<PClusterObject>(queryInfo);
		}
		foreach (PClusterObject item in enumerable)
		{
			if (item == null)
			{
				break;
			}
			yield return queryFromAdapter ? CacheManager.AddObject(item) : item;
		}
	}

	public override void TransferFrom(PClusterObject source, bool cacheIsLocked, int loadSelection)
	{
		if (!(source is PCluster))
		{
			throw new InvalidOperationException("Source and Target must be the same type: ".FormatCurrentCulture(GetType()));
		}
	}

	public override List<Action> ProcessNotification(Notification notification)
	{
		List<Action> list = base.ProcessNotification(notification);
		if (notification.Payload is ClusterDisconnectedEventArgs)
		{
			ClusterLog.LogVerbose(LogSubcategory.FxCoreNotification, "notification ClusterDisconnectedEventArgs received");
			ClusterDisconnectedEventArgs args = (ClusterDisconnectedEventArgs)notification.Payload;
			list.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.Disconnected, args));
			});
		}
		else if (notification.Payload is ClusterRenamedEventArgs)
		{
			ClusterRenamedEventArgs args2 = (ClusterRenamedEventArgs)notification.Payload;
			base.Name = args2.NewName;
			list.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.Renamed, args2));
			});
		}
		else if (notification.Payload is ClusterFullyQualifiedDomainChangedEventArgs)
		{
			ClusterFullyQualifiedDomainChangedEventArgs args3 = (ClusterFullyQualifiedDomainChangedEventArgs)notification.Payload;
			FqdnName = args3.NewFullyQualifiedDomainName;
			list.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.FullyQualifiedDomainNameChanged, args3));
			});
		}
		else if (notification.Payload is ClusterQuorumChangedEventArgs)
		{
			ClusterQuorumChangedEventArgs args4 = (ClusterQuorumChangedEventArgs)notification.Payload;
			QuorumConfiguration = args4.QuorumConfiguration;
			list.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.QuorumConfigurationChanged, args4));
			});
		}
		else if (notification.Payload is ClusterDestroyClusterProgressEventArgs)
		{
			ClusterDestroyClusterProgressEventArgs args5 = (ClusterDestroyClusterProgressEventArgs)notification.Payload;
			list.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.DestroyClusterUpdate, args5));
			});
		}
		return list;
	}

	public void HandleFatalError(ClusterException ex)
	{
		RouteEvent(new ClusterWrapperEventArgs(EventType.FatalError, new ClusterEventArgs(base.Id, ex)));
	}

	internal bool WillVoterLossCauseQuorumLoss(QuorumVoterActionCheck voterActionCheck, string id)
	{
		bool willLoseQuorum = false;
		ProtectedScope(delegate
		{
			willLoseQuorum = Server.Cluster.WillVoterLossCauseQuorumLoss(voterActionCheck, id);
		}, (ClusterException ex) => ex);
		return willLoseQuorum;
	}

	private static string CreateRegExQuery(string query)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in query)
		{
			switch (c)
			{
			case '$':
			case '(':
			case ')':
			case '*':
			case '.':
			case '?':
			case '[':
			case '\\':
			case ']':
			case '^':
			case '|':
				stringBuilder.Append('\\');
				break;
			}
			stringBuilder.Append(c);
		}
		stringBuilder.Replace("ANY", ".+");
		return stringBuilder.ToString();
	}
}

