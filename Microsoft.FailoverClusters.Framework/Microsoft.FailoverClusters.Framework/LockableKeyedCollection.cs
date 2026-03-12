using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
[ComVisible(false)]
[DebuggerDisplay("Count = {Count}")]
public abstract class LockableKeyedCollection<TKey, TItem> : LockableCollection<TItem>
{
	private readonly IEqualityComparer<TKey> comparer;

	private Dictionary<TKey, TItem> dict;

	private int keyCount;

	private readonly int threshold;

	public IEqualityComparer<TKey> Comparer => comparer;

	protected IDictionary<TKey, TItem> Dictionary => dict;

	public TItem this[TKey key]
	{
		get
		{
			if (key.Equals(default(TKey)))
			{
				throw new ArgumentNullException("key");
			}
			if (dict != null)
			{
				return dict[key];
			}
			foreach (TItem item in base.Items)
			{
				if (comparer.Equals(GetKeyForItem(item), key))
				{
					return item;
				}
			}
			throw new KeyNotFoundException();
		}
	}

	protected LockableKeyedCollection(bool realtime, Dispatcher dispatcher)
		: this((IEqualityComparer<TKey>)null, 0, realtime, dispatcher)
	{
	}

	protected LockableKeyedCollection(IEqualityComparer<TKey> comparer, bool realTime, Dispatcher dispatcher)
		: this(comparer, 0, realTime, dispatcher)
	{
	}

	protected LockableKeyedCollection(IEqualityComparer<TKey> comparer, int dictionaryCreationThreshold, bool realTime, Dispatcher dispatcher)
		: base(realTime, dispatcher)
	{
		if (comparer == null)
		{
			comparer = EqualityComparer<TKey>.Default;
		}
		if (dictionaryCreationThreshold == -1)
		{
			dictionaryCreationThreshold = int.MaxValue;
		}
		if (dictionaryCreationThreshold < -1)
		{
			throw new ArgumentOutOfRangeException("dictionaryCreationThreshold");
		}
		this.comparer = comparer;
		threshold = dictionaryCreationThreshold;
	}

	public new virtual void TrimExcess()
	{
		base.TrimExcess();
	}

	private void AddKey(TKey key, TItem item)
	{
		if (dict != null)
		{
			dict.Add(key, item);
			return;
		}
		if (keyCount == threshold)
		{
			CreateDictionary().Add(key, item);
			return;
		}
		if (Contains(key))
		{
			throw new ArgumentException(ExceptionResources.KeyAlreadyExists);
		}
		keyCount++;
	}

	protected void ChangeItemKey(TItem item, TKey newKey)
	{
		if (!ContainsItem(item))
		{
			throw new ArgumentException("Item doesnt exist");
		}
		TKey keyForItem = GetKeyForItem(item);
		if (!comparer.Equals(keyForItem, newKey))
		{
			if (!newKey.Equals(default(TKey)))
			{
				AddKey(newKey, item);
			}
			if (!keyForItem.Equals(default(TKey)))
			{
				RemoveKey(keyForItem);
			}
		}
	}

	protected override void ClearItems()
	{
		base.ClearItems();
		if (dict != null)
		{
			dict.Clear();
		}
		keyCount = 0;
	}

	public bool Contains(TKey key)
	{
		if (key.Equals(default(TKey)))
		{
			throw new ArgumentNullException("key");
		}
		if (dict != null)
		{
			return dict.ContainsKey(key);
		}
		if (!key.Equals(default(TKey)))
		{
			return base.Items.Any((TItem local) => comparer.Equals(GetKeyForItem(local), key));
		}
		return false;
	}

	private bool ContainsItem(TItem item)
	{
		if (dict != null)
		{
			TKey keyForItem;
			TKey val = (keyForItem = GetKeyForItem(item));
			if (!val.Equals(default(TItem)))
			{
				if (dict.TryGetValue(keyForItem, out var value))
				{
					return EqualityComparer<TItem>.Default.Equals(value, item);
				}
				return false;
			}
		}
		return base.Items.Contains(item);
	}

	private Dictionary<TKey, TItem> CreateDictionary()
	{
		dict = new Dictionary<TKey, TItem>(comparer);
		foreach (TItem item in base.Items)
		{
			TKey keyForItem = GetKeyForItem(item);
			if (!keyForItem.Equals(default(TKey)))
			{
				dict.Add(keyForItem, item);
			}
		}
		return dict;
	}

	protected abstract TKey GetKeyForItem(TItem item);

	protected override void InsertItem(int index, TItem item)
	{
		TKey keyForItem = GetKeyForItem(item);
		if (!keyForItem.Equals(default(TKey)))
		{
			AddKey(keyForItem, item);
		}
		base.InsertItem(index, item);
	}

	protected override void AddRange(List<TItem> items)
	{
		List<TItem> list = new List<TItem>();
		foreach (TItem item in items)
		{
			TKey keyForItem = GetKeyForItem(item);
			if (!keyForItem.Equals(default(TKey)) && !Contains(keyForItem))
			{
				AddKey(keyForItem, item);
				list.Add(item);
			}
		}
		base.AddRange(list);
	}

	public bool Remove(TKey key)
	{
		if (key.Equals(default(TKey)))
		{
			throw new ArgumentNullException("key");
		}
		if (dict != null)
		{
			if (dict.ContainsKey(key))
			{
				return Remove(dict[key]);
			}
			return false;
		}
		if (!key.Equals(default(TKey)))
		{
			for (int i = 0; i < base.Items.Count; i++)
			{
				if (comparer.Equals(GetKeyForItem(base.Items[i]), key))
				{
					RemoveItem(i);
					return true;
				}
			}
		}
		return false;
	}

	protected override void RemoveItem(int index)
	{
		TKey keyForItem = GetKeyForItem(base.Items[index]);
		if (!keyForItem.Equals(default(TKey)))
		{
			RemoveKey(keyForItem);
		}
		base.RemoveItem(index);
	}

	private void RemoveKey(TKey key)
	{
		if (dict != null)
		{
			dict.Remove(key);
		}
		else
		{
			keyCount--;
		}
	}

	protected override void SetItem(int index, TItem item)
	{
		TKey keyForItem = GetKeyForItem(item);
		TKey keyForItem2 = GetKeyForItem(base.Items[index]);
		if (comparer.Equals(keyForItem2, keyForItem))
		{
			if (!keyForItem.Equals(default(TKey)) && dict != null)
			{
				dict[keyForItem] = item;
			}
		}
		else
		{
			if (!keyForItem.Equals(default(TKey)))
			{
				AddKey(keyForItem, item);
			}
			if (!keyForItem2.Equals(default(TKey)))
			{
				RemoveKey(keyForItem2);
			}
		}
		base.SetItem(index, item);
	}

	protected bool TryGetValueInternal(TKey key, out TItem value)
	{
		if (dict != null)
		{
			return dict.TryGetValue(key, out value);
		}
		foreach (TItem item in base.Items)
		{
			if (comparer.Equals(GetKeyForItem(item), key))
			{
				value = item;
				return true;
			}
		}
		value = default(TItem);
		return false;
	}
}
