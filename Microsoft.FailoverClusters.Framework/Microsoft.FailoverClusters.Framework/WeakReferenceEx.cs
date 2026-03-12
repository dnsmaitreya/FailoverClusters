using System;
using System.Collections.Concurrent;
using System.Runtime.Serialization;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class WeakReferenceEx : WeakReference
{
	private static readonly ConcurrentBag<WeakReferenceEx> DelayedReferences = new ConcurrentBag<WeakReferenceEx>();

	private object objectInGc;

	private static readonly object ObjectLock = new object();

	public override object Target
	{
		get
		{
			object target = base.Target;
			if (target is IDelayedWeakReference delayedWeakReference && delayedWeakReference.DelayGcCollection && objectInGc == null)
			{
				objectInGc = target;
				DelayedReferences.Add(this);
			}
			return target;
		}
		set
		{
			base.Target = value;
		}
	}

	public WeakReferenceEx(object target)
		: base(target)
	{
		PerformanceCounters.Increment("WeakReferenceEx");
		ProcessDelayedReferences(target);
	}

	public WeakReferenceEx(object target, bool trackResurrection)
		: base(target, trackResurrection)
	{
		PerformanceCounters.Increment("WeakReferenceEx");
		ProcessDelayedReferences(target);
	}

	protected WeakReferenceEx(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	private void ReleaseHoldReference()
	{
		objectInGc = null;
	}

	private void ProcessDelayedReferences(object target)
	{
		if (target != null && target is IDelayedWeakReference delayedWeakReference && delayedWeakReference.DelayGcCollection)
		{
			objectInGc = target;
			DelayedReferences.Add(this);
		}
	}

	internal static void Collect()
	{
		lock (DelayedReferences)
		{
			WeakReferenceEx result;
			while (DelayedReferences.TryTake(out result))
			{
				result.ReleaseHoldReference();
			}
		}
	}

	public static T ReturnInstance<T>(ref WeakReferenceEx weakReference, Func<T> createInstanceCallback) where T : class
	{
		T val = null;
		if (weakReference != null)
		{
			val = (T)weakReference.Target;
		}
		if (weakReference == null || val == null)
		{
			lock (ObjectLock)
			{
				if (weakReference == null || val == null)
				{
					val = createInstanceCallback();
					weakReference = new WeakReferenceEx(val);
				}
			}
		}
		return (T)weakReference.Target;
	}

	public static T ReturnInstance<T>(ref WeakReferenceEx<T> weakReference, Func<T> createInstanceCallback) where T : class
	{
		T val = null;
		if (weakReference != null)
		{
			val = weakReference.Target;
		}
		if (weakReference == null || val == null)
		{
			lock (ObjectLock)
			{
				if (weakReference == null || val == null)
				{
					val = createInstanceCallback();
					weakReference = new WeakReferenceEx<T>(val);
				}
			}
		}
		return weakReference.Target;
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
[Serializable]
public class WeakReferenceEx<T> : WeakReferenceEx where T : class
{
	public new T Target
	{
		get
		{
			return (T)base.Target;
		}
		set
		{
			base.Target = value;
		}
	}

	public WeakReferenceEx(T target)
		: base(target)
	{
	}

	public WeakReferenceEx(T target, bool trackResurrection)
		: base(target, trackResurrection)
	{
	}

	protected WeakReferenceEx(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
