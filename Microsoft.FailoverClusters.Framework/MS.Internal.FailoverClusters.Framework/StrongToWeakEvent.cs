using System;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class StrongToWeakEvent<TEventArgs> : IDisposable where TEventArgs : EventArgs
{
	private EventHandler<TEventArgs> replacedHandler;

	private WeakReferenceEx targetDelegate;

	public StrongToWeakEvent(EventHandler<TEventArgs> strongEventHandler)
	{
		PerformanceCounters.Increment("Strong To Weak Events");
		targetDelegate = new WeakReferenceEx(strongEventHandler);
		replacedHandler = TargetMethod;
	}

	public void Dispose()
	{
		targetDelegate = null;
		replacedHandler = null;
	}

	public static implicit operator EventHandler<TEventArgs>(StrongToWeakEvent<TEventArgs> weakEventHandler)
	{
		return weakEventHandler.replacedHandler;
	}

	private void TargetMethod(object sender, TEventArgs e)
	{
		if (targetDelegate != null)
		{
			if (targetDelegate.Target is EventHandler<TEventArgs> eventHandler)
			{
				eventHandler(sender, e);
			}
			else
			{
				targetDelegate = null;
			}
		}
	}
}
