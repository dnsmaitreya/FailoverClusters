using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KDDSL.FailoverClusters.Framework;

internal class LockableListString<T> : IDictionary<string, T>, ICollection<KeyValuePair<string, T>>, IEnumerable<KeyValuePair<string, T>>, IEnumerable
{
	private Dictionary<string, T> internalDictionary = new Dictionary<string, T>();

	public ICollection<string> Keys => internalDictionary.Keys;

	public ICollection<T> Values => internalDictionary.Values;

	public T this[string key]
	{
		get
		{
			if (!internalDictionary.TryGetValue(key, out var value))
			{
				return default(T);
			}
			return value;
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public int Count => internalDictionary.Count;

	public bool IsReadOnly => false;

	public void Add(string key, T value)
	{
		internalDictionary.Add(key, value);
	}

	public bool ContainsKey(string key)
	{
		return internalDictionary.ContainsKey(key);
	}

	public bool Remove(string key)
	{
		return internalDictionary.Remove(key);
	}

	public bool TryGetValue(string key, out T value)
	{
		return internalDictionary.TryGetValue(key, out value);
	}

	public void Add(KeyValuePair<string, T> item)
	{
		internalDictionary.Add(item.Key, item.Value);
	}

	public void Clear()
	{
		internalDictionary.Clear();
	}

	public bool Contains(KeyValuePair<string, T> item)
	{
		return internalDictionary.Contains(item);
	}

	public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
	{
		throw new NotImplementedException();
	}

	public bool Remove(KeyValuePair<string, T> item)
	{
		return internalDictionary.Remove(item.Key);
	}

	public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
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
