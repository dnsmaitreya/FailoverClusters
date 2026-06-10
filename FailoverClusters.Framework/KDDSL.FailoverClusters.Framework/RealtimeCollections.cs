using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace KDDSL.FailoverClusters.Framework;

internal class RealtimeCollections : IDisposable
{
	private readonly List<WeakReferenceEx> filters = new List<WeakReferenceEx>();

	private readonly ReaderWriterLockSlimFramework lockObject = new ReaderWriterLockSlimFramework(LockRecursionPolicy.NoRecursion);

	~RealtimeCollections()
	{
		lockObject.Dispose();
	}

	public void Add<T>(ClusterList<T> collection) where T : ClusterObject
	{
		lockObject.EnterWriteLock();
		try
		{
			filters.Add(new WeakReferenceEx(collection));
		}
		finally
		{
			lockObject.ExitWriteLock();
		}
	}

	public void Remove<T>(ClusterList<T> collection) where T : ClusterObject
	{
		lockObject.EnterWriteLock();
		try
		{
			for (int num = filters.Count - 1; num >= 0; num--)
			{
				WeakReferenceEx weakReferenceEx = filters[num];
				if (weakReferenceEx.Target == collection || weakReferenceEx.Target == null)
				{
					_ = weakReferenceEx.Target;
					filters.RemoveAt(num);
				}
			}
		}
		finally
		{
			lockObject.ExitWriteLock();
		}
	}

	internal void Add<TType>(IPClusterObject<TType> privateClusterObject, RTCOperation rtcOperation) where TType : ClusterObject
	{
		lockObject.EnterReadLock();
		try
		{
			foreach (WeakReferenceEx filter in filters)
			{
				object target = filter.Target;
				if (target == null || !(target.GetType().GetGenericArguments()[0] == typeof(TType)))
				{
					continue;
				}
				ClusterList<TType> clusterList = (ClusterList<TType>)target;
				Delegate whereLambdaExpressionFx = clusterList.QueryInfo.WhereLambdaExpressionFx;
				TType proxy = privateClusterObject.GetProxy();
				if ((object)whereLambdaExpressionFx != null && (privateClusterObject.LoadedSelection & 1) != 1)
				{
					proxy.LoadAsync(1);
				}
				else if ((object)whereLambdaExpressionFx == null || (bool)whereLambdaExpressionFx.DynamicInvoke(proxy))
				{
					if (rtcOperation == RTCOperation.Add)
					{
						clusterList.AddInternal(proxy);
					}
					else
					{
						clusterList.ReplaceInternal(proxy);
					}
				}
			}
		}
		finally
		{
			lockObject.ExitReadLock();
		}
	}

	public void RunOperation(RTCMaintenance maintenaceType)
	{
		lockObject.EnterReadLock();
		try
		{
			foreach (WeakReferenceEx filter in filters)
			{
				object target = filter.Target;
				if (target == null)
				{
					continue;
				}
				switch (maintenaceType)
				{
				case RTCMaintenance.Clean:
					((IList)target).Clear();
					break;
				case RTCMaintenance.Refresh:
					foreach (object item in target as IEnumerable)
					{
						_ = item;
					}
					break;
				}
			}
		}
		finally
		{
			lockObject.ExitReadLock();
		}
	}

	internal void Change<TType>(IPClusterObject<TType> privateClusterObject, string propertyName) where TType : ClusterObject
	{
		if (privateClusterObject.IdentityType == ClusterIdentityType.Group)
		{
			ChangePrivate((IPClusterObject<Group>)privateClusterObject, propertyName);
		}
		else if (privateClusterObject.IdentityType == ClusterIdentityType.Resource)
		{
			ChangePrivate((IPClusterObject<Resource>)privateClusterObject, propertyName);
		}
		else if (privateClusterObject.IdentityType == ClusterIdentityType.Node)
		{
			ChangePrivate((IPClusterObject<Node>)privateClusterObject, propertyName);
		}
		else if (privateClusterObject.IdentityType == ClusterIdentityType.NetworkInterface)
		{
			ChangePrivate((IPClusterObject<NetworkInterface>)privateClusterObject, propertyName);
		}
		else if (privateClusterObject.IdentityType == ClusterIdentityType.Network)
		{
			ChangePrivate((IPClusterObject<Network>)privateClusterObject, propertyName);
		}
		else if (privateClusterObject.IdentityType == ClusterIdentityType.ResourceType)
		{
			ChangePrivate((IPClusterObject<ResourceType>)privateClusterObject, propertyName);
		}
		else if (privateClusterObject.IdentityType == ClusterIdentityType.Cluster)
		{
			ChangePrivate((IPClusterObject<Cluster>)privateClusterObject, propertyName);
		}
	}

