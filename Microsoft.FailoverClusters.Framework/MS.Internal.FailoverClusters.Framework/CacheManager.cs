using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UI.Common;

namespace MS.Internal.FailoverClusters.Framework;

internal class CacheManager
{
	private static readonly GuidDictionary<ClusterLock> ClusterList;

	private readonly GuidDictionary<ClusterLock> groupList = new GuidDictionary<ClusterLock>();

	private readonly GuidDictionary<ClusterLock> nodeList = new GuidDictionary<ClusterLock>();

	private readonly GuidDictionary<ClusterLock> networkList = new GuidDictionary<ClusterLock>();

	private readonly GuidDictionary<ClusterLock> networkInterfaceList = new GuidDictionary<ClusterLock>();

	private readonly GuidDictionary<ClusterLock> resourceList = new GuidDictionary<ClusterLock>();

	private readonly GuidDictionary<ClusterLock> resourceTypeList = new GuidDictionary<ClusterLock>();

	private readonly ConcurrentDictionary<Guid, NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK> replicatedResources = new ConcurrentDictionary<Guid, NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK>();

	private readonly ConcurrentDictionary<Guid, ReplicationGroupInfo> replicatedGroups = new ConcurrentDictionary<Guid, ReplicationGroupInfo>();

	private readonly ConcurrentDictionary<ClusterIdentityType, GuidDictionary<ClusterLock>> lockDictionaries = new ConcurrentDictionary<ClusterIdentityType, GuidDictionary<ClusterLock>>();

	private readonly ConcurrentDictionary<ClusterIdentityType, Func<PCluster, Guid, PClusterObject>> dictionaryOpenById = new ConcurrentDictionary<ClusterIdentityType, Func<PCluster, Guid, PClusterObject>>();

	private readonly ConcurrentDictionary<ClusterIdentityType, Func<PCluster, string, PClusterObject>> dictionaryOpenByName = new ConcurrentDictionary<ClusterIdentityType, Func<PCluster, string, PClusterObject>>();

	private static readonly ConcurrentDictionary<Type, ClusterIdentityType> DictionaryIdentities;

	private readonly ReaderWriterLockSlimFramework cacheLock = new ReaderWriterLockSlimFramework(LockRecursionPolicy.SupportsRecursion)
	{
		Name = "Cache Lock"
	};

	public ReaderWriterLockSlimFramework CacheLock => cacheLock;

	public ConcurrentDictionary<Guid, NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK> ReplicatedResources => replicatedResources;

	public ConcurrentDictionary<Guid, ReplicationGroupInfo> ReplicatedGroups => replicatedGroups;

	public bool GroupCacheLoaded { get; set; }

	public bool ResourceCacheLoaded { get; set; }

	public bool NodeCacheLoaded { get; set; }

	public bool NetworkCacheLoaded { get; set; }

	public bool NetworkInterfaceCacheLoaded { get; set; }

	public bool ResourceTypeCacheLoaded { get; set; }

	static CacheManager()
	{
		ClusterList = new GuidDictionary<ClusterLock>();
		DictionaryIdentities = new ConcurrentDictionary<Type, ClusterIdentityType>();
		DictionaryIdentities.TryAdd(typeof(Group), ClusterIdentityType.Group);
		DictionaryIdentities.TryAdd(typeof(Node), ClusterIdentityType.Node);
		DictionaryIdentities.TryAdd(typeof(Network), ClusterIdentityType.Network);
		DictionaryIdentities.TryAdd(typeof(NetworkInterface), ClusterIdentityType.NetworkInterface);
		DictionaryIdentities.TryAdd(typeof(Resource), ClusterIdentityType.Resource);
		DictionaryIdentities.TryAdd(typeof(ResourceType), ClusterIdentityType.ResourceType);
	}

