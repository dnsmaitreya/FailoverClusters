using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;
using FailoverClusters.UI.Common;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class ClusterList<T> : IClusterList<T>, IClusterLinqList<T>, IClusterLinqList, IOrderedQueryable<T>, IQueryable<T>, IEnumerable<T>, IEnumerable, IQueryable, IOrderedQueryable, IQueryProvider, IClusterList, IDisposable, IList<T>, ICollection<T>, IInputParameter, IList, ICollection, INotifyCollectionChanged, INotifyPropertyChanged, IDelayedWeakReference where T : ClusterObject
{
	internal struct InherithedSettings
	{
		public bool Static;

		public string Name;
	}

	[Serializable]
	public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
	{
		[NonSerialized]
		private readonly ClusterList<T> clusterList;

		private readonly int version;

		private int index;

		private T current;

		public T Current => current;

		object IEnumerator.Current
		{
			get
			{
				if (index == 0 || index == clusterList.size + 1)
				{
					throw new InvalidOperationException();
				}
				return Current;
			}
		}

		public event EventHandler Disposed;

		internal Enumerator(ClusterList<T> clusterList)
		{
			this = default(Enumerator);
			this.clusterList = clusterList;
			index = 0;
			version = clusterList.version;
			current = null;
			this.Disposed = null;
		}

		public void Dispose()
		{
			if (this.Disposed != null)
			{
				this.Disposed(this, new EventArgs());
			}
		}

		public bool MoveNext()
		{
			if (version == clusterList.version && index < clusterList.size)
			{
				current = clusterList.items[index].Value;
				index++;
				return true;
			}
			return MoveNextRare();
		}

		private bool MoveNextRare()
		{
			if (version != clusterList.version)
			{
				throw new InvalidOperationException();
			}
			index = clusterList.size + 1;
			current = null;
			return false;
		}

		void IEnumerator.Reset()
		{
			if (version != clusterList.version)
			{
				throw new InvalidOperationException();
			}
			index = 0;
			current = null;
		}
	}

	private class IndexValue
	{
		public int Index { get; set; }

		public T Value { get; set; }

		public IndexValue(int index, T value)
		{
			Index = index;
			Value = value;
		}
	}

	private class IndexValueComparer : IComparer<IndexValue>
	{
		public ClusterListComparer<T> Comparer { get; private set; }

		public IndexValueComparer(ClusterListComparer<T> comparer)
		{
			Comparer = comparer;
		}

		public int Compare(IndexValue x, IndexValue y)
		{
			if (Comparer == null)
			{
				throw new InvalidOperationException("Elements can be compared since there is no comparer");
			}
			return Comparer.Compare(x.Value, y.Value);
		}
	}

	private class ExecuteQueryOperation
	{
		public ResultExecution ResultExecution { get; private set; }

		public Action<OperationResult<IClusterList<T>>> OperationResult { get; private set; }

		public object Parameter { get; private set; }

		public ExecuteQueryOperation(ResultExecution resultExecution, Action<OperationResult<IClusterList<T>>> operationResult, object parameter)
		{
			ResultExecution = resultExecution;
			OperationResult = operationResult;
			Parameter = parameter;
		}
	}

	private class ComputeExpression : IDisposable
	{
		private readonly Func<T, bool> expression;

		private readonly ManualResetEventSlim waiter = new ManualResetEventSlim(initialState: false);

		private readonly ClusterList<T> clusterList;

		public ComputeExpression(ClusterList<T> clusterList, Func<T, bool> expression)
		{
			this.expression = expression;
			this.clusterList = clusterList;
			lock (this.clusterList.computeExpressions)
			{
				this.clusterList.computeExpressions.Add(this);
			}
		}

		public void Run(T item)
		{
			if (expression(item))
			{
				waiter.Set();
			}
		}

		public void Wait()
		{
			while (!waiter.Wait(TimeSpan.FromSeconds(5.0)))
			{
			}
		}

		public void Dispose()
		{
			waiter.Set();
			waiter.Dispose();
			lock (clusterList.computeExpressions)
			{
				clusterList.computeExpressions.Remove(this);
			}
		}
	}

	private readonly Cluster cluster;

	private bool isDisposed;

	private readonly Expression queryExpression;

	private InherithedSettings settings;

	private readonly bool realTime;

	private readonly Dispatcher dispatcher;

	private readonly IndexValueComparer indexValueComparer;

	private QueryInfo queryInfo;

	private bool loading;

	private ManualResetEventSlim isLoaded = new ManualResetEventSlim(initialState: false);

	private bool isEventDisposed;

	private readonly Queue<ExecuteQueryOperation> operationToExecute = new Queue<ExecuteQueryOperation>();

	private readonly object operationToExecuteLock = new object();

	private readonly List<ComputeExpression> computeExpressions = new List<ComputeExpression>();

	private bool isMonitored;

	private static readonly IndexValue[] EmptyArray = new IndexValue[0];

	private IndexValue[] items = EmptyArray;

	private int size;

	private int version;

	private readonly Dictionary<Guid, IndexValue> dictionary = new Dictionary<Guid, IndexValue>();

	private readonly ReaderWriterLockSlimFramework syncObject = new ReaderWriterLockSlimFramework(LockRecursionPolicy.SupportsRecursion);

	private object syncRoot;

	public bool DelayGcCollection => true;

	public string Name { get; set; }

	internal bool IsLoadingRtc { get; set; }

	public Cluster Cluster => cluster;

	public bool IsLoaded
	{
		get
		{
			if (IsLoadingRtc)
			{
				return false;
			}
			lock (operationToExecuteLock)
			{
				if (isLoaded == null)
				{
					return false;
				}
				return isLoaded.Wait(0);
			}
		}
	}

	public Dispatcher Dispatcher => dispatcher;

	internal QueryInfo QueryInfo
	{
		get
		{
			return queryInfo;
		}
		set
		{
			queryInfo = value;
		}
	}

	public Type ElementType => typeof(T);

	public Expression Expression => queryExpression;

	public IQueryProvider Provider => this;

	public int Capacity
	{
		get
		{
			return items.Length;
		}
		private set
		{
			if (value < size)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			if (value == items.Length)
			{
				return;
			}
			if (value > 0)
			{
				IndexValue[] destinationArray = new IndexValue[value];
				if (size > 0)
				{
					Array.Copy(items, 0, destinationArray, 0, size);
				}
				items = destinationArray;
			}
			else
			{
				items = EmptyArray;
			}
		}
	}

	public object Dependency { get; set; }

	public T this[int index]
	{
		get
		{
			syncObject.EnterReadLock();
			try
			{
				if (index > size)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return items[index].Value;
			}
			finally
			{
				syncObject.ExitReadLock();
			}
		}
		set
		{
			throw new NotSupportedException("Set Indexer operations are not supported on a ClusterList");
		}
	}

	public int Count => size;

	public bool IsReadOnly => false;

	public bool IsFixedSize => false;

	object IList.this[int index]
	{
		get
		{
			return this[index];
		}
		set
		{
			throw new NotSupportedException("Set indexer operations are not supported on a ClusterList");
		}
	}

	bool ICollection.IsSynchronized => false;

	public bool IsSynchronized => false;

	object ICollection.SyncRoot
	{
		get
		{
			if (syncRoot == null)
			{
				Interlocked.CompareExchange<object>(ref syncRoot, new object(), (object)null);
			}
			return syncRoot;
		}
	}

	public event EventHandler Loaded;

	public event PropertyChangedEventHandler PropertyChanged;

	public event NotifyCollectionChangedEventHandler CollectionChanged;

	public ClusterList(Cluster cluster)
		: this(cluster, realtime: true, Global.DefaultDispatcher, (Expression)null, default(InherithedSettings))
	{
	}

	public ClusterList(Cluster cluster, bool realtime)
		: this(cluster, realtime, Global.DefaultDispatcher, (Expression)null, default(InherithedSettings))
	{
	}

	public ClusterList(Cluster cluster, bool realtime, Dispatcher dispatcher)
		: this(cluster, realtime, dispatcher, (Expression)null, default(InherithedSettings))
	{
	}

	internal ClusterList(Cluster cluster, bool realtime, Dispatcher dispatcher, Expression expression, InherithedSettings settings)
	{
		Exceptions.ThrowIfNull(cluster, "cluster");
		PerformanceCounters.Increment("Real Time Collections");
		realTime = realtime;
		this.cluster = cluster;
		this.dispatcher = dispatcher;
		queryExpression = expression ?? Expression.Constant(this);
		this.settings = settings;
		Name = settings.Name;
		if (expression == null)
		{
			return;
		}
		QueryTranslator queryTranslator = new QueryTranslator();
		QueryInfo = queryTranslator.Translate(queryExpression);
		ClusterListComparer<T> comparer = null;
		if (queryInfo.OrderBy.Count > 0 || queryInfo.CustomOrderBy.Count > 0)
		{
			List<OrderByItem> list = new List<OrderByItem>(queryInfo.OrderBy);
			list.AddRange(queryInfo.CustomOrderBy);
			list.Sort(delegate(OrderByItem itemX, OrderByItem itemY)
			{
				if (itemX.OrderIndex < itemY.OrderIndex)
				{
					return -1;
				}
				return (itemX.OrderIndex > itemY.OrderIndex) ? 1 : 0;
			});
			comparer = list.Aggregate(null, (ClusterListComparer<T> current, OrderByItem item) => new ClusterListComparer<T>(item, current));
		}
		indexValueComparer = new IndexValueComparer(comparer);
	}

	~ClusterList()
	{
		PerformanceCounters.Decrement("Real Time Collections");
		lock (operationToExecuteLock)
		{
			if (!isEventDisposed)
			{
				isEventDisposed = true;
				isLoaded.Dispose();
				isLoaded = null;
			}
		}
		Dispose(disposing: false);
	}

	public void CancelQueryExecution()
	{
		if (queryInfo != null)
		{
			queryInfo.Cancel();
		}
	}

	public IQueryable<TResult> CreateQuery<TResult>(Expression expression)
	{
		if (expression == null)
		{
			throw new ArgumentNullException("expression");
		}
		if (!typeof(IQueryable<TResult>).IsAssignableFrom(expression.Type))
		{
			throw new InvalidOperationException(ExceptionResources.ExpressionIsNotIQueryable);
		}
		InherithedSettings inherithedSettings = default(InherithedSettings);
		inherithedSettings.Static = settings.Static;
		inherithedSettings.Name = Name;
		InherithedSettings inherithedSettings2 = inherithedSettings;
		return (IQueryable<TResult>)typeof(ClusterList<>).MakeGenericType(typeof(T)).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[5]
		{
			typeof(Cluster),
			typeof(bool),
			typeof(Dispatcher),
			typeof(Expression),
			typeof(InherithedSettings)
		}, null).Invoke(new object[5] { cluster, realTime, dispatcher, expression, inherithedSettings2 });
	}

	public IQueryable CreateQuery(Expression expression)
	{
		throw new NotSupportedException();
	}

	public TResult Execute<TResult>(Expression expression)
	{
		if (!IsLoaded && dispatcher != null && Thread.CurrentThread == dispatcher.Thread)
		{
			throw new ClusterInvalidDispatcherOperationException();
		}
		return (TResult)(object)cluster.Provider.Execute(this, expression, OperationType.Sync);
	}

	public object Execute(Expression expression)
	{
		if (!IsLoaded && dispatcher != null && Thread.CurrentThread == dispatcher.Thread)
		{
			throw new ClusterInvalidDispatcherOperationException();
		}
		return cluster.Provider.Execute(this, expression, OperationType.Sync);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposing || isDisposed)
		{
			return;
		}
		isDisposed = true;
		try
		{
			CancelQueryExecution();
			if (realTime && cluster != null)
			{
				LockAndExecute(ClearItems);
				Worker.StartExclusive(delegate
				{
					cluster.MonitorCollectionEnd(this);
				});
			}
		}
		catch (Exception exception)
		{
			ClusterLog.LogException(exception, "There was an error disposing the RTC");
		}
		this.Loaded = null;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return MyEnumerator();
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return MyEnumerator();
	}

	public IEnumerator<T> GetEnumerator()
	{
		return MyEnumerator();
	}

	private IEnumerator<T> MyEnumerator()
	{
		if (IsLoadingRtc)
		{
			return new List<T>().GetEnumerator();
		}
		if (!loading)
		{
			return RunQuery();
		}
		if (dispatcher != null && dispatcher.Thread != Thread.CurrentThread)
		{
			syncObject.EnterReadLock();
		}
		Enumerator enumerator = new Enumerator(this);
		enumerator.Disposed += delegate
		{
			if (dispatcher != null && dispatcher.Thread != Thread.CurrentThread)
			{
				syncObject.ExitReadLock();
			}
		};
		return enumerator;
	}

	private void StartMonitorQuery()
	{
		loading = true;
		if (QueryInfo != null && !isMonitored)
		{
			isMonitored = true;
			Worker.StartExclusive(delegate
			{
				cluster.MonitorCollectionStart(this);
			});
		}
	}

	private IEnumerator<T> RunQuery()
	{
		StartMonitorQuery();
		foreach (T item in cluster.Provider.Execute(this, (dispatcher == null) ? OperationType.Sync : OperationType.Async))
		{
			AddInternal(item);
			yield return item;
		}
	}

	private void RunQuerySync()
	{
		StartMonitorQuery();
		foreach (T item in cluster.Provider.Execute(this, OperationType.Sync))
		{
			Action action = null;
			syncObject.EnterWriteLock();
			try
			{
				action = AddOrInsertItemPrivate(item);
			}
			finally
			{
				syncObject.ExitWriteLock();
				action.SafeCall();
			}
		}
	}

	public void ExecuteQuery(Action<OperationResult<IClusterList<T>>> operationResult)
	{
		ExecuteQuery(ResultExecution.DoNotCare, operationResult, null);
	}

	public void ExecuteQuery(Action<OperationResult<IClusterList<T>>> operationResult, object parameter)
	{
		ExecuteQuery(ResultExecution.DoNotCare, operationResult, parameter);
	}

	public void ExecuteQuery(ResultExecution resultExecution, Action<OperationResult<IClusterList<T>>> operationResult)
	{
		ExecuteQuery(resultExecution, operationResult, null);
	}

	public void ExecuteQuery(ResultExecution resultExecution, Action<OperationResult<IClusterList<T>>> operationResult, object parameter)
	{
		if (dispatcher != null)
		{
			lock (operationToExecuteLock)
			{
				operationToExecute.Enqueue(new ExecuteQueryOperation(resultExecution, operationResult, parameter));
			}
			if (IsLoaded)
			{
				ProcessExecuteQueue(null);
				return;
			}
			if (resultExecution != ResultExecution.Sync)
			{
				using (IEnumerator<T> enumerator = GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Utilities.UnreferencedParameter(enumerator.Current);
					}
					return;
				}
			}
			RunQuerySync();
		}
		else
		{
			operationResult.SafeCall(new OperationResult<IClusterList<T>>(cluster, this, null, parameter));
		}
	}

	private void ProcessExecuteQueue(ClusterException error)
	{
		while (true)
		{
			ExecuteQueryOperation executeOperation;
			lock (operationToExecuteLock)
			{
				if (operationToExecute.Count <= 0)
				{
					break;
				}
				executeOperation = operationToExecute.Dequeue();
			}
			if (executeOperation.ResultExecution == ResultExecution.OnDispatcher)
			{
				Execute(delegate
				{
					executeOperation.OperationResult.SafeCall(new OperationResult<IClusterList<T>>(cluster, this, error, executeOperation.Parameter));
				});
			}
			else
			{
				executeOperation.OperationResult.SafeCall(new OperationResult<IClusterList<T>>(cluster, this, error, executeOperation.Parameter));
			}
		}
	}

	internal void LoadFinish(ClusterException error)
	{
		lock (operationToExecuteLock)
		{
			if (isLoaded == null)
			{
				return;
			}
			isLoaded.Set();
		}
		Execute(delegate
		{
			this.Loaded?.Invoke(this, EventArgs.Empty);
			OnPropertyChanged(new PropertyChangedEventArgs("IsLoaded"));
		});
		ProcessExecuteQueue(error);
	}

	internal void AddInternal(T item)
	{
		LockAndExecute(() => AddOrInsertItemPrivate(item));
	}

	internal void ReplaceInternal(T item)
	{
		LockAndExecute(() => ReplacePrivate(item));
	}

	public void WaitFor(Func<T, bool> expression)
	{
		if (expression == null || expression(null))
		{
			return;
		}
		using ComputeExpression computeExpression = new ComputeExpression(this, expression);
		computeExpression.Wait();
	}

	private Action ReplacePrivate(T item)
	{
		if (!dictionary.TryGetValue(item.Id, out var indexValue))
		{
			return null;
		}
		T oldItem = indexValue.Value;
		indexValue.Value = item;
		return delegate
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, oldItem, indexValue.Index));
			ComputeExpressions(item);
		};
	}

	private Action AddOrInsertItemPrivate(T item)
	{
		if (dictionary.ContainsKey(item.Id))
		{
			return null;
		}
		IndexValue indexValue = ((indexValueComparer.Comparer == null) ? AddPrivate(item) : InsertPrivate(item));
		dictionary.Add(item.Id, indexValue);
		return delegate
		{
			OnPropertyChanged("Count");
			OnPropertyChanged("Item[]");
			OnCollectionChanged(NotifyCollectionChangedAction.Add, item, indexValue.Index);
			ComputeExpressions(item);
		};
	}

	public void Insert(int index, T item)
	{
		throw new NotSupportedException("Insert operations are not supported on a ClusterList");
	}

	private IndexValue AddPrivate(T item)
	{
		int count = Count;
		if (size == items.Length)
		{
			EnsureCapacity(size + 1);
		}
		IndexValue indexValue = new IndexValue(count, item);
		items[count] = indexValue;
		size++;
		version++;
		return indexValue;
	}

	private IndexValue InsertPrivate(T item)
	{
		IndexValue indexValue = new IndexValue(-1, item);
		InsertPrivate(item, indexValue);
		return indexValue;
	}

	private void InsertPrivate(T item, IndexValue indexValue)
	{
		item.UpdateBackStoreProperties();
		int num = Array.BinarySearch(items, 0, size, indexValue, indexValueComparer);
		if (num < 0)
		{
			num = ~num;
		}
		if (num > size)
		{
			throw new IndexOutOfRangeException("index");
		}
		if (size == items.Length)
		{
			EnsureCapacity(size + 1);
		}
		if (num < size)
		{
			Array.Copy(items, num, items, num + 1, size - num);
		}
		indexValue.Index = num;
		indexValue.Value = item;
		items[num] = indexValue;
		size++;
		version++;
		for (int i = num + 1; i < size; i++)
		{
			items[i].Index = i;
		}
	}

	private void EnsureCapacity(int min)
	{
		if (items.Length < min)
		{
			int num = ((items.Length == 0) ? 4 : (items.Length * 2));
			if (num < min)
			{
				num = min;
			}
			Capacity = num;
		}
	}

	private Action ClearItems()
	{
		items = EmptyArray;
		size = 0;
		version = 0;
		dictionary.Clear();
		TrimExcessPrivate();
		isLoaded.Reset();
		loading = false;
		return delegate
		{
			OnPropertyChanged("Count");
			OnPropertyChanged("Item[]");
			OnCollectionReset();
		};
	}

	internal void RemoveInternal(T item)
	{
		RemoveInternal(item.Id);
	}

	internal void RemoveInternal(Guid guid, Action onRemoved = null)
	{
		Execute(delegate
		{
			Action action = null;
			syncObject.EnterWriteLock();
			try
			{
				if (dictionary.TryGetValue(guid, out var indexValue))
				{
					RemoveAtPrivate(indexValue.Index);
					dictionary.Remove(guid);
					action = delegate
					{
						OnPropertyChanged("Count");
						OnPropertyChanged("Item[]");
						OnCollectionChanged(NotifyCollectionChangedAction.Remove, indexValue.Value, indexValue.Index);
						ComputeExpressions(indexValue.Value);
					};
				}
			}
			finally
			{
				syncObject.ExitWriteLock();
				action?.SafeCall();
				if (onRemoved != null)
				{
					onRemoved.SafeCall();
				}
			}
		});
	}

	public void RemoveAt(int index)
	{
		throw new NotSupportedException("RemoveAt operations are not supported on a ClusterList");
	}

	private void RemoveAtPrivate(int index)
	{
		if (index > size)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		size--;
		if (index < size)
		{
			Array.Copy(items, index + 1, items, index, size - index);
		}
		items[size] = null;
		for (int i = index; i < size; i++)
		{
			items[i].Index = i;
		}
		version++;
	}

	internal void ChangedInternal(T item, string propertyName)
	{
		for (ClusterListComparer<T> clusterListComparer = indexValueComparer.Comparer; clusterListComparer != null; clusterListComparer = clusterListComparer.ChildComparer)
		{
			if (propertyName.Equals("LoadSelection", StringComparison.OrdinalIgnoreCase) || clusterListComparer.FieldName.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
			{
				LockAndExecute(delegate
				{
					int oldIndex;
					int index = Move(item, out oldIndex);
					return (index == oldIndex) ? null : ((Action)delegate
					{
						OnPropertyChanged("Item[]");
						OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, index, oldIndex));
					});
				});
				break;
			}
		}
	}

	private int Move(T item, out int oldIndex)
	{
		if (dictionary.TryGetValue(item.Id, out var value))
		{
			oldIndex = value.Index;
			item.UpdateBackStoreProperties();
			int num = Array.BinarySearch(items, 0, size, value, indexValueComparer);
			if (num == oldIndex)
			{
				if ((oldIndex == 0 || indexValueComparer.Comparer.Compare(items[oldIndex - 1].Value, item) <= 0) && (oldIndex == size - 1 || indexValueComparer.Comparer.Compare(items[oldIndex + 1].Value, item) >= 0))
				{
					return num;
				}
				RemoveAtPrivate(oldIndex);
				InsertPrivate(item, value);
				return value.Index;
			}
			RemoveAtPrivate(oldIndex);
			if (num < 0)
			{
				num = ~num;
			}
			if (num > oldIndex)
			{
				num--;
			}
			if (num > size)
			{
				throw new ArgumentOutOfRangeException("oldIndex");
			}
			if (size == items.Length)
			{
				EnsureCapacity(size + 1);
			}
			if (num < size)
			{
				Array.Copy(items, num, items, num + 1, size - num);
			}
			value.Index = num;
			items[num] = value;
			size++;
			version++;
			for (int i = num + 1; i < size; i++)
			{
				items[i].Index = i;
			}
			return value.Index;
		}
		oldIndex = -1;
		return -1;
	}

	public int IndexOf(object value)
	{
		if (!(value is T))
		{
			throw new ArgumentException(ExceptionResources.InvalidParameter, "value");
		}
		return IndexOf((T)value);
	}

	public int IndexOf(T item)
	{
		syncObject.EnterReadLock();
		try
		{
			int result = -1;
			if (dictionary.TryGetValue(item.Id, out var value))
			{
				result = value.Index;
			}
			return result;
		}
		finally
		{
			syncObject.ExitReadLock();
		}
	}

	public void Add(T item)
	{
		if (realTime)
		{
			throw new InvalidOperationException(ExceptionResources.ItemCannotBelongToACluster);
		}
		if (item.Name == null)
		{
			throw new ArgumentNullException("item", ExceptionResources.ItemNameMustHaveValue);
		}
		throw new NotSupportedException("Elements cannot be added or removed from Real Time Collections");
	}

	public void Clear()
	{
		LockAndExecute(ClearItems);
	}

	public bool TryGetValue(Guid id, out T value)
	{
		syncObject.EnterReadLock();
		try
		{
			if (dictionary.TryGetValue(id, out var value2))
			{
				value = items[value2.Index].Value;
				return true;
			}
			value = null;
			return false;
		}
		finally
		{
			syncObject.ExitReadLock();
		}
	}

	public bool Contains(T item)
	{
		syncObject.EnterReadLock();
		try
		{
			return dictionary.ContainsKey(item.Id);
		}
		finally
		{
			syncObject.ExitReadLock();
		}
	}

	public bool Contains(Guid key)
	{
		syncObject.EnterReadLock();
		try
		{
			return dictionary.ContainsKey(key);
		}
		finally
		{
			syncObject.ExitReadLock();
		}
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		if (array != null && array.Rank != 1)
		{
			throw new ArgumentException(ExceptionResources.RankMultiDimNotSupported);
		}
		Exceptions.ThrowIfNull(array, "array");
		syncObject.EnterReadLock();
		try
		{
			if (arrayIndex < array.GetLowerBound(0) || array.GetLength(0) < arrayIndex + size)
			{
				throw new ArgumentException(ExceptionResources.ArrayIndexOutOfBound);
			}
			for (int i = 0; i < size; i++)
			{
				array[i + arrayIndex] = items[i].Value;
			}
		}
		finally
		{
			syncObject.ExitReadLock();
		}
	}

	public bool Remove(T item)
	{
		throw new NotSupportedException("Remove operations are not supported on a ClusterList");
	}

	public void TrimExcess()
	{
		Execute(delegate
		{
			if (dispatcher != null && dispatcher.Thread != Thread.CurrentThread)
			{
				syncObject.EnterWriteLock();
			}
			try
			{
				TrimExcessPrivate();
			}
			finally
			{
				if (dispatcher != null && dispatcher.Thread != Thread.CurrentThread)
				{
					syncObject.ExitWriteLock();
				}
			}
		});
	}

	private void TrimExcessPrivate()
	{
		int num = (int)((double)items.Length * 0.9);
		if (size < num)
		{
			Capacity = size;
		}
	}

	public ClusterList<T> AsStatic()
	{
		settings.Static = true;
		return this;
	}

	private void Execute(Action function)
	{
		if ((dispatcher != null && dispatcher.CheckAccess()) || dispatcher == null)
		{
			function();
		}
		else
		{
			dispatcher.EnqueueInvoke(function);
		}
	}

	private void LockAndExecute(Func<Action> function)
	{
		Action function2 = delegate
		{
			syncObject.EnterWriteLock();
			Action action;
			try
			{
				action = function();
			}
			finally
			{
				syncObject.ExitWriteLock();
			}
			action.SafeCall();
		};
		Execute(function2);
	}

	private TU Execute<TU>(Func<TU> function)
	{
		if ((dispatcher != null && dispatcher.CheckAccess()) || dispatcher == null)
		{
			return function();
		}
		dispatcher.EnqueueInvoke(function);
		return default(TU);
	}

	private void OnCollectionReset()
	{
		OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
	}

	private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
	{
		OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
	}

	private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
	{
		if (this.CollectionChanged != null)
		{
			this.CollectionChanged(this, e);
		}
	}

	private void OnPropertyChanged(PropertyChangedEventArgs e)
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

	public int Add(object value)
	{
		throw new NotSupportedException("Add operations are not supported on a ClusterList");
	}

	public bool Contains(object value)
	{
		return Contains((T)value);
	}

	public void Insert(int index, object value)
	{
		throw new NotSupportedException("Insert operations are not supported on a ClusterList");
	}

	public void Remove(object value)
	{
		throw new NotSupportedException("Remove operations are not supported on a ClusterList");
	}

	public void CopyTo(Array array, int index)
	{
		if (array != null && array.Rank != 1)
		{
			throw new ArgumentException(ExceptionResources.RankMultiDimNotSupported);
		}
		syncObject.EnterReadLock();
		try
		{
			object[] array2 = new object[size];
			for (int i = 0; i < size; i++)
			{
				array2[i] = items[i].Value;
			}
			Array.Copy(array2, 0, array, index, size);
		}
		finally
		{
			syncObject.ExitReadLock();
		}
	}

	public IClusterList<T> ForceLoadStart()
	{
		ExecuteQuery(delegate
		{
		});
		return this;
	}

	private void ComputeExpressions(T item)
	{
		lock (computeExpressions)
		{
			foreach (ComputeExpression computeExpression in computeExpressions)
			{
				computeExpression.Run(item);
			}
		}
	}
}

