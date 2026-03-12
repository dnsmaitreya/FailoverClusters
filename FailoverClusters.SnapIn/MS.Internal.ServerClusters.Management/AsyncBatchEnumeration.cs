using System;
using System.Collections.Generic;
using System.Threading;

namespace MS.Internal.ServerClusters.Management;

internal class AsyncBatchEnumeration<T> : IDisposable
{
	private delegate void VoidParamDelegate();

	private List<T> batchedItems;

	private object lockObject = new object();

	private Timer updateTimer;

	private AsyncBatchReady<T> callback;

	private const int updateTimerInterval = 1500;

	public AsyncBatchEnumeration(AsyncBatchReady<T> callback)
	{
		this.callback = callback;
		batchedItems = new List<T>();
		updateTimer = new Timer(FlushBatchedItems, null, 1500, 1500);
	}

	public void Dispose()
	{
		updateTimer.Dispose();
		GC.SuppressFinalize(this);
	}

	public void AddItem(T item)
	{
		lock (lockObject)
		{
			batchedItems.Add(item);
		}
	}

	public void FlushBatchedItems(object data)
	{
		FlushBatchedItems();
	}

	public void FlushBatchedItems()
	{
		List<T> list;
		lock (lockObject)
		{
			list = batchedItems;
			batchedItems = new List<T>();
		}
		if (list.Count > 0)
		{
			callback(list);
		}
	}
}