	private void ChangePrivate<TType>(IPClusterObject<TType> privateClusterObject, string propertyName) where TType : ClusterObject
	{
		lockObject.EnterReadLock();
		try
		{
			foreach (WeakReferenceEx filter in filters)
			{
				object target = filter.Target;
				if (target == null || Cluster.IdentityFromType(target.GetType().GetGenericArguments()[0]) != privateClusterObject.IdentityType)
				{
					continue;
				}
				ClusterList<TType> clusterList = (ClusterList<TType>)target;
				Delegate whereLambdaExpressionFx = clusterList.QueryInfo.WhereLambdaExpressionFx;
				TType value = null;
				bool flag = false;
				if (!clusterList.TryGetValue(privateClusterObject.Id, out value))
				{
					value = privateClusterObject.GetProxy();
					flag = true;
				}
				if ((object)whereLambdaExpressionFx != null && (privateClusterObject.LoadedSelection & 1) != 1)
				{
					value.LoadAsync(1);
					break;
				}
				bool flag2 = false;
				try
				{
					if ((object)whereLambdaExpressionFx != null)
					{
						flag2 = (bool)whereLambdaExpressionFx.DynamicInvoke(value);
					}
				}
				catch (Exception exception)
				{
					ClusterLog.LogException(LogLevel.Verbose, exception);
					break;
				}
				if ((object)whereLambdaExpressionFx != null && !flag2)
				{
					if (!flag)
					{
						clusterList.RemoveInternal(value);
					}
				}
				else if (flag)
				{
					clusterList.AddInternal(value);
				}
				else
				{
					clusterList.ChangedInternal(value, propertyName);
				}
			}
		}
		finally
		{
			lockObject.ExitReadLock();
		}
	}

	internal void Remove<TType>(PClusterObject privateClusterObject) where TType : ClusterObject
	{
		lockObject.EnterReadLock();
		try
		{
			foreach (WeakReferenceEx filter in filters)
			{
				object target = filter.Target;
				if (target != null && target.GetType().GetGenericArguments()[0] == typeof(TType))
				{
					((ClusterList<TType>)target).RemoveInternal(privateClusterObject.Id);
				}
			}
		}
		finally
		{
			lockObject.ExitReadLock();
		}
	}

	internal void Collect(PCluster cluster = null)
	{
		List<WeakReferenceEx> list = new List<WeakReferenceEx>();
		lockObject.EnterWriteLock();
		try
		{
			foreach (WeakReferenceEx filter in filters)
			{
				IClusterList clusterList = (IClusterList)filter.Target;
				if (clusterList == null)
				{
					list.Add(filter);
				}
				else if (cluster != null && clusterList.Cluster.Id == cluster.Id)
				{
					list.Add(filter);
				}
				else
				{
					clusterList.TrimExcess();
				}
			}
			foreach (WeakReferenceEx item in list)
			{
				filters.Remove(item);
			}
		}
		finally
		{
			lockObject.ExitWriteLock();
		}
	}

	public void Dispose()
	{
		WeakReferenceEx[] array = new WeakReferenceEx[0];
		lockObject.EnterWriteLock();
		try
		{
			array = filters.ToArray();
		}
		finally
		{
			lockObject.ExitWriteLock();
		}
		WeakReferenceEx[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			((IClusterList)array2[i].Target)?.Dispose();
		}
		lockObject.EnterWriteLock();
		try
		{
			filters.Clear();
		}
		finally
		{
			lockObject.ExitWriteLock();
		}
	}
}

