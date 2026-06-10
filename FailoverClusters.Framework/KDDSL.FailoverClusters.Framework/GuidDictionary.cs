using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class GuidDictionary<T> : IDictionary<Guid, T>, ICollection<KeyValuePair<Guid, T>>, IEnumerable<KeyValuePair<Guid, T>>, IEnumerable
{
	private ConcurrentDictionary<Guid, T> internalDictionary = new ConcurrentDictionary<Guid, T>();

	private ConcurrentDictionary<Guid, T> refreshDictionary;

	public ICollection<Guid> Keys => internalDictionary.Keys;

	public ICollection<T> Values => internalDictionary.Values;

	public T this[Guid key]
	{
		get
		{
			if (refreshDictionary != null)
			{
				refreshDictionary.TryRemove(key, out var _);
			}
			if (!internalDictionary.TryGetValue(key, out var value2))
			{
				return default(T);
			}
			return value2;
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public T this[FailoverClusters.Framework.IIdentifiable value] => this[value.Id];

	public int Count => internalDictionary.Count;

	public bool IsReadOnly => false;

	public GuidDictionary()
	{
	}

	public GuidDictionary(IDictionary<Guid, T> dictionary)
	{
		internalDictionary = new ConcurrentDictionary<Guid, T>(dictionary);
	}

	public void StartRefresh()
	{
		refreshDictionary = new ConcurrentDictionary<Guid, T>(internalDictionary);
	}

	public void StopRefresh()
	{
		if (refreshDictionary != null)
		{
			T value;
			refreshDictionary.ForEach(delegate(KeyValuePair<Guid, T> item)
			{
				internalDictionary.TryRemove(item.Key, out value);
			});
		}
	}

	public void Add(Guid key, T value)
	{
		if (refreshDictionary != null)
		{
			refreshDictionary.TryRemove(key, out var _);
		}
		internalDictionary.TryAdd(key, value);
	}

	public bool ContainsKey(Guid key)
	{
		if (refreshDictionary != null)
		{
			refreshDictionary.TryRemove(key, out var _);
		}
		return internalDictionary.ContainsKey(key);
	}

	public bool Remove(Guid key)
	{
		T value;
		if (refreshDictionary != null)
		{
			refreshDictionary.TryRemove(key, out value);
		}
		return internalDictionary.TryRemove(key, out value);
	}

	public bool TryGetValue(Guid key, out T value)
	{
		if (refreshDictionary != null)
		{
			refreshDictionary.TryRemove(key, out var _);
		}
		return internalDictionary.TryGetValue(key, out value);
	}

	public void Add(KeyValuePair<Guid, T> item)
	{
		if (refreshDictionary != null)
		{
			refreshDictionary.TryRemove(item.Key, out var _);
		}
		internalDictionary.TryAdd(item.Key, item.Value);
	}

	public void Clear()
	{
		refreshDictionary = null;
		internalDictionary.Clear();
	}

	public bool Contains(KeyValuePair<Guid, T> item)
	{
		if (refreshDictionary != null)
		{
			refreshDictionary.TryRemove(item.Key, out var _);
		}
		return internalDictionary.Contains(item);
	}

	public void CopyTo(KeyValuePair<Guid, T>[] array, int arrayIndex)
	{
		throw new NotImplementedException();
	}

	public bool Remove(KeyValuePair<Guid, T> item)
	{
		T value;
		if (refreshDictionary != null)
		{
			refreshDictionary.TryRemove(item.Key, out value);
		}
		return internalDictionary.TryRemove(item.Key, out value);
	}

	public IEnumerator<KeyValuePair<Guid, T>> GetEnumerator()
	{
		return internalDictionary.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return internalDictionary.GetEnumerator();
	}

	public void TrimExcess()
	{
	}
}

