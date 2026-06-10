using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace KDDSL.ServerClusters;

public abstract class ClusterItem : IDisposable
{
	private ulong m_notifyId;

	private static bool m_cachingDisabled = false;

	private bool isDisposed;

	internal ulong NotifyId
	{
		get
		{
			return m_notifyId;
		}
		set
		{
			m_notifyId = value;
		}
	}

	public static bool CachingDisabled
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_cachingDisabled;
		}
		[param: MarshalAs(UnmanagedType.U1)]
		set
		{
			m_cachingDisabled = value;
		}
	}

	public abstract bool IsDeleted
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get;
	}

	public abstract Guid Id { get; }

	public abstract string Name { get; }

	public string getName()
	{
		return Name;
	}

	public abstract ControlExecutor GetControlExecutor();

	public abstract PropertyCollection GetCommonProperties(PropertyCollectionSet propSet);

	public abstract PropertyCollection GetPrivateProperties(PropertyCollectionSet propSet);

	public abstract void Close();

	internal abstract void Refresh();

	private void _0021ClusterItem()
	{
		Close();
	}

	private void _007EClusterItem()
	{
		if (!isDisposed)
		{
			isDisposed = true;
		}
	}

	[HandleProcessCorruptedStateExceptions]
	protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
	{
		if (A_0)
		{
			_007EClusterItem();
			return;
		}
		try
		{
			_0021ClusterItem();
		}
		finally
		{
			base.Finalize();
		}
	}

	public virtual sealed void Dispose()
	{
		Dispose(A_0: true);
		GC.SuppressFinalize(this);
	}

	~ClusterItem()
	{
		Dispose(A_0: false);
	}
}
