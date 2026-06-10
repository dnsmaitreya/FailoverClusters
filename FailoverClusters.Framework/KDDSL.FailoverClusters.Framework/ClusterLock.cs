using System;
using System.Threading;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class ClusterLock : ILockable, IDisposable
{
	private PClusterObject clusterObject;

	private ReaderWriterLockSlimFramework lockObject;

	public PClusterObject Owner
	{
		get
		{
			return clusterObject;
		}
		internal set
		{
			clusterObject = value;
		}
	}

	internal ReaderWriterLockSlimFramework LockObject
	{
		get
		{
			return lockObject;
		}
		set
		{
			lockObject = value;
		}
	}

	internal ClusterLock(PClusterObject clusterObject)
	{
		this.clusterObject = clusterObject;
		lockObject = new ReaderWriterLockSlimFramework(LockRecursionPolicy.SupportsRecursion);
		lockObject.LockType = typeof(PClusterObject);
	}

	public void Reader()
	{
		lockObject.EnterReadLock();
	}

	public bool IsReaderLockHeld()
	{
		return lockObject.IsReadLockHeld;
	}

	public bool IsWriteLockHeld()
	{
		return lockObject.IsWriteLockHeld;
	}

	public bool IsUpgradeableReadLockHeld()
	{
		return lockObject.IsUpgradeableReadLockHeld;
	}

	public void Writer()
	{
		lockObject.EnterWriteLock();
	}

	public void UpgradeableReader()
	{
		lockObject.EnterUpgradeableReadLock();
	}

	public void UnlockReader()
	{
		lockObject.ExitReadLock();
	}

	public void UnlockUpgradeableReader()
	{
		lockObject.ExitUpgradeableReadLock();
	}

	public void UnlockWriter()
	{
		lockObject.ExitWriteLock();
	}

	public void Dispose()
	{
		UnlockReader();
	}

	public override int GetHashCode()
	{
		if (lockObject == null)
		{
			return base.GetHashCode();
		}
		return lockObject.GetHashCode();
	}
}

