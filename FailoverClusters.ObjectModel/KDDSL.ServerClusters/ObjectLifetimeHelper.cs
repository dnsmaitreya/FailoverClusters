using System.Runtime.InteropServices;
using System.Threading;

namespace KDDSL.ServerClusters;

internal class ObjectLifetimeHelper
{
	private bool m_isDisposed;

	private bool m_isDeleted;

	private object m_lockObject;

	internal bool IsDeleted
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_isDeleted;
		}
	}

	internal bool IsDisposed
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_isDisposed;
		}
	}

	internal ObjectLifetimeHelper()
	{
		m_isDisposed = false;
		m_isDeleted = false;
		m_lockObject = new object();
	}

	internal void CheckObjectState()
	{
		if (m_isDeleted)
		{
			ClusApiExceptionFactory.ThrowObjectDeletedException();
		}
		if (m_isDisposed)
		{
			ClusApiExceptionFactory.ThrowObjectDisposedException();
		}
	}

	internal void MarkAsDeleted()
	{
		m_isDeleted = true;
	}

	internal void MarkAsDisposed()
	{
		m_isDisposed = true;
	}

	internal void AquireDisposeLock()
	{
		Monitor.Enter(m_lockObject);
	}

	internal void ReleaseDisposeLock()
	{
		Monitor.Exit(m_lockObject);
	}
}
