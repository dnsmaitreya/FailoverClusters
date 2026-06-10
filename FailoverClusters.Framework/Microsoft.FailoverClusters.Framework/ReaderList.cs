using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Security;
using System.Threading;
using FailoverClusters.UI.Common;
using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

[Serializable]
[DebuggerDisplay("Count = {Count}")]
public class ReaderList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection
{
	[Serializable]
	public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
	{
		private readonly ReaderList<T> list;

		private readonly int version;

		private int index;

		private T current;

		public T Current => current;

		object IEnumerator.Current
		{
			get
			{
				if (index == 0 || index == list.size + 1)
				{
					throw new InvalidOperationException();
				}
				return Current;
			}
		}

		public event EventHandler Disposed;

		internal Enumerator(ReaderList<T> list)
		{
			this.list = list;
			index = 0;
			version = list.version;
			current = default(T);
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
			ReaderList<T> readerList = list;
			if (version == readerList.version && index < readerList.size)
			{
				current = readerList.items[index];
				index++;
				return true;
			}
			return MoveNextRare();
		}

		private bool MoveNextRare()
		{
			if (version != list.version)
			{
				throw new InvalidOperationException();
			}
			index = list.size + 1;
			current = default(T);
			return false;
		}

		void IEnumerator.Reset()
		{
			if (version != list.version)
			{
				throw new InvalidOperationException();
			}
			index = 0;
			current = default(T);
		}
	}

	[Serializable]
	internal class SynchronizedList : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	{
		private readonly ReaderList<T> list;

		[NonSerialized]
		private readonly ReaderWriterLockSlimFramework root = new ReaderWriterLockSlimFramework(LockRecursionPolicy.SupportsRecursion);

		public int Count
		{
			get
			{
				try
				{
					root.EnterReadLock();
					return list.Count;
				}
				finally
				{
					root.ExitReadLock();
				}
			}
		}

		public bool IsReadOnly => ((IList)list).IsReadOnly;

		public T this[int index]
		{
			get
			{
				try
				{
					root.EnterReadLock();
					return list[index];
				}
				finally
				{
					root.ExitReadLock();
				}
			}
			set
			{
				try
				{
					root.EnterWriteLock();
					list[index] = value;
				}
				finally
				{
					root.ExitWriteLock();
				}
			}
		}

		internal SynchronizedList(ReaderList<T> list)
		{
			root.LockType = typeof(SynchronizedList);
			this.list = list;
		}

		~SynchronizedList()
		{
			root.Dispose();
		}

		public void Add(T item)
		{
			try
			{
				root.EnterWriteLock();
				list.Add(item);
			}
			finally
			{
				root.ExitWriteLock();
			}
		}

		public void Clear()
		{
			try
			{
				root.EnterWriteLock();
				list.Clear();
			}
			finally
			{
				root.ExitWriteLock();
			}
		}

		public bool Contains(T item)
		{
			try
			{
				root.EnterReadLock();
				return list.Contains(item);
			}
			finally
			{
				root.ExitReadLock();
			}
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			try
			{
				root.ExitWriteLock();
				list.CopyTo(array, arrayIndex);
			}
			finally
			{
				root.ExitWriteLock();
			}
		}

		public int IndexOf(T item)
		{
			try
			{
				root.EnterReadLock();
				return list.IndexOf(item);
			}
			finally
			{
				root.ExitReadLock();
			}
		}

		public void Insert(int index, T item)
		{
			try
			{
				root.EnterWriteLock();
				list.Insert(index, item);
			}
			finally
			{
				root.ExitWriteLock();
			}
		}

		public bool Remove(T item)
		{
			try
			{
				root.EnterWriteLock();
				return list.Remove(item);
			}
			finally
			{
				root.ExitWriteLock();
			}
		}

		public void RemoveAt(int index)
		{
			try
			{
				root.EnterWriteLock();
				list.RemoveAt(index);
			}
			finally
			{
				root.ExitWriteLock();
			}
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return GetEnumerator(unlockAtDispose: false);
		}

		public IEnumerator<T> GetEnumerator(bool unlockAtDispose)
		{
			Enumerator enumerator;
			try
			{
				root.EnterReadLock();
				enumerator = list.GetEnumerator();
				enumerator.Disposed += delegate
				{
					if (unlockAtDispose)
					{
						root.ExitReadLock();
					}
				};
			}
			finally
			{
				if (!unlockAtDispose)
				{
					root.ExitReadLock();
				}
			}
			return enumerator;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			try
			{
				root.EnterReadLock();
				return list.GetEnumerator();
			}
			finally
			{
				root.ExitReadLock();
			}
		}
	}

