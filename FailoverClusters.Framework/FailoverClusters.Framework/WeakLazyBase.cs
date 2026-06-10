namespace FailoverClusters.Framework;

public abstract class WeakLazyBase<T> where T : class
{
	private static readonly object SharedLockObject = new object();

	private WeakReferenceEx<T> weakReferenceToInstance;

	public T GetInstance()
	{
		lock (SharedLockObject)
		{
			if (weakReferenceToInstance == null)
			{
				return StartGenerateInstance();
			}
			T target = weakReferenceToInstance.Target;
			if (target != null)
			{
				return target;
			}
			return StartGenerateInstance();
		}
	}

	public void ReleaseReference()
	{
		lock (SharedLockObject)
		{
			weakReferenceToInstance = null;
		}
	}

	public T TryGetInstance()
	{
		lock (SharedLockObject)
		{
			return (weakReferenceToInstance != null) ? weakReferenceToInstance.Target : null;
		}
	}

	protected abstract T GenerateInstance();

	protected virtual void DoPostGenerateInstance(T newValue)
	{
	}

	protected virtual void DoPreGenerateInstance()
	{
	}

	private T StartGenerateInstance()
	{
		DoPreGenerateInstance();
		T val = GenerateInstance();
		weakReferenceToInstance = new WeakReferenceEx<T>(val);
		DoPostGenerateInstance(val);
		return val;
	}

	public static implicit operator T(WeakLazyBase<T> weakLazyInstance)
	{
		return weakLazyInstance.ToInstance();
	}

	private T ToInstance()
	{
		return GetInstance();
	}
}

