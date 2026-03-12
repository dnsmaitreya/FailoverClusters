using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.FailoverClusters.UI.Common;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

public class FileShareCollection : LockableKeyedCollection<string, FileShare>, IEnumerable<FileShare>, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
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

	private const string CountString = "Count";

	private const string IndexerName = "Item[]";

	private readonly SimpleMonitor monitor;

	[NonSerialized]
	private readonly ReaderWriterLockSlimFramework lockObject = new ReaderWriterLockSlimFramework(LockRecursionPolicy.SupportsRecursion);

	private readonly Group group;

	private readonly Cluster cluster;

	private bool? exist;

	private bool? empty;

	private bool subscribed;

	private ClusterList<Resource> netNames;

	private readonly HashSet<ReadOnlyCollection<FileShare>> shareCollections = new HashSet<ReadOnlyCollection<FileShare>>();

	private ClusterList<Resource> fileServers;

	private readonly Dictionary<FileShareProtocol, Counter> protocols = new Dictionary<FileShareProtocol, Counter>();

	public Group OwnerGroup => group;

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

	public bool? Exist
	{
		get
		{
			return exist;
		}
		set
		{
			lockObject.EnterWriteLock();
			try
			{
				if (value == true && exist != true)
				{
					SubscribeToNetNames();
				}
				if (value == false && exist != false)
				{
					UnsubscribeToNetNames();
				}
				exist = value;
			}
			finally
			{
				lockObject.ExitWriteLock();
			}
			OnPropertyChanged("Exist");
		}
	}

	public IList<FileShareProtocol> Protocols => new List<FileShareProtocol>(protocols.Keys.ToArray());

	public IList<string> Servers
	{
		get
		{
			if (netNames == null)
			{
				return new List<string>();
			}
			return new List<string>(((IList)netNames.ToArray()).ConvertAll((Converter<object, string>)((object resource) => ((Resource)resource).Name)).ToArray());
		}
	}

	public new virtual FileShare this[string key]
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

	[field: NonSerialized]
	public event NotifyCollectionChangedEventHandler CollectionChanged;

	[field: NonSerialized]
	public event PropertyChangedEventHandler PropertyChanged;

	internal FileShareCollection(Group group)
		: base(realtime: false, Global.DefaultDispatcher)
	{
		if (group == null)
		{
			throw new ArgumentNullException("group");
		}
		this.group = group;
		cluster = group.Cluster;
		monitor = new SimpleMonitor();
	}

	~FileShareCollection()
	{
	}

	private void FileServersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		Exist = fileServers.Count > 0;
	}

	private void NetNamesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		switch (e.Action)
		{
		case NotifyCollectionChangedAction.Add:
		{
			foreach (Resource newItem in e.NewItems)
			{
				if (!(newItem is NetNameResource))
				{
					continue;
				}
				NetNameResource resourceWithShares = newItem as NetNameResource;
				resourceWithShares.Restart();
				Worker.Start(delegate
				{
					ReadOnlyObservableCollection<FileShare> shares2 = resourceWithShares.Shares;
					Global.DefaultDispatcher.EnqueueInvoke((Action)delegate
					{
						shareCollections.Add(shares2);
						AggregateShares(shares2);
						OnPropertyChanged("Servers");
					});
				});
			}
			break;
		}
		case NotifyCollectionChangedAction.Remove:
		{
			foreach (Resource oldItem in e.OldItems)
			{
				if (!(oldItem is ISharesContainer))
				{
					continue;
				}
				ReadOnlyObservableCollection<FileShare> shares = (oldItem as ISharesContainer).Shares;
				shareCollections.Remove(shares);
				RemoveShares(shares);
				OnPropertyChanged("Servers");
				if (netNames.Count == 0)
				{
					Execute(delegate
					{
						Empty = null;
					});
				}
			}
			break;
		}
		}
	}

	private void SharesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		switch (e.Action)
		{
		case NotifyCollectionChangedAction.Add:
		{
			foreach (FileShare newItem in e.NewItems)
			{
				AddToCollection(newItem);
			}
			break;
		}
		case NotifyCollectionChangedAction.Remove:
		{
			foreach (FileShare oldItem in e.OldItems)
			{
				RemoveFromCollection(oldItem);
			}
			break;
		}
		}
	}

	public void UpdateShares()
	{
		netNames.ExecuteQuery(NetNamesQuery);
	}

	private void NetNamesQuery(OperationResult<IClusterList<Resource>> operationResult)
	{
		if (operationResult.Error != null)
		{
			ClusterLog.LogException(operationResult.Error, "There was an error updating file shares for resource '{0}'".FormatCurrentCulture(group.Name));
		}
		foreach (NetNameResource item in operationResult.Result)
		{
			item.UpdateShares();
		}
	}

	internal void Subscribe()
	{
		if (!subscribed)
		{
			subscribed = true;
			Group ownerGroup = group;
			fileServers = (ClusterList<Resource>)new ClusterList<Resource>(cluster)
			{
				Name = "File Shares Subcription to File Share or DFS"
			}.Where((Resource r) => r.OwnerGroup == ownerGroup && ((int)r.ResourceType.ResourceKind == 8 || (int)r.ResourceType.ResourceKind == 25));
			fileServers.CollectionChanged += FileServersCollectionChanged;
			fileServers.ExecuteQuery(FileServersQuery);
		}
	}

	private void FileServersQuery<TU>(OperationResult<IClusterList<TU>> operationResult) where TU : ClusterObject
	{
		if (operationResult.Error != null)
		{
			Exist = false;
			group.Error = operationResult.Error;
			return;
		}
		Exist = fileServers.Count > 0;
		if (fileServers.Count == 0)
		{
			UpdateEmpty();
		}
	}

	private void UnsubscribeToNetNames()
	{
		if (netNames != null)
		{
			netNames.CollectionChanged -= NetNamesCollectionChanged;
			netNames = null;
		}
	}

	private void SubscribeToNetNames()
	{
		Group ownerGroup = group;
		netNames = (ClusterList<Resource>)new ClusterList<Resource>(cluster)
		{
			Name = "File Shares Subcription to Net Names"
		}.Where((Resource r) => r.OwnerGroup == ownerGroup && ((int)r.ResourceType.ResourceKind == 7 || (int)r.ResourceType.ResourceKind == 26) && (int)r.ResourceState == 2);
		netNames.CollectionChanged += NetNamesCollectionChanged;
		netNames.ExecuteQuery(NetNamesQuerySubscribe);
	}

	private void NetNamesQuerySubscribe(OperationResult<IClusterList<Resource>> operationResult)
	{
		if (operationResult.Error != null)
		{
			UpdateEmpty();
			group.Error = operationResult.Error;
		}
		else if (netNames != null && netNames.Count == 0)
		{
			UpdateEmpty();
		}
	}

	private void AggregateShares(ReadOnlyObservableCollection<FileShare> shares)
	{
		UIHelper.ExecuteOnDispatcher(delegate(ReadOnlyObservableCollection<FileShare> sharesCollection)
		{
			((ObservableCollection<FileShare>)sharesCollection.GetType().GetProperty("Items", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(sharesCollection, null)).CollectionChanged += SharesCollectionChanged;
			foreach (FileShare item in sharesCollection)
			{
				AddToCollection(item);
			}
		}, OperationType.Async, shares);
	}

	private void RemoveShares(ReadOnlyObservableCollection<FileShare> shares)
	{
		UIHelper.ExecuteOnDispatcher(delegate(ReadOnlyObservableCollection<FileShare> sharesCollection)
		{
			((ObservableCollection<FileShare>)sharesCollection.GetType().GetProperty("Items", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(sharesCollection, null)).CollectionChanged -= SharesCollectionChanged;
			foreach (FileShare item in sharesCollection)
			{
				Remove(item);
			}
		}, OperationType.Async, shares);
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

	private void AddToCollection(FileShare fileShare)
	{
		fileShare.Owner = this;
		Add(fileShare);
		UpdateEmpty();
		if (!protocols.TryGetValue(fileShare.Protocol, out var value))
		{
			value = new Counter(1);
			protocols.Add(fileShare.Protocol, value);
			OnPropertyChanged("Protocols");
		}
		else
		{
			value.Increment();
		}
	}

	private void RemoveFromCollection(FileShare fileShare)
	{
		Remove(fileShare.Key);
		fileShare.Owner = null;
		UpdateEmpty();
		if (protocols.TryGetValue(fileShare.Protocol, out var value) && value.Decrement())
		{
			protocols.Remove(fileShare.Protocol);
			OnPropertyChanged("Protocols");
		}
	}

	protected override string GetKeyForItem(FileShare item)
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

	public bool TryGetValue(string key, out FileShare value)
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

	internal virtual IEnumerator<FileShare> GetEnumerator(bool synchronized)
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

	internal void RestartFileShares()
	{
		if (netNames == null)
		{
			return;
		}
		netNames.ExecuteQuery(delegate(OperationResult<IClusterList<Resource>> or)
		{
			or.Result.ForEach(delegate(Resource nn)
			{
				((NetNameResource)nn).Restart();
			});
		});
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

	private void CopyFrom(IEnumerable<FileShare> collection)
	{
		IList<FileShare> list = base.Items;
		if (collection == null || list == null)
		{
			return;
		}
		foreach (FileShare item in collection)
		{
			list.Add(item);
		}
	}

	protected override void InsertItem(int index, FileShare item)
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

	protected override int MoveItem(FileShare item, IComparer<FileShare> comparer, out int oldIndex)
	{
		int num = -1;
		lockObject.EnterWriteLock();
		try
		{
			CheckReentrancy();
			num = base.MoveItem(item, comparer, out oldIndex);
			OnCollectionChanged(NotifyCollectionChangedAction.Move, item, num, oldIndex);
			return num;
		}
		finally
		{
			lockObject.ExitWriteLock();
		}
	}

	protected override int InsertItem(FileShare item, IComparer<FileShare> comparer)
	{
		int num = -1;
		lockObject.EnterWriteLock();
		try
		{
			if (Contains(item.Key))
			{
				return -1;
			}
			CheckReentrancy();
			num = base.InsertItem(item, comparer);
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

	protected override void AddRange(List<FileShare> items)
	{
		lockObject.EnterWriteLock();
		try
		{
			CheckReentrancy();
			base.AddRange(items);
			foreach (FileShare item in items)
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
			FileShare item = base[oldIndex];
			base.RemoveItem(oldIndex);
			base.InsertItem(newIndex, item);
			OnPropertyChanged("Item[]");
			OnCollectionChanged(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex);
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
				FileShare item = base[localIndex];
				base.RemoveItem(localIndex);
				OnPropertyChanged("Count");
				OnPropertyChanged("Item[]");
				OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, localIndex);
			}
			finally
			{
				lockObject.ExitWriteLock();
			}
		}, OperationType.Async, index);
	}

	protected override void SetItem(int index, FileShare item)
	{
		lockObject.EnterWriteLock();
		try
		{
			CheckReentrancy();
			FileShare oldItem = base[index];
			base.SetItem(index, item);
			OnPropertyChanged("Item[]");
			OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldItem, item, index);
		}
		finally
		{
			lockObject.ExitWriteLock();
		}
	}
}
