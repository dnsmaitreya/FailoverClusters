using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public abstract class ObservableDictionary<T> : LockableKeyedCollection<Guid, T>, IDisposable, INotifyCollectionChanged, INotifyPropertyChanged where T : ClusterObject
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

	private readonly SimpleMonitor monitor;

	[NonSerialized]
	private readonly ReaderWriterLockSlimFramework lockObject;

	public new virtual T this[Guid key]
	{
		get
		{
			if (lockObject != null)
			{
				lockObject.EnterReadLock();
			}
			try
			{
				return base[key];
			}
			finally
			{
				if (lockObject != null)
				{
					lockObject.ExitReadLock();
				}
			}
		}
	}

	public event NotifyCollectionChangedEventHandler CollectionChanged;

	public event PropertyChangedEventHandler PropertyChanged;

	protected ObservableDictionary(bool realTime)
		: this(realTime, (Dispatcher)null)
	{
	}

	protected ObservableDictionary(bool realTime, Dispatcher dispatcher)
		: base(realTime, dispatcher)
	{
		monitor = new SimpleMonitor();
		lockObject = new ReaderWriterLockSlimFramework(LockRecursionPolicy.SupportsRecursion);
	}

	public override void TrimExcess()
	{
		if (lockObject != null)
		{
			lockObject.EnterWriteLock();
		}
		try
		{
			base.TrimExcess();
		}
		finally
		{
			if (lockObject != null)
			{
				lockObject.ExitWriteLock();
			}
		}
	}

	protected override Guid GetKeyForItem(T item)
	{
		if (lockObject != null)
		{
			lockObject.EnterReadLock();
		}
		try
		{
			return item.Id;
		}
		finally
		{
			if (lockObject != null)
			{
				lockObject.ExitReadLock();
			}
		}
	}

	public bool TryGetValue(Guid id, out T value)
	{
		if (lockObject != null)
		{
			lockObject.EnterReadLock();
		}
		try
		{
			return TryGetValueInternal(id, out value);
		}
		finally
		{
			if (lockObject != null)
			{
				lockObject.ExitReadLock();
			}
		}
	}

	internal void RemoveInternal(Guid guid)
	{
		Execute(delegate
		{
			if (TryGetValueInternal(guid, out var value))
			{
				RemoveInternal(value);
			}
		});
	}

	internal virtual IEnumerator<T> GetEnumerator(bool synchronized)
	{
		if (lockObject != null)
		{
			lockObject.EnterReadLock();
		}
		try
		{
			return base.GetEnumerator(synchronized ? lockObject : null);
		}
		finally
		{
			if (lockObject != null)
			{
				lockObject.ExitReadLock();
			}
		}
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
			throw new InvalidOperationException("Reentrancy is not allowed on ObservableDictionaries");
		}
	}

	protected override void ClearItems()
	{
		Execute(OnClearItems);
	}

	private void OnClearItems()
	{
		if (lockObject != null)
		{
			lockObject.EnterWriteLock();
		}
		try
		{
			CheckReentrancy();
			base.ClearItems();
			base.TrimExcess();
			OnPropertyChanged("Count");
			OnPropertyChanged("Item[]");
			OnCollectionReset();
		}
		finally
		{
			if (lockObject != null)
			{
				lockObject.ExitWriteLock();
			}
		}
	}

	private void CopyFrom(IEnumerable<T> collection)
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
		if (lockObject != null)
		{
			lockObject.EnterWriteLock();
		}
		try
		{
			if (!Contains(item.Id))
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
			if (lockObject != null)
			{
				lockObject.ExitWriteLock();
			}
		}
	}

	protected override int MoveItem(T item, IComparer<T> comparer, out int oldIndex)
	{
		if (lockObject != null)
		{
			lockObject.EnterWriteLock();
		}
		try
		{
			CheckReentrancy();
			int num = base.MoveItem(item, comparer, out oldIndex);
			OnCollectionChanged(NotifyCollectionChangedAction.Move, item, num, oldIndex);
			return num;
		}
		finally
		{
			if (lockObject != null)
			{
				lockObject.ExitWriteLock();
			}
		}
	}

	protected override int InsertItem(T item, IComparer<T> comparer)
	{
		if (lockObject != null)
		{
			lockObject.EnterWriteLock();
		}
		try
		{
			if (Contains(item.Id))
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
			if (lockObject != null)
			{
				lockObject.ExitWriteLock();
			}
		}
	}

	protected override void AddRange(List<T> items)
	{
		if (lockObject != null)
		{
			lockObject.EnterWriteLock();
		}
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
			if (lockObject != null)
			{
				lockObject.ExitWriteLock();
			}
		}
	}

	public void Move(int oldIndex, int newIndex)
	{
		MoveItem(oldIndex, newIndex);
	}

	protected virtual void MoveItem(int oldIndex, int newIndex)
	{
		if (lockObject != null)
		{
			lockObject.EnterWriteLock();
		}
		try
		{
			CheckReentrancy();
			T item = base[oldIndex];
			base.RemoveItem(oldIndex);
			base.InsertItem(newIndex, item);
			OnPropertyChanged("Item[]");
			OnCollectionChanged(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex);
		}
		finally
		{
			if (lockObject != null)
			{
				lockObject.ExitWriteLock();
			}
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
		if (lockObject != null)
		{
			lockObject.EnterWriteLock();
		}
		try
		{
			CheckReentrancy();
			T item = base[index];
			base.RemoveItem(index);
			OnPropertyChanged("Count");
			OnPropertyChanged("Item[]");
			OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
		}
		finally
		{
			if (lockObject != null)
			{
				lockObject.ExitWriteLock();
			}
		}
	}

	protected override void SetItem(int index, T item)
	{
		if (lockObject != null)
		{
			lockObject.EnterWriteLock();
		}
		try
		{
			CheckReentrancy();
			T oldItem = base[index];
			base.SetItem(index, item);
			OnPropertyChanged("Item[]");
			OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldItem, item, index);
		}
		finally
		{
			if (lockObject != null)
			{
				lockObject.ExitWriteLock();
			}
		}
	}

	~ObservableDictionary()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		this.CollectionChanged = null;
		this.PropertyChanged = null;
	}
}
