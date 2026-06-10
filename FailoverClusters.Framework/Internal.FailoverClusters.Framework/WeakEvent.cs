using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace MS.Internal.FailoverClusters.Framework;

internal static class WeakEvent
{
	private class WeakEventCallback
	{
		private class MethodTarget
		{
			private readonly MethodInfo method;

			private readonly WeakReferenceEx target;

			public MethodInfo Method => method;

			[DebuggerNonUserCode]
			public object Target
			{
				get
				{
					if (target == null)
					{
						return null;
					}
					return target.Target;
				}
			}

			public bool IsAlive
			{
				get
				{
					if (target == null)
					{
						return false;
					}
					return target.IsAlive;
				}
			}

			public MethodTarget(object target, MethodInfo method)
			{
				if (target != null)
				{
					this.method = method;
					_ = target.GetType().DeclaringType;
					this.target = new WeakReferenceEx(target);
				}
			}
		}

		private MethodTarget method;

		private List<MethodTarget> methods;

		private readonly object lockWeakMethodRefs = new object();

		public WeakEventCallback()
		{
			PerformanceCounters.Increment("Weak Events");
		}

		public void Compact()
		{
			lock (lockWeakMethodRefs)
			{
				try
				{
					if (method != null)
					{
						object target = method.Target;
						if (target == null || (target is IDisposableEx && ((IDisposableEx)target).IsDisposed))
						{
							method = null;
						}
					}
					else
					{
						if (methods == null)
						{
							return;
						}
						for (int num = methods.Count - 1; num >= 0; num--)
						{
							object target2 = methods[num].Target;
							if (target2 == null || (target2 is IDisposableEx && ((IDisposableEx)target2).IsDisposed))
							{
								methods.RemoveAt(num);
							}
						}
						if (methods.Count == 1)
						{
							method = methods[0];
							methods = null;
						}
						else if (methods.Count == 0)
						{
							methods = null;
						}
					}
				}
				catch (ArgumentNullException)
				{
				}
			}
		}

		public void Callback(object sender, EventArgs args)
		{
			MethodTarget methodTarget = null;
			MethodTarget[] array = null;
			lock (lockWeakMethodRefs)
			{
				if (method != null)
				{
					methodTarget = method;
				}
				else if (methods != null)
				{
					array = methods.ToArray();
				}
			}
			if (methodTarget != null)
			{
				object target = methodTarget.Target;
				if (target != null && (!(target is IDisposableEx) || !((IDisposableEx)target).IsDisposed))
				{
					try
					{
						object[] parameters = new object[2] { sender, args };
						methodTarget.Method.Invoke(target, parameters);
						return;
					}
					catch (TargetInvocationException ex)
					{
						if (!(ex.InnerException is ObjectDisposedException))
						{
							ClusterLog.LogException(ex, "There was an error calling the weak event");
						}
						return;
					}
					catch (Exception exception)
					{
						ClusterLog.LogException(exception, "There was an error calling the weak event");
						return;
					}
				}
				if (sender is IWeakEventContainer weakEventContainer)
				{
					weakEventContainer.NeedCompactation = true;
				}
			}
			else
			{
				if (array == null)
				{
					return;
				}
				MethodTarget[] array2 = array;
				foreach (MethodTarget methodTarget2 in array2)
				{
					object target2 = methodTarget2.Target;
					if (target2 == null || (target2 is IDisposableEx && ((IDisposableEx)target2).IsDisposed))
					{
						if (sender is IWeakEventContainer weakEventContainer2)
						{
							weakEventContainer2.NeedCompactation = true;
						}
						continue;
					}
					try
					{
						object[] parameters2 = new object[2] { sender, args };
						methodTarget2.Method.Invoke(target2, parameters2);
					}
					catch (TargetInvocationException ex2)
					{
						if (!(ex2.InnerException is ObjectDisposedException))
						{
							ClusterLog.LogException(ex2, "There was an error calling the weak event");
						}
					}
					catch (Exception exception2)
					{
						ClusterLog.LogException(exception2, "There was an error calling the weak event");
					}
				}
			}
		}

		public void Add(object target, MethodInfo targetMethod)
		{
			lock (lockWeakMethodRefs)
			{
				if (method == null && methods == null)
				{
					method = new MethodTarget(target, targetMethod);
					return;
				}
				if (methods == null)
				{
					methods = new List<MethodTarget>(2);
					methods.Add(method);
					method = null;
				}
				methods.Add(new MethodTarget(target, targetMethod));
			}
		}

		public void Remove(object target, MethodInfo targetMethod)
		{
			lock (lockWeakMethodRefs)
			{
				if (method != null)
				{
					if (method.Target == target && method.Method == targetMethod)
					{
						method = null;
					}
				}
				else
				{
					if (methods == null)
					{
						return;
					}
					for (int num = methods.Count - 1; num >= 0; num--)
					{
						MethodTarget methodTarget = methods[num];
						if (methodTarget.Target == target && methodTarget.Method == targetMethod)
						{
							methods.RemoveAt(num);
							break;
						}
					}
					if (methods.Count == 1)
					{
						method = methods[0];
						methods = null;
					}
					else if (methods.Count == 0)
					{
						methods = null;
					}
				}
			}
		}
	}

	public static WeakEventHandler Create()
	{
		return new WeakEventCallback().Callback;
	}

	public static void Add(this WeakEventHandler weakHandler, EventHandler handler)
	{
		if (!(weakHandler.Target is WeakEventCallback))
		{
			throw new ArgumentException("WeakEventHandler must be created using 'WeakEvent.Create();'");
		}
		((WeakEventCallback)weakHandler.Target).Add(handler.Target, handler.Method);
	}

	public static void Remove(this WeakEventHandler weakHandler, EventHandler handler)
	{
		if (!(weakHandler.Target is WeakEventCallback))
		{
			throw new ArgumentException("WeakEventHandler must be created using 'WeakEvent.Create();'");
		}
		((WeakEventCallback)weakHandler.Target).Remove(handler.Target, handler.Method);
	}

	public static WeakEventHandler<T> Create<T>() where T : EventArgs
	{
		return new WeakEventCallback().Callback;
	}

	public static void Add<T>(this WeakEventHandler<T> weakHandler, EventHandler<T> handler) where T : EventArgs
	{
		if (!(weakHandler.Target is WeakEventCallback))
		{
			throw new ArgumentException("WeakEventHandler must be created using 'WeakEvent.Create();'");
		}
		if (handler.Target != null)
		{
			((WeakEventCallback)weakHandler.Target).Add(handler.Target, handler.Method);
		}
	}

	public static void Remove<T>(this WeakEventHandler<T> weakHandler, EventHandler<T> handler) where T : EventArgs
	{
		if (!(weakHandler.Target is WeakEventCallback))
		{
			throw new ArgumentException("WeakEventHandler must be created using 'WeakEvent.Create();'");
		}
		((WeakEventCallback)weakHandler.Target).Remove(handler.Target, handler.Method);
	}

	internal static void Compact(Delegate delegateToTarget)
	{
		if ((object)delegateToTarget != null && delegateToTarget.Target != null && delegateToTarget.Target is WeakEventCallback weakEventCallback)
		{
			weakEventCallback.Compact();
		}
	}
}

