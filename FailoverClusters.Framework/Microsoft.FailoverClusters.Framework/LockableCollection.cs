using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Threading;
using FailoverClusters.UI.Common;
using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

[Serializable]
[DebuggerDisplay("Count = {Count}")]
[ComVisible(false)]
public class LockableCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection
{
	private readonly IList<T> items;

	private readonly bool realTime;

	[NonSerialized]
	private readonly Dispatcher dispatcher;

	[NonSerialized]
	private object syncRoot;

	public virtual int Count => items.Count;

	public T this[int index]
	{
		get
		{
			return items[index];
		}
		set
		{
			if (items.IsReadOnly)
			{
				throw new NotSupportedException();
			}
			if (index < 0 || index >= items.Count)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			SetItem(index, value);
		}
	}

	public IList<T> Items => items;

	bool ICollection<T>.IsReadOnly => items.IsReadOnly;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot
	{
		get
		{
			if (syncRoot == null)
			{
				if (items is ICollection collection)
				{
					syncRoot = collection.SyncRoot;
				}
				else
				{
					Interlocked.CompareExchange<object>(ref syncRoot, new object(), (object)null);
				}
			}
			return syncRoot;
		}
	}

	bool IList.IsFixedSize
	{
		get
		{
			if (items is IList list)
			{
				return list.IsFixedSize;
			}
			return items.IsReadOnly;
		}
	}

	bool IList.IsReadOnly => items.IsReadOnly;

