using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FailoverClusters.UI.Common;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public abstract class ObservableKeyCollection<T> : LockableKeyedCollection<string, T>, INotifyCollectionChanged, INotifyPropertyChanged, IObserverPlus<T>, IObservableKeyCollection where T : IKeyQueryable<T>
{
	[Serializable]
	private class SimpleMonitor : IDisposable
	{
		private int busyCount;

		public bool Busy => busyCount > 0;

		public void Dispose()
		{
			busyCount--;
		}

		public void Enter()
		{
			busyCount++;
		}
	}

	private enum ObservableCollectionType
	{
		Enumeration,
		Association
	}

	private class DummyIntContainer
	{
		public int Data { get; set; }
	}

	[NonSerialized]
	private readonly ReaderWriterLockSlimFramework lockObject = new ReaderWriterLockSlimFramework(LockRecursionPolicy.SupportsRecursion);

	private static readonly ConcurrentDictionary<Cluster, ConcurrentDictionary<int, WeakReferenceEx<ObservableKeyCollection<T>>>> CachedCollections = new ConcurrentDictionary<Cluster, ConcurrentDictionary<int, WeakReferenceEx<ObservableKeyCollection<T>>>>();

	private readonly ConcurrentDictionary<Cluster, ConcurrentDictionary<Action<ObservableKeyCollection<T>>, object>> enumerationCallbackDictionary = new ConcurrentDictionary<Cluster, ConcurrentDictionary<Action<ObservableKeyCollection<T>>, object>>();

	private const string CountString = "Count";

	private const string IndexerName = "Item[]";

	private bool? empty;

	private readonly DummyIntContainer isAlreadyEnumerated = new DummyIntContainer
	{
		Data = 0
	};

	private readonly SimpleMonitor monitor;

	private Cluster cluster;

	public ObservableCollectionFilter<T> Filter { get; private set; }

	public Cluster Cluster { get; private set; }

	public object Owner { get; private set; }

	public ObservableCollectionItem ItemType { get; protected set; }

	public bool? Empty
	{
		get
		{
			return empty;
		}
		set
		{
			if (empty != value)
			{
				empty = value;
				OnPropertyChanged("Empty");
			}
		}
	}

	public new virtual T this[string key]
	{
		get
		{
			lockObject.EnterReadLock();
			try
			{
				return base[key];
			}
			finally
			{
				lockObject.ExitReadLock();
			}
		}
	}

	public event NotifyCollectionChangedEventHandler CollectionChanged;

	public event PropertyChangedEventHandler PropertyChanged;

	protected ObservableKeyCollection(object owner, Cluster cluster, ObservableCollectionItem itemType)
		: this(owner, cluster, itemType, (ObservableCollectionFilter<T>)null)
	{
	}

	protected ObservableKeyCollection(object owner, Cluster cluster, ObservableCollectionItem itemType, ObservableCollectionFilter<T> filter)
		: base(realtime: true, Global.DefaultDispatcher)
	{
		Cluster = cluster;
		Owner = owner;
		ItemType = itemType;
		Filter = filter;
		monitor = new SimpleMonitor();
		cluster.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PCluster)lockObject.Owner)?.ClusterGc.RegisterCollect(Collect, cluster);
		}, OperationType.Async, LockAccess.Reader);
	}

	public void ForceEnumeration()
	{
		using IEnumerator<T> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			_ = enumerator.Current;
		}
	}

	private static void Collect(object parameter)
	{
		Cluster key = (Cluster)parameter;
		if (!CachedCollections.TryGetValue(key, out var value))
		{
			return;
		}
		foreach (KeyValuePair<int, WeakReferenceEx<ObservableKeyCollection<T>>> item in value)
		{
			WeakReferenceEx<ObservableKeyCollection<T>> value2 = item.Value;
			if (value2.Target == null)
			{
				value.TryRemove(item.Key, out value2);
			}
		}
		if (value.Count == 0)
		{
			CachedCollections.TryRemove(key, out value);
		}
	}

	protected static ObservableKeyCollection<T> GetAssociationInstance<T1>(object owner, Cluster cluster, Func<ObservableKeyCollection<T>> creationFunc, T1 association)
	{
		return GetCollectionInstanceInternal(owner, cluster, creationFunc, ObservableCollectionType.Association, association);
	}

	protected static ObservableKeyCollection<T> GetCollectionInstance(object owner, Cluster cluster, Func<ObservableKeyCollection<T>> creationFunc)
	{
		return GetCollectionInstanceInternal<Enclosure>(owner, cluster, creationFunc, ObservableCollectionType.Enumeration, null);
	}

	private static ObservableKeyCollection<T> GetCollectionInstanceInternal<T1>(object owner, Cluster cluster, Func<ObservableKeyCollection<T>> creationFunc, ObservableCollectionType collectionType, T1 parameter)
	{
		ConcurrentDictionary<int, WeakReferenceEx<ObservableKeyCollection<T>>> orAdd = CachedCollections.GetOrAdd(cluster, (Cluster key) => new ConcurrentDictionary<int, WeakReferenceEx<ObservableKeyCollection<T>>>());
		ObservableKeyCollection<T> observableKeyCollectionInstance = null;
		orAdd.AddOrUpdate(owner.GetHashCode() + typeof(T).GetHashCode(), (int key) => CreateCollection(cluster, creationFunc, collectionType, parameter, out observableKeyCollectionInstance), delegate(int key, WeakReferenceEx<ObservableKeyCollection<T>> value)
		{
			ObservableKeyCollection<T> target = value.Target;
			if (target != null)
			{
				observableKeyCollectionInstance = target;
				return value;
			}
			return CreateCollection(cluster, creationFunc, collectionType, parameter, out observableKeyCollectionInstance);
		});
		return observableKeyCollectionInstance;
	}

	private static WeakReferenceEx<ObservableKeyCollection<T>> CreateCollection<T1>(Cluster cluster, Func<ObservableKeyCollection<T>> creationFunc, ObservableCollectionType collectionType, T1 parameter, out ObservableKeyCollection<T> observableKeyCollectionInstance)
	{
		observableKeyCollectionInstance = creationFunc();
		observableKeyCollectionInstance.cluster = cluster;
		WeakReferenceEx<ObservableKeyCollection<T>> weakRef = new WeakReferenceEx<ObservableKeyCollection<T>>(observableKeyCollectionInstance);
		Task.Factory.StartNew(delegate
		{
			Thread.Sleep(1000);
			ObservableKeyCollection<T> target = weakRef.Target;
			if (target != null)
			{
				target.PopulateCollection(collectionType, parameter);
				target.Subscribe();
			}
		});
		return weakRef;
	}

	private IEnumerable<T> PopulateCollectionInternal(IConnectionAdapterStorage storageAdapter)
	{
		return storageAdapter.Enumerate(this, Filter);
	}

	private IEnumerable<T> PopulateAssociationInternal<T1>(IConnectionAdapterStorage storageAdapter, T1 association)
	{
		return storageAdapter.Association(this, association);
	}

	private void SubscribeInternal(IConnectionAdapterStorage storageAdapter)
	{
		storageAdapter.Subscribe(this);
	}

	private void PopulateCollection<T1>(ObservableCollectionType collectionType, T1 parameter)
	{
		cluster.ExecuteMethod(delegate(ILockable proxyObject)
		{
			PCluster pCluster = (PCluster)proxyObject.Owner;
			bool value = true;
			foreach (T item in collectionType switch
			{
				ObservableCollectionType.Enumeration => PopulateCollectionInternal(pCluster.Server.Storage), 
				ObservableCollectionType.Association => PopulateAssociationInternal(pCluster.Server.Storage, parameter), 
				_ => throw new ArgumentException("ObservableCollectionType.{0} not supported".FormatCurrentCulture(collectionType)), 
			})
			{
				AddInternal(item, null);
				value = false;
			}
			Empty = value;
			CallEnumerationCallbacks();
		}, LockAccess.Reader, setErrorOnObject: false);
	}

	private void Subscribe()
	{
		cluster.ExecuteMethod(delegate(ILockable proxyObject)
		{
			PCluster pCluster = (PCluster)proxyObject.Owner;
			SubscribeInternal(pCluster.Server.Storage);
		}, LockAccess.Reader, setErrorOnObject: false);
	}

	public void OnNext(SubscriptionOperation operation, T item)
	{
		switch (operation)
		{
		case SubscriptionOperation.Add:
			if (Filter == null || Filter.FilterFx == null || Filter.FilterFx(item))
			{
				AddInternal(item, null);
			}
			break;
		case SubscriptionOperation.Delete:
			RemoveInternal(item.Key);
			break;
		case SubscriptionOperation.Modify:
		{
			if (TryGetValue(item.Key, out var value))
			{
				value.CopyFrom(item);
			}
			break;
		}
		}
	}

	public void OnCompleted(SubscriptionOperation operation)
	{
	}

	public void OnError(SubscriptionOperation operation, Exception ex)
	{
	}

	public void OnEnumerationComplete(Action<ObservableKeyCollection<T>> callback)
	{
		bool flag = true;
		lock (isAlreadyEnumerated)
		{
			ConcurrentDictionary<Action<ObservableKeyCollection<T>>, object> orAdd = enumerationCallbackDictionary.GetOrAdd(cluster, (Cluster key) => new ConcurrentDictionary<Action<ObservableKeyCollection<T>>, object>());
			if (isAlreadyEnumerated.Data == 0)
			{
				orAdd.TryAdd(callback, null);
			}
			else if (!orAdd.ContainsKey(callback))
			{
				flag = false;
			}
		}
		if (!flag)
		{
			Execute(delegate
			{
				callback.SafeCall(this);
			});
		}
	}

	private void CallEnumerationCallbacks()
	{
		List<Action<ObservableKeyCollection<T>>> list;
		lock (isAlreadyEnumerated)
		{
			list = enumerationCallbackDictionary.GetOrAdd(cluster, (Cluster key) => new ConcurrentDictionary<Action<ObservableKeyCollection<T>>, object>()).Keys.ToList();
			isAlreadyEnumerated.Data = 1;
		}
		if (list.Count == 0)
		{
			return;
		}
		foreach (Action<ObservableKeyCollection<T>> item in list)
		{
			Action<ObservableKeyCollection<T>> myCallback = item;
			Execute(delegate
			{
				myCallback.SafeCall(this);
			});
		}
	}

	internal virtual IEnumerator<T> GetEnumerator(bool synchronized)
	{
		lockObject.EnterReadLock();
		try
		{
			return base.GetEnumerator(synchronized ? lockObject : null);
		}
		finally
		{
			lockObject.ExitReadLock();
		}
	}

	protected override string GetKeyForItem(T item)
	{
		lockObject.EnterReadLock();
		try
		{
			return item.Key;
		}
		finally
		{
			lockObject.ExitReadLock();
		}
	}

	private void UpdateEmpty()
	{
		if (Count == 0 && Empty != true)
		{
			Empty = true;
		}
		if (Count > 0 && Empty != false)
		{
			Empty = false;
		}
	}

	protected void AddToCollection(T item)
	{
		Add(item);
		UpdateEmpty();
	}

	protected void RemoveFromCollection(T item)
	{
		Remove(item.Key);
		UpdateEmpty();
	}

	public override void TrimExcess()
	{
		lockObject.EnterWriteLock();
		try
		{
			base.TrimExcess();
		}
		finally
		{
			lockObject.ExitWriteLock();
		}
	}

	public bool TryGetValue(string key, out T value)
	{
		lockObject.EnterReadLock();
		try
		{
			return TryGetValueInternal(key, out value);
		}
		finally
		{
			lockObject.ExitReadLock();
		}
	}

	internal void RemoveInternal(string key)
	{
		Execute(delegate
		{
			if (TryGetValueInternal(key, out var value))
			{
				RemoveInternal(value);
			}
		});
	}

	internal void Restart()
	{
	}

	protected IDisposable BlockReentrancy()
	{
		monitor.Enter();
		return monitor;
	}

	protected void CheckReentrancy()
	{
		if (monitor.Busy && this.CollectionChanged != null && this.CollectionChanged.GetInvocationList().Length > 1)
		{
			throw new InvalidOperationException("Reentrancy is not allowed on FileShareCollections");
		}
	}

	protected override void ClearItems()
	{
		Execute(OnClearItems);
	}

	private void OnClearItems()
	{
		lockObject.EnterWriteLock();
		try
		{
			CheckReentrancy();
			base.ClearItems();
			base.TrimExcess();
			OnPropertyChanged("Count");
			OnPropertyChanged("Item[]");
			Empty = null;
			OnCollectionReset();
		}
		finally
		{
			lockObject.ExitWriteLock();
		}
	}

	protected void CopyFrom(IEnumerable<T> collection)
	{
		IList<T> list = base.Items;
		if (collection == null || list == null)
		{
			return;
		}
		foreach (T item in collection)
		{
			list.Add(item);
		}
	}

	protected override void InsertItem(int index, T item)
	{
		lockObject.EnterWriteLock();
		try
		{
			if (!Contains(item.Key))
			{
				CheckReentrancy();
				base.InsertItem(index, item);
				OnPropertyChanged("Count");
				OnPropertyChanged("Item[]");
				OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
			}
		}
		finally
		{
			lockObject.ExitWriteLock();
		}
	}

	protected override int MoveItem(T item, IComparer<T> comparer, out int oldIndex)
	{
		lockObject.EnterWriteLock();
		try
		{
			CheckReentrancy();
			int num = base.MoveItem(item, comparer, out oldIndex);
			OnCollectionChanged(NotifyCollectionChangedAction.Move, item, num, oldIndex);
			return num;
		}
		finally
		{
			lockObject.ExitWriteLock();
		}
	}

	protected override int InsertItem(T item, IComparer<T> comparer)
	{
		lockObject.EnterWriteLock();
		try
		{
			if (Contains(item.Key))
			{
				return -1;
			}
			CheckReentrancy();
			int num = base.InsertItem(item, comparer);
			OnPropertyChanged("Count");
			OnPropertyChanged("Item[]");
			OnCollectionChanged(NotifyCollectionChangedAction.Add, item, num);
			return num;
		}
		finally
		{
			lockObject.ExitWriteLock();
		}
	}

	protected override void AddRange(List<T> items)
	{
		lockObject.EnterWriteLock();
		try
		{
			CheckReentrancy();
			base.AddRange(items);
			foreach (T item in items)
			{
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
			}
			OnPropertyChanged("Count");
			OnPropertyChanged("Item[]");
		}
		finally
		{
			lockObject.ExitWriteLock();
		}
	}

	public void Move(int oldIndex, int newIndex)
	{
		MoveItem(oldIndex, newIndex);
	}

	protected virtual void MoveItem(int oldIndex, int newIndex)
	{
		lockObject.EnterWriteLock();
		try
		{
			CheckReentrancy();
			T val = base[oldIndex];
			base.RemoveItem(oldIndex);
			base.InsertItem(newIndex, val);
			OnPropertyChanged("Item[]");
			OnCollectionChanged(NotifyCollectionChangedAction.Move, val, newIndex, oldIndex);
		}
		finally
		{
			lockObject.ExitWriteLock();
		}
	}

	protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
	{
		if (this.CollectionChanged != null)
		{
			using (BlockReentrancy())
			{
				this.CollectionChanged(this, e);
			}
		}
	}

	private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
	{
		OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
	}

	private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
	{
		OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
	}

	private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index)
	{
		OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
	}

	private void OnCollectionReset()
	{
		OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
	}

	protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, e);
		}
	}

	private void OnPropertyChanged(string propertyName)
	{
		OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
	}

	protected override void RemoveItem(int index)
	{
		UIHelper.ExecuteOnDispatcher(delegate(int localIndex)
		{
			lockObject.EnterWriteLock();
			try
			{
				CheckReentrancy();
				T val = base[localIndex];
				base.RemoveItem(localIndex);
				OnPropertyChanged("Count");
				OnPropertyChanged("Item[]");
				OnCollectionChanged(NotifyCollectionChangedAction.Remove, val, localIndex);
			}
			finally
			{
				lockObject.ExitWriteLock();
			}
		}, OperationType.Async, index);
	}

	protected override void SetItem(int index, T item)
	{
		lockObject.EnterWriteLock();
		try
		{
			CheckReentrancy();
			T val = base[index];
			base.SetItem(index, item);
			OnPropertyChanged("Item[]");
			OnCollectionChanged(NotifyCollectionChangedAction.Replace, val, item, index);
		}
		finally
		{
			lockObject.ExitWriteLock();
		}
	}
}