	private enum ExceptionArgument
	{
		obj,
		dictionary,
		dictionaryCreationThreshold,
		array,
		info,
		key,
		collection,
		list,
		match,
		converter,
		queue,
		stack,
		capacity,
		index,
		startIndex,
		value,
		count,
		arrayIndex,
		name,
		mode,
		item,
		options,
		view
	}

	private const int DefaultCapacity = 4;

	private static readonly T[] EmptyArray;

	private T[] items;

	private int size;

	[NonSerialized]
	private object syncRoot;

	private int version;

	public int Capacity
	{
		get
		{
			return items.Length;
		}
		set
		{
			if (value < size)
			{
				throw new ArgumentOutOfRangeException(ExceptionResources.InvalidCapacity);
			}
			if (value == items.Length)
			{
				return;
			}
			if (value > 0)
			{
				T[] destinationArray = new T[value];
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

	public int Count => size;

	public T this[int index]
	{
		get
		{
			if (index >= size)
			{
				throw new ArgumentOutOfRangeException("index", ExceptionResources.ParameterOutOfRange);
			}
			return items[index];
		}
		set
		{
			if (index >= size)
			{
				throw new ArgumentOutOfRangeException("index", ExceptionResources.ParameterOutOfRange);
			}
			items[index] = value;
			version++;
		}
	}

	bool ICollection<T>.IsReadOnly => false;

	bool ICollection.IsSynchronized => false;

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

	bool IList.IsFixedSize => false;

	bool IList.IsReadOnly => false;

	object IList.this[int index]
	{
		get
		{
			return this[index];
		}
		set
		{
			IfNullAndNullsAreIllegalThenThrow<T>(value);
			try
			{
				this[index] = (T)value;
			}
			catch (InvalidCastException)
			{
				throw;
			}
		}
	}

	static ReaderList()
	{
		EmptyArray = new T[0];
	}

	public ReaderList()
	{
		items = EmptyArray;
	}

	public ReaderList(IEnumerable<T> collection)
	{
		Exceptions.ThrowIfNull(collection, "collection");
		if (collection is ICollection<T> collection2)
		{
			int count = collection2.Count;
			items = new T[count];
			collection2.CopyTo(items, 0);
			size = count;
			return;
		}
		size = 0;
		items = new T[4];
		foreach (T item in collection)
		{
			Add(item);
		}
	}

	public ReaderList(int capacity)
	{
		if (capacity < 0)
		{
			throw new ArgumentOutOfRangeException(ExceptionResources.InvalidCapacity);
		}
		items = new T[capacity];
	}

	public void Add(T item)
	{
		if (size == items.Length)
		{
			EnsureCapacity(size + 1);
		}
		items[size++] = item;
		version++;
	}

	public void AddRange(IEnumerable<T> collection)
	{
		InsertRange(size, collection);
	}

	public ReadOnlyCollection<T> AsReadOnly()
	{
		return new ReadOnlyCollection<T>(this);
	}

	public int BinarySearch(T item)
	{
		return BinarySearch(0, Count, item, null);
	}

	public int BinarySearch(T item, IComparer<T> comparer)
	{
		return BinarySearch(0, Count, item, comparer);
	}

	public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
	{
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException(ExceptionResources.IndexParameterLessThanZero);
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException(ExceptionResources.CountParameterLessThanZero);
		}
		if (size - index < count)
		{
			throw new ArgumentException(ExceptionResources.InvalidLengthParameter);
		}
		return Array.BinarySearch(items, index, count, item, comparer);
	}

	public void Clear()
	{
		if (size > 0)
		{
			Array.Clear(items, 0, size);
			size = 0;
		}
		version++;
	}

	public bool Contains(T item)
	{
		if (item == null)
		{
			for (int i = 0; i < size; i++)
			{
				if (items[i] == null)
				{
					return true;
				}
			}
			return false;
		}
		EqualityComparer<T> @default = EqualityComparer<T>.Default;
		for (int j = 0; j < size; j++)
		{
			if (@default.Equals(items[j], item))
			{
				return true;
			}
		}
		return false;
	}

	public ReaderList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
	{
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		ReaderList<TOutput> readerList = new ReaderList<TOutput>(size);
		for (int i = 0; i < size; i++)
		{
			readerList.items[i] = converter(items[i]);
		}
		readerList.size = size;
		return readerList;
	}

	public void CopyTo(T[] array)
	{
		CopyTo(array, 0);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		Array.Copy(items, 0, array, arrayIndex, size);
	}

	public void CopyTo(int index, T[] array, int arrayIndex, int count)
	{
		if (size - index < count)
		{
			throw new ArgumentException(ExceptionResources.invalidIndexParameter);
		}
		Array.Copy(items, index, array, arrayIndex, count);
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

	public bool Exists(Predicate<T> match)
	{
		return FindIndex(match) != -1;
	}

	public T Find(Predicate<T> match)
	{
		if (match == null)
		{
			throw new ArgumentNullException("match");
		}
		for (int i = 0; i < size; i++)
		{
			if (match(items[i]))
			{
				return items[i];
			}
		}
		return default(T);
	}

	public List<T> FindAll(Predicate<T> match)
	{
		if (match == null)
		{
			throw new ArgumentNullException("match");
		}
		List<T> list = new List<T>();
		for (int i = 0; i < size; i++)
		{
			if (match(items[i]))
			{
				list.Add(items[i]);
			}
		}
		return list;
	}

	public int FindIndex(Predicate<T> match)
	{
		return FindIndex(0, size, match);
	}

	public int FindIndex(int startIndex, Predicate<T> match)
	{
		return FindIndex(startIndex, size - startIndex, match);
	}

	public int FindIndex(int startIndex, int count, Predicate<T> match)
	{
		if (startIndex > size)
		{
			throw new ArgumentOutOfRangeException("startIndex", ExceptionResources.ParameterOutOfRange);
		}
		if (count < 0 || startIndex > size - count)
		{
			throw new ArgumentOutOfRangeException("count", ExceptionResources.ParameterOutOfRange);
		}
		if (match == null)
		{
			throw new ArgumentNullException("match");
		}
		int num = startIndex + count;
		for (int i = startIndex; i < num; i++)
		{
			if (match(items[i]))
			{
				return i;
			}
		}
		return -1;
	}

	public T FindLast(Predicate<T> match)
	{
		if (match == null)
		{
			throw new ArgumentNullException("match");
		}
		for (int num = size - 1; num >= 0; num--)
		{
			if (match(items[num]))
			{
				return items[num];
			}
		}
		return default(T);
	}

	public int FindLastIndex(Predicate<T> match)
	{
		return FindLastIndex(size - 1, size, match);
	}

	public int FindLastIndex(int startIndex, Predicate<T> match)
	{
		return FindLastIndex(startIndex, startIndex + 1, match);
	}

	public int FindLastIndex(int startIndex, int count, Predicate<T> match)
	{
		if (match == null)
		{
			throw new ArgumentNullException("match");
		}
		if (size == 0)
		{
			if (startIndex != -1)
			{
				throw new ArgumentOutOfRangeException("startIndex", ExceptionResources.ParameterOutOfRange);
			}
		}
		else if (startIndex >= size)
		{
			throw new ArgumentOutOfRangeException("startIndex", ExceptionResources.ParameterOutOfRange);
		}
		if (count < 0 || startIndex - count + 1 < 0)
		{
			throw new ArgumentOutOfRangeException("count", ExceptionResources.ParameterOutOfRange);
		}
		int num = startIndex - count;
		for (int num2 = startIndex; num2 > num; num2--)
		{
			if (match(items[num2]))
			{
				return num2;
			}
		}
		return -1;
	}

	public void ForEach(Action<T> action)
	{
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		for (int i = 0; i < size; i++)
		{
			action(items[i]);
		}
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	internal Enumerator GetEnumerator(ReaderWriterLockSlimFramework readerLock)
	{
		if (readerLock == null)
		{
			return new Enumerator(this);
		}
		readerLock.EnterReadLock();
		Enumerator result = new Enumerator(this);
		result.Disposed += delegate
		{
			readerLock.ExitReadLock();
		};
		return result;
	}

	public ReaderList<T> GetRange(int index, int count)
	{
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException("index", ExceptionResources.ParameterOutOfRange);
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count", ExceptionResources.ParameterOutOfRange);
		}
		if (size - index < count)
		{
			throw new ArgumentOutOfRangeException("index", ExceptionResources.ParameterOutOfRange);
		}
		ReaderList<T> readerList = new ReaderList<T>(count);
		Array.Copy(items, index, readerList.items, 0, count);
		readerList.size = count;
		return readerList;
	}

	public int IndexOf(T item)
	{
		return Array.IndexOf(items, item, 0, size);
	}

	public int IndexOf(T item, int index)
	{
		if (index > size)
		{
			throw new ArgumentOutOfRangeException("index", ExceptionResources.ParameterOutOfRange);
		}
		return Array.IndexOf(items, item, index, size - index);
	}

	public int IndexOf(T item, int index, int count)
	{
		if (index > size)
		{
			throw new ArgumentOutOfRangeException("index", ExceptionResources.ParameterOutOfRange);
		}
		if (count < 0 || index > size - count)
		{
			throw new ArgumentOutOfRangeException("count", ExceptionResources.ParameterOutOfRange);
		}
		return Array.IndexOf(items, item, index, count);
	}

	internal int Move(T item, IComparer<T> comparer, out int oldIndex)
	{
		oldIndex = IndexOf(item);
		RemoveAt(oldIndex);
		return Insert(item, comparer);
	}

	public int Insert(T item, IComparer<T> comparer)
	{
		int num = Array.BinarySearch(items, 0, size, item, comparer);
		if (num < 0)
		{
			num = ~num;
		}
		if (num > size)
		{
			throw new ArgumentOutOfRangeException("item", ExceptionResources.ParameterOutOfRange);
		}
		if (size == items.Length)
		{
			EnsureCapacity(size + 1);
		}
		if (num < size)
		{
			Array.Copy(items, num, items, num + 1, size - num);
		}
		items[num] = item;
		size++;
		version++;
		return num;
	}

	public void Insert(int index, T item)
	{
		if (index > size)
		{
			throw new ArgumentOutOfRangeException("index", ExceptionResources.ParameterOutOfRange);
		}
		if (size == items.Length)
		{
			EnsureCapacity(size + 1);
		}
		if (index < size)
		{
			Array.Copy(items, index, items, index + 1, size - index);
		}
		items[index] = item;
		size++;
		version++;
	}

	public void InsertRange(int index, IEnumerable<T> collection)
	{
		if (collection == null)
		{
			throw new ArgumentOutOfRangeException("collection", ExceptionResources.ParameterOutOfRange);
		}
		if (index > size)
		{
			throw new ArgumentOutOfRangeException("index", ExceptionResources.ParameterOutOfRange);
		}
		if (collection is ICollection<T> collection2)
		{
			int count = collection2.Count;
			if (count > 0)
			{
				EnsureCapacity(size + count);
				if (index < size)
				{
					Array.Copy(items, index, items, index + count, size - index);
				}
				if (this == collection2)
				{
					Array.Copy(items, 0, items, index, index);
					Array.Copy(items, index + count, items, index * 2, size - index);
				}
				else
				{
					T[] array = new T[count];
					collection2.CopyTo(array, 0);
					array.CopyTo(items, index);
				}
				size += count;
			}
		}
		else
		{
			using IEnumerator<T> enumerator = collection.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Insert(index++, enumerator.Current);
			}
		}
		version++;
	}

	private static bool IsCompatibleObject(object value)
	{
		if (!(value is T))
		{
			if (value == null)
			{
				return default(T) == null;
			}
			return false;
		}
		return true;
	}

	public int LastIndexOf(T item)
	{
		if (size == 0)
		{
			return -1;
		}
		return LastIndexOf(item, size - 1, size);
	}

	public int LastIndexOf(T item, int index)
	{
		if (index >= size)
		{
			throw new ArgumentOutOfRangeException("index", ExceptionResources.ParameterOutOfRange);
		}
		return LastIndexOf(item, index, index + 1);
	}

	public int LastIndexOf(T item, int index, int count)
	{
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException("index", ExceptionResources.ParameterOutOfRange);
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count", ExceptionResources.ParameterOutOfRange);
		}
		if (size == 0)
		{
			return -1;
		}
		if (index >= size)
		{
			throw new ArgumentOutOfRangeException("index", ExceptionResources.ParameterOutOfRange);
		}
		if (count > index + 1)
		{
			throw new ArgumentOutOfRangeException("count", ExceptionResources.ParameterOutOfRange);
		}
		return Array.LastIndexOf(items, item, index, count);
	}

	public bool Remove(T item)
	{
		int num = IndexOf(item);
		if (num >= 0)
		{
			RemoveAt(num);
			return true;
		}
		return false;
	}

	public int RemoveAll(Predicate<T> match)
	{
		if (match == null)
		{
			throw new ArgumentNullException("match");
		}
		int i;
		for (i = 0; i < size && !match(items[i]); i++)
		{
		}
		if (i >= size)
		{
			return 0;
		}
		int j = i + 1;
		while (j < size)
		{
			for (; j < size && match(items[j]); j++)
			{
			}
			if (j < size)
			{
				items[i++] = items[j++];
			}
		}
		Array.Clear(items, i, size - i);
		int result = size - i;
		size = i;
		version++;
		return result;
	}

	public void RemoveAt(int index)
	{
		if (index >= size)
		{
			throw new ArgumentOutOfRangeException("index", ExceptionResources.ParameterOutOfRange);
		}
		size--;
		if (index < size)
		{
			Array.Copy(items, index + 1, items, index, size - index);
		}
		items[size] = default(T);
		version++;
	}

	public void RemoveRange(int index, int count)
	{
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException("index", ExceptionResources.ParameterOutOfRange);
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count", ExceptionResources.ParameterOutOfRange);
		}
		if (size - index < count)
		{
			throw new ArgumentOutOfRangeException("index", ExceptionResources.ParameterOutOfRange);
		}
		if (count > 0)
		{
			size -= count;
			if (index < size)
			{
				Array.Copy(items, index + count, items, index, size - index);
			}
			Array.Clear(items, size, count);
			version++;
		}
	}

