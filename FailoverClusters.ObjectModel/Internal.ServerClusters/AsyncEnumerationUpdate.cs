using System;
using System.Runtime.InteropServices;

namespace MS.Internal.ServerClusters;

public class AsyncEnumerationUpdate<T>
{
	private T m_item;

	private Exception m_exception;

	private long m_numTotalItems;

	private long m_currentItemNumber;

	public bool IsComplete
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_currentItemNumber == m_numTotalItems;
		}
	}

	public long CurrentItemNumber => m_currentItemNumber;

	public long NumberOfTotalItems => m_numTotalItems;

	public Exception Error => m_exception;

	public T Item => m_item;

	internal AsyncEnumerationUpdate(Exception exception)
	{
		m_item = default(T);
		m_exception = exception;
		m_currentItemNumber = -1L;
		m_numTotalItems = -1L;
	}

	internal AsyncEnumerationUpdate(long currentItemNumber, long numTotalItems, T item)
	{
		m_item = item;
		m_exception = null;
		m_currentItemNumber = currentItemNumber;
		m_numTotalItems = numTotalItems;
	}
}