	object IList.this[int index]
	{
		get
		{
			return items[index];
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
				throw new ArgumentException("value");
			}
		}
	}

	public LockableCollection(bool realTime, Dispatcher dispatcher)
	{
		if (realTime)
		{
			this.dispatcher = dispatcher;
		}
		this.realTime = realTime;
		items = new ReaderList<T>();
	}

	public LockableCollection(IList<T> list)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		items = list;
	}

	protected void TrimExcess()
	{
		if (items is ReaderList<T> readerList)
		{
			readerList.TrimExcess();
		}
		else if (items is List<T> list)
		{
			list.TrimExcess();
		}
	}

	public virtual void Add(T item)
	{
		if (realTime)
		{
			throw new ClusterListInvalidOperationException();
		}
		AddInternal(item, null);
	}

	protected virtual void AddInternal(T item, IComparer<T> comparer)
	{
		Execute(delegate
		{
			if (items.IsReadOnly)
			{
				throw new NotSupportedException();
			}
			if (comparer == null)
			{
				int count = items.Count;
				InsertItem(count, item);
			}
			else
			{
				InsertItem(item, comparer);
			}
		});
	}

	protected virtual void MoveInternal(T item, IComparer<T> comparer)
	{
		Execute(delegate
		{
			if (items.IsReadOnly)
			{
				throw new NotSupportedException();
			}
			if (comparer != null)
			{
				MoveItem(item, comparer, out var _);
			}
		});
	}

	internal void AddRangeInternal(List<T> newItems)
	{
		Execute(delegate
		{
			if (items.IsReadOnly)
			{
				throw new NotSupportedException();
			}
			AddRange(newItems);
		});
	}

	public void Clear()
	{
		if (realTime)
		{
			throw new ClusterListInvalidOperationException();
		}
		ClearInternal();
	}

	internal void ClearInternal()
	{
		Execute(delegate
		{
			if (items.IsReadOnly)
			{
				throw new NotSupportedException();
			}
			ClearItems();
		});
	}

	protected virtual void ClearItems()
	{
		Execute(delegate
		{
			items.Clear();
		});
	}

	public bool Contains(T item)
	{
		return items.Contains(item);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		items.CopyTo(array, arrayIndex);
	}

	public virtual IEnumerator<T> GetEnumerator()
	{
		return items.GetEnumerator();
	}

	internal virtual IEnumerator<T> GetEnumerator(ReaderWriterLockSlimFramework readerLock)
	{
		if (items is ReaderList<T>)
		{
			return ((ReaderList<T>)items).GetEnumerator(readerLock);
		}
		return items.GetEnumerator();
	}

	public int IndexOf(T item)
	{
		return items.IndexOf(item);
	}

	public void Insert(int index, T item)
	{
		if (realTime)
		{
			throw new ClusterListInvalidOperationException();
		}
		InsertInternal(index, item);
	}

	internal void InsertInternal(int index, T item)
	{
		Execute(delegate
		{
			if (items.IsReadOnly)
			{
				throw new NotSupportedException(ExceptionResources.ReadOnlyCollectionNotSupported);
			}
			if (index < 0 || index > items.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			InsertItem(index, item);
		});
	}

	protected virtual void InsertItem(int index, T item)
	{
		items.Insert(index, item);
	}

	protected virtual int InsertItem(T item, IComparer<T> comparer)
	{
		return ((ReaderList<T>)items).Insert(item, comparer);
	}

	protected virtual int MoveItem(T item, IComparer<T> comparer, out int oldIndex)
	{
		return ((ReaderList<T>)items).Move(item, comparer, out oldIndex);
	}

	protected virtual void AddRange(List<T> newItems)
	{
		foreach (T newItem in newItems)
		{
			items.Add(newItem);
		}
	}

	private static bool IsCompatibleObject(object value)
	{
		if (!(value is T))
		{
			if (value == null)
			{
				return !typeof(T).IsValueType;
			}
			return false;
		}
		return true;
	}

	public bool Remove(T item)
	{
		if (realTime)
		{
			throw new ClusterListInvalidOperationException();
		}
		return RemoveInternal(item);
	}

	internal bool RemoveInternal(T item)
	{
		return Execute(delegate
		{
			if (items.IsReadOnly)
			{
				throw new NotSupportedException();
			}
			int num = items.IndexOf(item);
			if (num < 0)
			{
				return false;
			}
			RemoveItem(num);
			return true;
		});
	}

	internal void RemoveAtInternal(int index)
	{
		if (realTime)
		{
			throw new ClusterListInvalidOperationException();
		}
		RemoveAt(index);
	}

	public void RemoveAt(int index)
	{
		Execute(delegate
		{
			if (items.IsReadOnly)
			{
				throw new NotSupportedException(ExceptionResources.ReadOnlyCollectionNotSupported);
			}
			if (index < 0 || index >= items.Count)
			{
				throw new ArgumentOutOfRangeException("index", ExceptionResources.ParameterOutOfRange);
			}
			RemoveItem(index);
		});
	}

	protected virtual void RemoveItem(int index)
	{
		items.RemoveAt(index);
	}

	protected virtual void SetItem(int index, T item)
	{
		items[index] = item;
	}

	void ICollection.CopyTo(Array array, int index)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (array.Rank != 1)
		{
			throw new ArgumentException(ExceptionResources.MultidimentionalArrayNotSupported);
		}
		if (array.GetLowerBound(0) != 0)
		{
			throw new ArgumentException(ExceptionResources.ArrayIsNotZeroLowerBound);
		}
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		if (array.Length - index < Count)
		{
			throw new ArgumentException(ExceptionResources.ArrayIsTooSmall);
		}
		if (array is T[] array2)
		{
			items.CopyTo(array2, index);
			return;
		}
		Type elementType = array.GetType().GetElementType();
		Type typeFromHandle = typeof(T);
		if (!elementType.IsAssignableFrom(typeFromHandle) && !typeFromHandle.IsAssignableFrom(elementType))
		{
			throw new ArgumentException(ExceptionResources.InvalidArrayType);
		}
		if (!(array is object[] array3))
		{
			throw new ArgumentException(ExceptionResources.InvalidArrayType);
		}
		int count = items.Count;
		try
		{
			for (int i = 0; i < count; i++)
			{
				array3[index++] = items[i];
			}
		}
		catch (ArrayTypeMismatchException)
		{
			throw new ArgumentException(ExceptionResources.InvalidArrayType);
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return items.GetEnumerator();
	}

	int IList.Add(object value)
	{
		if (items.IsReadOnly)
		{
			throw new NotSupportedException();
		}
		IfNullAndNullsAreIllegalThenThrow<T>(value);
		try
		{
			Add((T)value);
		}
		catch (InvalidCastException)
		{
			throw new ArgumentException("value");
		}
		return Count - 1;
	}

	bool IList.Contains(object value)
	{
		if (IsCompatibleObject(value))
		{
			return Contains((T)value);
		}
		return false;
	}

	int IList.IndexOf(object value)
	{
		if (IsCompatibleObject(value))
		{
			return IndexOf((T)value);
		}
		return -1;
	}

	void IList.Insert(int index, object value)
	{
		if (items.IsReadOnly)
		{
			throw new NotSupportedException(ExceptionResources.ReadOnlyCollectionNotSupported);
		}
		IfNullAndNullsAreIllegalThenThrow<T>(value);
		try
		{
			Insert(index, (T)value);
		}
		catch (InvalidCastException)
		{
			throw new ArgumentException(ExceptionResources.WrongValueTypeArgument);
		}
	}

	void IList.Remove(object value)
	{
		if (items.IsReadOnly)
		{
			throw new NotSupportedException(ExceptionResources.ReadOnlyCollectionNotSupported);
		}
		if (IsCompatibleObject(value))
		{
			Remove((T)value);
		}
	}

	protected void Execute(Action function)
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

	private TU Execute<TU>(Func<TU> function)
	{
		if ((dispatcher != null && dispatcher.CheckAccess()) || dispatcher == null)
		{
			return function();
		}
		dispatcher.EnqueueInvoke(function);
		return default(TU);
	}

	private static void IfNullAndNullsAreIllegalThenThrow<TX>(object value)
	{
		if (value == null && typeof(TX).IsValueType)
		{
			throw new ArgumentNullException("value");
		}
	}
}

