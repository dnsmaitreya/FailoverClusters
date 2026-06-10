using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace MS.Internal.ServerClusters;

internal class AsyncEnumeration<T>
{
	private CreateItemFunction<T> m_createItem;

	private SafeEnumHandleBase m_enumHandle;

	private AsyncEnumerationCallback<T> m_callback;

	private AsyncEnumerationStatus m_status;

	private bool m_dontSort;

	private ICollection<T> m_existingItems;

	private List<T> m_enumeratedItems;

	private EventHandler _003Cbacking_store_003EEnumerationComplete;

	internal ICollection<T> EnumeratedItems => m_enumeratedItems;

	[SpecialName]
	internal event EventHandler EnumerationComplete
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003EEnumerationComplete = (EventHandler)Delegate.Combine(_003Cbacking_store_003EEnumerationComplete, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003EEnumerationComplete = (EventHandler)Delegate.Remove(_003Cbacking_store_003EEnumerationComplete, value);
		}
	}

	[SpecialName]
	private protected void raise_EnumerationComplete(object value0, EventArgs value1)
	{
		_003Cbacking_store_003EEnumerationComplete?.Invoke(value0, value1);
	}

	internal AsyncEnumeration(ICollection<T> existingCollection)
	{
		m_existingItems = existingCollection;
		m_enumeratedItems = new List<T>();
		m_createItem = null;
		m_enumHandle = null;
		m_status = null;
		m_dontSort = false;
	}

	internal AsyncEnumeration(CreateItemFunction<T> createItem, SafeEnumHandleBase enumHandle)
	{
		Construct(createItem, enumHandle, dontSort: false);
	}

	internal AsyncEnumeration(CreateItemFunction<T> createItem, SafeEnumHandleBase enumHandle, [MarshalAs(UnmanagedType.U1)] bool dontSort)
	{
		Construct(createItem, enumHandle, dontSort);
	}

	internal void SetCallback(AsyncEnumerationCallback<T> callback)
	{
		m_callback = callback;
	}

	internal AsyncEnumerationStatus StartEnumeration([MarshalAs(UnmanagedType.U1)] bool useDifferentThread)
	{
		m_status = new AsyncEnumerationStatus();
		if (useDifferentThread)
		{
			ThreadPool.QueueUserWorkItem(BackgroundEnumeration, null);
		}
		else
		{
			BackgroundEnumeration(null);
		}
		return m_status;
	}

	private void Construct(CreateItemFunction<T> createItem, SafeEnumHandleBase enumHandle, [MarshalAs(UnmanagedType.U1)] bool dontSort)
	{
		m_createItem = createItem;
		m_enumHandle = enumHandle;
		m_status = null;
		m_dontSort = dontSort;
		m_enumeratedItems = new List<T>();
	}

	private void BackgroundEnumeration(object data)
	{
		try
		{
			if (m_enumHandle == null)
			{
				BackgroundCollectionEnumeration(data);
			}
			else
			{
				BackgroundApiEnumeration(data);
			}
		}
		catch (ThreadAbortException)
		{
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "An exception occurred in a background thread.");
		}
	}

	private void BackgroundApiEnumeration(object data)
	{
		//Discarded unreachable code: IL_0146
		try
		{
			m_enumHandle.Reset();
			m_status.ThrowIfCanceled();
			long count = m_enumHandle.GetCount();
			long num = 0L;
			if (count == 0L)
			{
				ReportItem(0L, 0L, default(T));
				CompleteEnumeration(null);
				return;
			}
			m_status.ThrowIfCanceled();
			List<ClusterEnumItem> list = new List<ClusterEnumItem>();
			while (m_enumHandle.MoveNext())
			{
				m_status.ThrowIfCanceled();
				ClusterEnumItem clusterEnumItem = (ClusterEnumItem)m_enumHandle.Current;
				if (clusterEnumItem != null)
				{
					list.Add(clusterEnumItem);
				}
			}
			count = list.Count;
			if (!m_dontSort)
			{
				list.Sort();
			}
			List<ClusterEnumItem>.Enumerator enumerator = list.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ClusterEnumItem current = enumerator.Current;
				m_status.ThrowIfCanceled();
				T item;
				try
				{
					item = m_createItem(current.Name, current.ID);
				}
				catch (Exception ex)
				{
					Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(ex);
					if (firstException != null && (firstException.NativeErrorCode == -2147024894 || firstException.NativeErrorCode == -2147019890 || firstException.NativeErrorCode == -2147019889 || firstException.NativeErrorCode == -2147019854))
					{
						DebugLog.LogException(ex, "An object was not found in the enumeration, it will be skipped");
						continue;
					}
					throw;
				}
				m_status.ThrowIfCanceled();
				num++;
				ReportItem(num, count, item);
			}
			CompleteEnumeration(null);
		}
		catch (Exception ex2)
		{
			ExceptionHelp.HandleSpecialExceptions(ex2);
			m_callback(new AsyncEnumerationUpdate<T>(ex2));
			CompleteEnumeration(ex2);
		}
	}

	private void BackgroundCollectionEnumeration(object data)
	{
		try
		{
			long num = m_existingItems.Count;
			if (num == 0L)
			{
				ReportItem(0L, 0L, default(T));
			}
			else
			{
				long num2 = 1L;
				IEnumerator<T> enumerator = m_existingItems.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						T current = enumerator.Current;
						long currentItemNumber = num2;
						num2++;
						ReportItem(currentItemNumber, num, current);
					}
				}
				finally
				{
					IEnumerator<T> enumerator2 = enumerator;
					IDisposable disposable = enumerator;
					enumerator?.Dispose();
				}
			}
			CompleteEnumeration(null);
		}
		catch (Exception ex)
		{
			ExceptionHelp.HandleSpecialExceptions(ex);
			m_callback(new AsyncEnumerationUpdate<T>(ex));
			CompleteEnumeration(ex);
		}
	}

	private void ReportItem(long currentItemNumber, long numTotalItems, T item)
	{
		m_callback(new AsyncEnumerationUpdate<T>(currentItemNumber, numTotalItems, item));
		if (item != null)
		{
			m_enumeratedItems.Add(item);
		}
	}

	private void CompleteEnumeration(Exception e)
	{
		m_status.MarkAsFinished(e);
		if (e != null)
		{
			m_enumeratedItems = null;
		}
		OnEnumerationComplete();
	}

	private void OnEnumerationComplete()
	{
		EventArgs empty = EventArgs.Empty;
		_003Cbacking_store_003EEnumerationComplete?.Invoke(this, empty);
	}
}