	public CacheManager()
	{
		lockDictionaries.TryAdd(ClusterIdentityType.Group, groupList);
		lockDictionaries.TryAdd(ClusterIdentityType.Node, nodeList);
		lockDictionaries.TryAdd(ClusterIdentityType.Network, networkList);
		lockDictionaries.TryAdd(ClusterIdentityType.NetworkInterface, networkInterfaceList);
		lockDictionaries.TryAdd(ClusterIdentityType.Resource, resourceList);
		lockDictionaries.TryAdd(ClusterIdentityType.ResourceType, resourceTypeList);
		dictionaryOpenById.TryAdd(ClusterIdentityType.Group, (PCluster pcluster, Guid id) => pcluster.Server.Group.Open(id));
		dictionaryOpenById.TryAdd(ClusterIdentityType.Node, (PCluster pcluster, Guid id) => pcluster.Server.Node.Open(id));
		dictionaryOpenById.TryAdd(ClusterIdentityType.Network, (PCluster pcluster, Guid id) => pcluster.Server.Network.Open(id));
		dictionaryOpenById.TryAdd(ClusterIdentityType.NetworkInterface, (PCluster pcluster, Guid id) => pcluster.Server.NetworkInterface.Open(id));
		dictionaryOpenById.TryAdd(ClusterIdentityType.Resource, (PCluster pcluster, Guid id) => pcluster.Server.Resource.Open(id));
		dictionaryOpenByName.TryAdd(ClusterIdentityType.Group, (PCluster pcluster, string name) => pcluster.Server.Group.Open(name));
		dictionaryOpenByName.TryAdd(ClusterIdentityType.Node, (PCluster pcluster, string name) => pcluster.Server.Node.Open(name));
		dictionaryOpenByName.TryAdd(ClusterIdentityType.Network, (PCluster pcluster, string name) => pcluster.Server.Network.Open(name));
		dictionaryOpenByName.TryAdd(ClusterIdentityType.NetworkInterface, (PCluster pcluster, string name) => pcluster.Server.NetworkInterface.Open(name));
		dictionaryOpenByName.TryAdd(ClusterIdentityType.Resource, (PCluster pcluster, string name) => pcluster.Server.Resource.Open(name));
		dictionaryOpenByName.TryAdd(ClusterIdentityType.ResourceType, (PCluster pcluster, string name) => pcluster.Server.ResourceType.Open(name));
	}

	~CacheManager()
	{
		cacheLock.Dispose();
	}

	public void Obliterate(Action clearing)
	{
		List<PNetNameResource> netNames = new List<PNetNameResource>();
		cacheLock.EnterWriteLock();
		try
		{
			GroupCacheLoaded = false;
			ResourceCacheLoaded = false;
			NodeCacheLoaded = false;
			NetworkCacheLoaded = false;
			NetworkInterfaceCacheLoaded = false;
			ResourceTypeCacheLoaded = false;
			resourceList.ForEach(delegate(KeyValuePair<Guid, ClusterLock> element)
			{
				if (element.Value.Owner is PNetNameResource item)
				{
					netNames.Add(item);
				}
			});
			lockDictionaries.Values.ForEach(delegate(GuidDictionary<ClusterLock> dictionary)
			{
				dictionary.Clear();
			});
			clearing.SafeCall();
		}
		finally
		{
			cacheLock.ExitWriteLock();
		}
		netNames.ForEach(delegate(PNetNameResource netName)
		{
			netName.CancelObservables();
		});
	}

	public void Invalidate()
	{
		List<GuidDictionary<ClusterLock>> cacheDictionaryRefresh = new List<GuidDictionary<ClusterLock>>();
		cacheLock.EnterWriteLock();
		try
		{
			GroupCacheLoaded = false;
			ResourceCacheLoaded = false;
			NodeCacheLoaded = false;
			NetworkCacheLoaded = false;
			NetworkInterfaceCacheLoaded = false;
			ResourceTypeCacheLoaded = false;
			lockDictionaries.Values.ForEach(delegate(GuidDictionary<ClusterLock> cacheDictionary)
			{
				cacheDictionaryRefresh.Add(new GuidDictionary<ClusterLock>(cacheDictionary));
			});
		}
		finally
		{
			cacheLock.ExitWriteLock();
		}
		cacheDictionaryRefresh.ForEach(delegate(GuidDictionary<ClusterLock> cacheDictionary)
		{
			cacheDictionary.ForEach(delegate(KeyValuePair<Guid, ClusterLock> item)
			{
				ClusterLock value = item.Value;
				value.Writer();
				try
				{
					value.Owner.Refresh(targeted: true);
				}
				finally
				{
					value.UnlockWriter();
				}
			});
		});
	}