	public void Reverse()
	{
		Reverse(0, Count);
	}

	public void Reverse(int index, int count)
	{
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException("index", ExceptionResources.ParameterOutOfRange);
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count", ExceptionResources.ParameterOutOfRange);
		}
		if (size - index < count)
		{
			throw new ArgumentOutOfRangeException("index", ExceptionResources.ParameterOutOfRange);
		}
		Array.Reverse(items, index, count);
		version++;
	}

	public void Sort()
	{
		Sort(0, Count, null);
	}

	public void Sort(IComparer<T> comparer)
	{
		Sort(0, Count, comparer);
	}

	public void Sort(Comparison<T> comparison)
	{
		if (comparison == null)
		{
			throw new ArgumentNullException("comparison");
		}
		if (size > 0)
		{
			IComparer<T> comparer = new FunctorComparer<T>(comparison);
			Array.Sort(items, 0, size, comparer);
		}
	}

	public void Sort(int index, int count, IComparer<T> comparer)
	{
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException("index", ExceptionResources.ParameterOutOfRange);
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count", ExceptionResources.ParameterOutOfRange);
		}
		if (size - index < count)
		{
			throw new ArgumentOutOfRangeException("index", ExceptionResources.ParameterOutOfRange);
		}
		Array.Sort(items, index, count, comparer);
		version++;
	}

	internal static IList<T> Synchronized(ReaderList<T> list)
	{
		return new SynchronizedList(list);
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return new Enumerator(this);
	}

	void ICollection.CopyTo(Array array, int arrayIndex)
	{
		if (array != null && array.Rank != 1)
		{
			throw new ArgumentException(ExceptionResources.MultidimentionalArrayNotSupported);
		}
		try
		{
			Array.Copy(items, 0, array, arrayIndex, size);
		}
		catch (ArrayTypeMismatchException)
		{
			throw new ArgumentException(ExceptionResources.InvalidArrayType);
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new Enumerator(this);
	}

	int IList.Add(object item)
	{
		IfNullAndNullsAreIllegalThenThrow<T>(item);
		try
		{
			Add((T)item);
		}
		catch (InvalidCastException)
		{
			throw;
		}
		return Count - 1;
	}

	[SecuritySafeCritical]
	bool IList.Contains(object item)
	{
		if (IsCompatibleObject(item))
		{
			return Contains((T)item);
		}
		return false;
	}

	int IList.IndexOf(object item)
	{
		if (IsCompatibleObject(item))
		{
			return IndexOf((T)item);
		}
		return -1;
	}

	void IList.Insert(int index, object item)
	{
		IfNullAndNullsAreIllegalThenThrow<T>(item);
		try
		{
			Insert(index, (T)item);
		}
		catch (InvalidCastException)
		{
			throw;
		}
	}

	[SecuritySafeCritical]
	void IList.Remove(object item)
	{
		if (IsCompatibleObject(item))
		{
			Remove((T)item);
		}
	}

	public T[] ToArray()
	{
		T[] array = new T[size];
		Array.Copy(items, 0, array, 0, size);
		return array;
	}

	public void TrimExcess()
	{
		int num = (int)((double)items.Length * 0.9);
		if (size < num)
		{
			Capacity = size;
		}
	}

	public bool TrueForAll(Predicate<T> match)
	{
		if (match == null)
		{
			throw new ArgumentNullException("match");
		}
		for (int i = 0; i < size; i++)
		{
			if (!match(items[i]))
			{
				return false;
			}
		}
		return true;
	}

	private void IfNullAndNullsAreIllegalThenThrow<X>(object value)
	{
		if (value == null && default(X) != null)
		{
			throw new ArgumentNullException("value");
		}
	}
}

