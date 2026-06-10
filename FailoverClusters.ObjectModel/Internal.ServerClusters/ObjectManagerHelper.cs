using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace MS.Internal.ServerClusters;

internal class ObjectManagerHelper<T> : IDisposable where T : ClusterItem
{
	private static int registered;

	private Dictionary<Guid, GenericWeakReference<T>> m_objects;

	private Dictionary<ulong, Guid> m_notifyIds;

	private Dictionary<string, Guid> m_names;

	private object m_collectionLockObject;

	private string m_ObjectType;

	private static ulong m_nextId;

	private static object m_idLockObject;

	private EventHandler<ObjectLifetimeEventArgs<T>> _003Cbacking_store_003EObjectLifetimeChanged;

	[SpecialName]
	internal event EventHandler<ObjectLifetimeEventArgs<T>> ObjectLifetimeChanged
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003EObjectLifetimeChanged = (EventHandler<ObjectLifetimeEventArgs<T>>)Delegate.Combine(_003Cbacking_store_003EObjectLifetimeChanged, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003EObjectLifetimeChanged = (EventHandler<ObjectLifetimeEventArgs<T>>)Delegate.Remove(_003Cbacking_store_003EObjectLifetimeChanged, value);
		}
	}

	[SpecialName]
	private protected void raise_ObjectLifetimeChanged(object value0, ObjectLifetimeEventArgs<T> value1)
	{
		_003Cbacking_store_003EObjectLifetimeChanged?.Invoke(value0, value1);
	}

	static ObjectManagerHelper()
	{
		m_nextId = 1uL;
		m_idLockObject = new object();
	}

	private static ulong GetNextId()
	{
		Monitor.Enter(m_idLockObject);
		try
		{
			ulong nextId = m_nextId;
			m_nextId++;
			return nextId;
		}
		finally
		{
			Monitor.Exit(m_idLockObject);
		}
	}

	private void OnLifetimeChanged(T clusterObject, ulong notifyId, ObjectLifetime type)
	{
		ObjectLifetimeEventArgs<T> e = new ObjectLifetimeEventArgs<T>(clusterObject, notifyId, type);
		if (clusterObject != null)
		{
			DebugLog.LogVerbose("LifeTime Changed {0} (key: {1}) {2} ({3})", clusterObject.Name, notifyId, type, GetObjectType());
		}
		else
		{
			DebugLog.LogVerbose("LifeTime Changed key: {0} {1} ({2})", notifyId, type, GetObjectType());
		}
		_003Cbacking_store_003EObjectLifetimeChanged?.Invoke(this, e);
	}

	private void EnterLock()
	{
		Monitor.Enter(m_collectionLockObject);
	}

	private void LeaveLock()
	{
		Monitor.Exit(m_collectionLockObject);
	}

	private string GetObjectType()
	{
		if (m_ObjectType == null)
		{
			Type typeFromHandle = typeof(T);
			m_ObjectType = typeFromHandle.Name;
		}
		return m_ObjectType;
	}

	private void CloseObjects()
	{
		foreach (T @object in GetObjects(includeDeleted: false))
		{
			T current = @object;
			try
			{
				current.Close();
			}
			catch (ClusterObjectDeletedException)
			{
			}
		}
	}

	private Collection<T> GetObjects([MarshalAs(UnmanagedType.U1)] bool includeDeleted)
	{
		//Discarded unreachable code: IL_0073
		Monitor.Enter(m_collectionLockObject);
		try
		{
			Collection<T> collection = new Collection<T>();
			Dictionary<Guid, GenericWeakReference<T>>.Enumerator enumerator = m_objects.GetEnumerator();
			while (enumerator.MoveNext())
			{
				T target = ((KeyValuePair<Guid, GenericWeakReference<T>>)(object)enumerator.Current).Value.Target;
				if (target != null && (includeDeleted || !target.IsDeleted))
				{
					collection.Add(target);
				}
			}
			return collection;
		}
		finally
		{
			Monitor.Exit(m_collectionLockObject);
		}
	}

	internal Collection<T> GetObjects()
	{
		return GetObjects(includeDeleted: false);
	}

	internal ObjectManagerHelper()
	{
		m_collectionLockObject = new object();
		m_objects = new Dictionary<Guid, GenericWeakReference<T>>();
		m_notifyIds = new Dictionary<ulong, Guid>();
		m_names = new Dictionary<string, Guid>();
		m_ObjectType = null;
	}

	private void _007EObjectManagerHelper_00601()
	{
		CloseObjects();
	}

	internal void RefreshObjects()
	{
		foreach (T @object in GetObjects(includeDeleted: false))
		{
			T current = @object;
			try
			{
				current.Refresh();
			}
			catch (ClusterObjectDeletedException)
			{
			}
			catch (ThreadAbortException)
			{
			}
			catch (Exception caughtException)
			{
				Guid id = current.Id;
				ExceptionHelp.LogException(caughtException, "Error refreshing object: {0}", id);
			}
		}
	}

	internal void RegisterInstance(T instance)
	{
		if (ClusterItem.CachingDisabled)
		{
			return;
		}
		Monitor.Enter(m_collectionLockObject);
		ulong nextId;
		try
		{
			nextId = GetNextId();
			Guid value = default(Guid);
			if (m_names.TryGetValue(instance.Name, out value))
			{
				T instance2 = GetInstance(instance.Name);
				if (instance2 != null)
				{
					UnregisterInstance(instance2);
				}
				else
				{
					m_names.Remove(instance.Name);
					m_objects.Remove(value);
					ulong key = 0uL;
					Dictionary<ulong, Guid>.Enumerator enumerator = m_notifyIds.GetEnumerator();
					while (enumerator.MoveNext())
					{
						ValueType valueType = enumerator.Current;
						Guid value2 = ((KeyValuePair<ulong, Guid>)valueType).Value;
						Guid guid = value2;
						if (value2 == value)
						{
							key = ((KeyValuePair<ulong, Guid>)valueType).Key;
							break;
						}
					}
					m_notifyIds.Remove(key);
				}
			}
			Guid id = instance.Id;
			m_names.Add(instance.Name, id);
			Guid id2 = instance.Id;
			m_objects.Add(id2, new GenericWeakReference<T>(instance));
			Guid id3 = instance.Id;
			m_notifyIds.Add(nextId, id3);
			instance.NotifyId = nextId;
		}
		finally
		{
			Monitor.Exit(m_collectionLockObject);
		}
		OnLifetimeChanged(instance, nextId, ObjectLifetime.Start);
	}

	internal void UnregisterInstance(T instance)
	{
		if (!ClusterItem.CachingDisabled)
		{
			Monitor.Enter(m_collectionLockObject);
			try
			{
				Guid id = instance.Id;
				m_objects.Remove(id);
				m_notifyIds.Remove(instance.NotifyId);
				m_names.Remove(instance.Name);
			}
			finally
			{
				Monitor.Exit(m_collectionLockObject);
			}
			OnLifetimeChanged(instance, instance.NotifyId, ObjectLifetime.End);
		}
	}

	internal void SyncRename(ulong id, string name)
	{
		Monitor.Enter(m_collectionLockObject);
		try
		{
			Guid value = Guid.Empty;
			if (m_names.TryGetValue(name, out value))
			{
				return;
			}
			DebugLog.LogVerbose(string.Format(CultureInfo.CurrentCulture, "Name is not found in dictionary : {0}", name));
			if (!m_notifyIds.TryGetValue(id, out value))
			{
				return;
			}
			string text = string.Empty;
			Dictionary<string, Guid>.Enumerator enumerator = m_names.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ValueType valueType = enumerator.Current;
				Guid value2 = ((KeyValuePair<string, Guid>)valueType).Value;
				if (value == value2)
				{
					text = ((KeyValuePair<string, Guid>)valueType).Key;
					break;
				}
			}
			m_names.Remove(text);
			m_names.Add(name, value);
			DebugLog.LogVerbose(string.Format(CultureInfo.CurrentCulture, "Name dictionary repaired: {0} --> {1}", text, name));
		}
		finally
		{
			Monitor.Exit(m_collectionLockObject);
		}
	}

	internal T GetInstance(Guid id)
	{
		GenericWeakReference<T> genericWeakReference = null;
		Monitor.Enter(m_collectionLockObject);
		try
		{
			genericWeakReference = null;
			if (m_objects.TryGetValue(id, out genericWeakReference) && genericWeakReference.Target != null)
			{
				return genericWeakReference.Target;
			}
		}
		finally
		{
			Monitor.Exit(m_collectionLockObject);
		}
		return null;
	}

	internal T GetInstance(ulong id)
	{
		GenericWeakReference<T> genericWeakReference = null;
		Monitor.Enter(m_collectionLockObject);
		try
		{
			genericWeakReference = null;
			Guid value = default(Guid);
			if (m_notifyIds.TryGetValue(id, out value) && m_objects.TryGetValue(value, out genericWeakReference) && genericWeakReference.Target != null)
			{
				return genericWeakReference.Target;
			}
		}
		finally
		{
			Monitor.Exit(m_collectionLockObject);
		}
		return null;
	}

	internal T GetInstance(string name)
	{
		GenericWeakReference<T> genericWeakReference = null;
		Monitor.Enter(m_collectionLockObject);
		try
		{
			genericWeakReference = null;
			Guid value = default(Guid);
			if (m_names.TryGetValue(name, out value) && m_objects.TryGetValue(value, out genericWeakReference) && genericWeakReference.Target != null)
			{
				return genericWeakReference.Target;
			}
		}
		finally
		{
			Monitor.Exit(m_collectionLockObject);
		}
		return null;
	}

	protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
	{
		if (A_0)
		{
			_007EObjectManagerHelper_00601();
		}
		else
		{
			base.Finalize();
		}
	}

	public virtual sealed void Dispose()
	{
		Dispose(A_0: true);
		GC.SuppressFinalize(this);
	}
}