	public Guid GetReplicationGroupIdFromName(string replicationGroupName)
	{
		if (replicationGroupName == null)
		{
			return Guid.Empty;
		}
		return (from _ in replicatedResources
			where _.Value.ReplicationGroupName == replicationGroupName
			select _.Value.ReplicationGroupId).FirstOrDefault();
	}

	public Guid GetReplicationGroupIdFromResourceId(Guid resourceId)
	{
		if (replicatedResources.TryGetValue(resourceId, out var value))
		{
			return value.ReplicationGroupId;
		}
		return Guid.Empty;
	}

	public void StartRefresh()
	{
		lockDictionaries.Values.ForEach(delegate(GuidDictionary<ClusterLock> dictionary)
		{
			dictionary.StartRefresh();
		});
	}

	public void StopRefresh()
	{
		lockDictionaries.Values.ForEach(delegate(GuidDictionary<ClusterLock> dictionary)
		{
			dictionary.StopRefresh();
		});
	}

	public IEnumerable<PClusterObject> Select(QueryInfo queryInfo)
	{
		return from privateGroup in ReturnObjectsFromQuery(queryInfo)
			select (privateGroup);
	}

	private List<Guid> FindGuidFields(IList<IClusterQueryArgument> arguments, string fieldName)
	{
		List<Guid> list = new List<Guid>();
		for (int i = 0; i < arguments.Count; i++)
		{
			if (arguments[i] is FieldArgument && arguments[i].Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase) && i + 2 < arguments.Count && arguments[i + 1] is OperatorArgument && arguments[i + 2] is GuidArgument)
			{
				list.Add(new Guid(((GuidArgument)arguments[i + 2]).Value.ToString()));
			}
		}
		return list;
	}

	private IEnumerable<PClusterObject> ReturnObjectsFromQuery(QueryInfo queryInfo)
	{
		if (!DictionaryIdentities.TryGetValue(queryInfo.Source, out var value))
		{
			throw new NotSupportedException("Return object not supported for query type:" + queryInfo.Source.Name);
		}
		if (!lockDictionaries.TryGetValue(value, out var value2))
		{
			throw new NotSupportedException("Return object not supported for query type:" + value.Translate());
		}
		Delegate filterFx = queryInfo.WhereLambdaExpressionFx;
		cacheLock.EnterReadLock();
		List<PClusterObject> collection;
		try
		{
			collection = new List<PClusterObject>();
			IEnumerable<ClusterLock> enumerable = value2.Values;
			if (typeof(Resource).IsAssignableFrom(queryInfo.Source))
			{
				List<Guid> ownerGroups = FindGuidFields(queryInfo.WhereSyntaxis, "ownergroup");
				if (ownerGroups.Count > 0)
				{
					enumerable = enumerable.Where(delegate(ClusterLock clusterLock)
					{
						PResource pResource = (PResource)clusterLock.Owner;
						return pResource.OwnerGroup == null || pResource.ResourceType.ResourceKind == ResourceKind.ClusterFileSystem || ownerGroups.Contains(pResource.OwnerGroup.Id);
					});
				}
			}
			enumerable.ForEach(delegate(ClusterLock lockObject)
			{
				PClusterObject owner = lockObject.Owner;
				if ((object)filterFx != null)
				{
					object proxyObject = owner.GetProxyObject();
					if ((bool)filterFx.DynamicInvoke(proxyObject))
					{
						collection.Add(lockObject.Owner);
					}
				}
				else
				{
					collection.Add(lockObject.Owner);
				}
			});
		}
		finally
		{
			cacheLock.ExitReadLock();
		}
		if (queryInfo.OrderBy.Count > 0)
		{
			ClusterListComparer<PClusterObject> comparer = queryInfo.OrderBy.Aggregate(null, (ClusterListComparer<PClusterObject> current, OrderByItem item) => new ClusterListComparer<PClusterObject>(item, current));
			collection.Sort(comparer);
		}
		else
		{
			collection.Sort((PClusterObject itemX, PClusterObject itemY) => NativeMethods.StrCmpLogicalW(itemX.Name, itemY.Name));
		}
		return collection;
	}

	public static IEnumerable<PCluster> GetClusters()
	{
		List<PCluster> list = new List<PCluster>();
		lock (ClusterList)
		{
			list.AddRange(from l in ClusterList.Values
				where l.Owner.IsOpen
				select (PCluster)l.Owner);
		}
		return new ReadOnlyCollection<PCluster>(list);
	}

	public static PCluster AddCluster(PCluster cluster)
	{
		lock (ClusterList)
		{
			if (ClusterList.TryGetValue(cluster.CacheId, out var value))
			{
				ClusterLog.LogWarning("Adding a new Cluster object to cache when already contains a cluster with identifier" + cluster.CacheId);
				return (PCluster)value.Owner;
			}
			ClusterList.Add(cluster.CacheId, cluster.LockObject);
			return cluster;
		}
	}

	public void ReplaceReplicatedResources(IEnumerable<NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK> resources)
	{
		cacheLock.EnterWriteLock();
		try
		{
			replicatedResources.Clear();
			foreach (NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK replicatedResource in resources)
			{
				replicatedResources.AddOrUpdate(replicatedResource.ClusterResourceId, replicatedResource, (Guid key, NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK value) => replicatedResource);
			}
		}
		finally
		{
			cacheLock.ExitWriteLock();
		}
	}

	public void ReplaceReplicatedGroups(IEnumerable<ReplicationGroupInfo> groups)
	{
		cacheLock.EnterWriteLock();
		try
		{
			replicatedGroups.Clear();
			foreach (ReplicationGroupInfo replicatedGroup in groups)
			{
				replicatedGroups.AddOrUpdate(replicatedGroup.ClusterGroupId, replicatedGroup, (Guid key, ReplicationGroupInfo value) => replicatedGroup);
			}
		}
		finally
		{
			cacheLock.ExitWriteLock();
		}
	}

	public PClusterObject AddObject(PClusterObject clusterObject, bool cacheIsLocked = false, bool replace = false)
	{
		if (clusterObject == null)
		{
			return null;
		}
		if (!clusterObject.Cluster.IsOpen)
		{
			return clusterObject;
		}
		if (clusterObject is PCluster cluster)
		{
			return AddCluster(cluster);
		}
		PClusterObject pClusterObject = clusterObject;
		if (!cacheIsLocked)
		{
			cacheLock.EnterUpgradeableReadLock();
		}
		try
		{
			if (replace)
			{
				RemoveObject(pClusterObject);
			}
			if (!lockDictionaries.TryGetValue(pClusterObject.IdentityType, out var value))
			{
				throw new NotSupportedException("Add Object not supported for type:" + pClusterObject.GetType());
			}
			if (value.TryGetValue(pClusterObject.Id, out var value2))
			{
				pClusterObject = value2.Owner;
			}
			else
			{
				if (!cacheIsLocked)
				{
					cacheLock.EnterWriteLock();
				}
				try
				{
					value.Add(pClusterObject.Id, pClusterObject.LockObject);
				}
				finally
				{
					if (!cacheIsLocked)
					{
						cacheLock.ExitWriteLock();
					}
				}
			}
		}
		finally
		{
			if (!cacheIsLocked)
			{
				cacheLock.ExitUpgradeableReadLock();
			}
		}
		pClusterObject.TransferFrom(clusterObject, cacheIsLocked, (pClusterObject.LoadedSelection | clusterObject.LoadedSelection) ^ pClusterObject.LoadedSelection);
		return pClusterObject;
	}

	public void RemoveObject(PClusterObject clusterObject)
	{
		if (clusterObject == null)
		{
			throw new ArgumentNullException("clusterObject");
		}
		if (!lockDictionaries.TryGetValue(clusterObject.IdentityType, out var value))
		{
			if (!(clusterObject is PCluster pCluster))
			{
				throw new NotSupportedException("Remove Object not supported for type:" + clusterObject.GetType());
			}
			lock (ClusterList)
			{
				ClusterList.Remove(pCluster.CacheId);
			}
		}
		if (value == null)
		{
			throw new NullReferenceException("There was no dictionary found for the given Identity Type");
		}
		value.Remove(clusterObject.Id);
		clusterObject.OnRemovedFromCache();
	}

	public void ReplaceObject(PClusterObject clusterObject, Guid newId)
	{
		if (!lockDictionaries.TryGetValue(clusterObject.IdentityType, out var value))
		{
			if (!(clusterObject is PCluster))
			{
				throw new NotSupportedException(ExceptionResources.TypeNotSupportedInCache.FormatCurrentCulture(clusterObject.GetType().ToString()));
			}
			value = ClusterList;
		}
		ClusterLock clusterLock = null;
		ClusterLock clusterLock2 = null;
		if (value == ClusterList)
		{
			Monitor.Enter(ClusterList);
		}
		try
		{
			cacheLock.EnterWriteLock();
			try
			{
				ClusterLock clusterLock3 = value[clusterObject.Id];
				value.Remove(clusterObject.Id);
				clusterObject.Id = newId;
				if (value.TryGetValue(clusterObject.Id, out var value2))
				{
					clusterLock = clusterLock3;
					clusterLock2 = value2;
					clusterLock3.LockObject = value2.LockObject;
				}
				else
				{
					value.Add(clusterObject.Id, clusterObject.LockObject);
				}
			}
			finally
			{
				cacheLock.ExitWriteLock();
			}
		}
		finally
		{
			if (value == ClusterList)
			{
				Monitor.Exit(ClusterList);
			}
		}
		if (clusterLock != null && clusterLock.LockObject.IsWriteLockHeld)
		{
			clusterLock2.Writer();
			clusterLock.UnlockWriter();
		}
	}

	public ClusterLock Get(ClusterObject clusterObject, LockAccess lockAccess)
	{
		if (!lockDictionaries.TryGetValue(clusterObject.IdentityType, out var objectCacheDictionary))
		{
			throw new NotSupportedException(ExceptionResources.TypeNotSupportedInCache.FormatCurrentCulture(clusterObject.GetType().ToString()));
		}
		return ExecuteUnderCacheLock(lockAccess, () => objectCacheDictionary[clusterObject.Id]);
	}

	public PClusterObject AddObject(PCluster cluster, ClusterIdentityType identityType, Guid id)
	{
		if (!dictionaryOpenById.TryGetValue(identityType, out var value))
		{
			throw new NotSupportedException(string.Concat("The type ", identityType, " is not supported"));
		}
		PClusterObject clusterObject = value(cluster, id);
		return AddObject(clusterObject);
	}

	public PClusterObject AddObject(PCluster cluster, ClusterIdentityType identityType, string name)
	{
		if (!dictionaryOpenByName.TryGetValue(identityType, out var value))
		{
			throw new NotSupportedException(string.Concat("The type ", identityType, " is not supported"));
		}
		PClusterObject clusterObject = value(cluster, name);
		return AddObject(clusterObject);
	}

	public ClusterLock Get(Guid id, LockAccess lockAccess)
	{
		return ExecuteUnderCacheLock(lockAccess, () => (from ld in lockDictionaries.Values
			where ld.ContainsKey(id)
			select ld[id]).FirstOrDefault());
	}

	public ClusterLock Get(Guid id, ClusterIdentityType identity, LockAccess lockAccess)
	{
		if (!lockDictionaries.TryGetValue(identity, out var value))
		{
			throw new NotSupportedException(string.Concat("The type ", identity, " is not supported"));
		}
		return Get(id, value, lockAccess);
	}

	public bool IsWriteLockHeld<T>(IPClusterObject<T> clusterObject) where T : ClusterObject
	{
		if (!lockDictionaries.TryGetValue(clusterObject.IdentityType, out var value))
		{
			throw new NotSupportedException(string.Concat("The type ", clusterObject.IdentityType, " is not supported"));
		}
		return IsWriteLockHeld(clusterObject.Id, value);
	}

	public ClusterLock Get(string name, ClusterIdentityType identity, LockAccess lockAccess)
	{
		if (!lockDictionaries.TryGetValue(identity, out var value))
		{
			throw new NotSupportedException(string.Concat("The type ", identity, " is not supported"));
		}
		return Get(name, value, lockAccess);
	}

	private ClusterLock Get(Guid id, GuidDictionary<ClusterLock> cacheList, LockAccess lockAccess)
	{
		return ExecuteUnderCacheLock(lockAccess, () => cacheList[id]);
	}

	private ClusterLock Get(string name, GuidDictionary<ClusterLock> cacheList, LockAccess lockAccess)
	{
		return ExecuteUnderCacheLock(lockAccess, () => cacheList.Values.FirstOrDefault((ClusterLock clusterLockValue) => clusterLockValue.Owner.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)));
	}

	private ClusterLock ExecuteUnderCacheLock(LockAccess lockAccess, Func<ClusterLock> selector)
	{
		cacheLock.EnterReadLock();
		ClusterLock clusterLock;
		try
		{
			clusterLock = selector();
		}
		finally
		{
			cacheLock.ExitReadLock();
		}
		Lock(clusterLock, lockAccess);
		return clusterLock;
	}

	public static void RemoveCluster(PCluster cluster)
	{
		lock (ClusterList)
		{
			ClusterList.Remove(cluster.CacheId);
		}
	}

	public static ClusterLock ClusterLockById(Guid clusterId, LockAccess lockAccess)
	{
		ClusterLock clusterLock = null;
		lock (ClusterList)
		{
			foreach (ClusterLock item in ClusterList.Values.Where((ClusterLock clusterValueLock) => clusterValueLock.Owner.Id == clusterId))
			{
				clusterLock = item;
			}
		}
		Lock(clusterLock, lockAccess);
		return clusterLock;
	}

	public static ClusterLock GetCluster(Guid uniqueCacheId, LockAccess lockAccess)
	{
		ClusterLock value;
		lock (ClusterList)
		{
			if (!ClusterList.TryGetValue(uniqueCacheId, out value))
			{
				return null;
			}
		}
		Lock(value, lockAccess);
		return value;
	}

	private bool IsWriteLockHeld(Guid id, GuidDictionary<ClusterLock> cacheList)
	{
		cacheList.TryGetValue(id, out var value);
		return value?.IsWriteLockHeld() ?? false;
	}

	private static void Lock(ClusterLock clusterLock, LockAccess lockAccess)
	{
		if (clusterLock != null)
		{
			switch (lockAccess)
			{
			case LockAccess.Reader:
				clusterLock.Reader();
				break;
			case LockAccess.UpgradableReader:
				clusterLock.UpgradeableReader();
				break;
			case LockAccess.Writer:
				clusterLock.Writer();
				break;
			}
		}
	}

	public void SendEventToProxyGroups(ClusterWrapperEventArgs args)
	{
		cacheLock.EnterWriteLock();
		GuidDictionary<ClusterLock> list;
		try
		{
			list = new GuidDictionary<ClusterLock>(groupList);
		}
		finally
		{
			cacheLock.ExitWriteLock();
		}
		list.ForEach(delegate(KeyValuePair<Guid, ClusterLock> item)
		{
			item.Value.LockObject.EnterReadLock();
			try
			{
				item.Value.Owner.SendEventToProxy(args);
			}
			finally
			{
				item.Value.LockObject.ExitReadLock();
			}
		});
	}

	public void Collect()
	{
		cacheLock.EnterWriteLock();
		List<ILockable> cacheObjectList;
		try
		{
			ClusterList.TrimExcess();
			lockDictionaries.Values.ForEach(delegate(GuidDictionary<ClusterLock> dictionary)
			{
				dictionary.TrimExcess();
			});
			cacheObjectList = new List<ILockable>();
			cacheObjectList.AddRange(ClusterList.Values);
			lockDictionaries.Values.ForEach(delegate(GuidDictionary<ClusterLock> dictionary)
			{
				cacheObjectList.AddRange(dictionary.Values);
			});
		}
		finally
		{
			cacheLock.ExitWriteLock();
		}
		foreach (ILockable item in cacheObjectList)
		{
			item.Reader();
			try
			{
				IWeakEventContainer owner = item.Owner;
				if (owner != null)
				{
					if (!owner.NeedCompactation.HasValue)
					{
						owner.NeedCompactation = true;
					}
					else if (owner.NeedCompactation == true)
					{
						owner.Compact();
						owner.NeedCompactation = false;
					}
					else if (owner.NeedCompactation == false)
					{
						owner.NeedCompactation = null;
					}
				}
			}
			finally
			{
				item.UnlockReader();
			}
		}
	}
}
