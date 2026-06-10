using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace MS.Internal.ServerClusters;

[DefaultMember("Item")]
public abstract class ClusterObjectCollectionBase<T> : ReadOnlyCollection<T> where T : ClusterItem
{
	private void EnumerationCallback(AsyncEnumerationUpdate<T> update)
	{
		if (update.Item != null)
		{
			InternalAdd(update.Item);
		}
	}

	public void InternalAdd(T item)
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		base.Items.Add(item);
	}

	internal void LoadFromEnum(AsyncEnumeration<T> asyncEnum)
	{
		AsyncEnumerationCallback<T> callback = EnumerationCallback;
		asyncEnum.SetCallback(callback);
		AsyncEnumerationStatus asyncEnumerationStatus = asyncEnum.StartEnumeration(useDifferentThread: false);
		asyncEnumerationStatus.WaitForFinish();
		asyncEnumerationStatus.RethrowError();
	}

	protected ClusterObjectCollectionBase()
		: base((IList<T>)new List<T>())
	{
	}
}
