using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace KDDSL.ServerClusters;

public class AsyncEnumerationStatus
{
	private volatile bool m_isCanceled;

	private volatile bool m_isFinished;

	private volatile ManualResetEvent m_finishedEvent;

	private volatile Exception m_error;

	private object m_finishLock;

	public bool IsCanceled
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_isCanceled;
		}
	}

	internal AsyncEnumerationStatus()
	{
		m_isCanceled = false;
		m_isFinished = false;
		m_finishLock = new object();
	}

	internal void ThrowIfCanceled()
	{
		if (m_isCanceled)
		{
			throw new OperationCanceledException();
		}
	}

	internal void MarkAsFinished(Exception exception)
	{
		Monitor.Enter(m_finishLock);
		try
		{
			m_error = exception;
			m_isFinished = true;
			if (m_finishedEvent != null)
			{
				m_finishedEvent.Set();
			}
		}
		finally
		{
			Monitor.Exit(m_finishLock);
		}
	}

	internal void WaitForFinish()
	{
		Monitor.Enter(m_finishLock);
		try
		{
			if (m_isFinished)
			{
				return;
			}
			m_finishedEvent = new ManualResetEvent(initialState: false);
		}
		finally
		{
			Monitor.Exit(m_finishLock);
		}
		m_finishedEvent.WaitOne();
	}

	internal void RethrowError()
	{
		if (m_error != null)
		{
			string[] args = new string[0];
			throw ExceptionHelp.Build<ApplicationException>(m_error, args);
		}
	}

	public void CancelEnumeration()
	{
		m_isCanceled = true;
	}
}
