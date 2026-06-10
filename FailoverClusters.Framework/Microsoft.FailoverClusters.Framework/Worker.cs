using System;
using System.Collections.Concurrent;
using System.Threading;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public static class Worker
{
	private static readonly ConcurrentQueue<Action<string>> ExclusiveWorkerQueue;

	private static readonly ManualResetEventSlim ExclusiveResetEvent;

	private static int threadsEnqueued;

	private static int threadsExecuting;

	static Worker()
	{
		ExclusiveWorkerQueue = new ConcurrentQueue<Action<string>>();
		ExclusiveResetEvent = new ManualResetEventSlim(initialState: false);
		ThreadPool.GetMaxThreads(out var _, out var _);
		ThreadPool.GetMinThreads(out var _, out var _);
		ThreadPool.SetMinThreads(50, 5);
		Start(delegate
		{
			ExclusiveWorkerStarted();
		});
	}

	public static bool Start(Action<string> callback)
	{
		return Start<object, ClusterException>(delegate(object param, string stackTrace)
		{
			callback(stackTrace);
		}, null, null);
	}

	public static bool Start(Action<string> callback, Action<ClusterException> onError)
	{
		return Worker.Start<object, ClusterException>((Action<object, string>)delegate(object param, string stackTrace)
		{
			callback(stackTrace);
		}, (object)null, onError);
	}

	public static bool Start<T>(Action<T, string> callback, T parameter, Action<ClusterException> onError)
	{
		return Worker.Start<T, ClusterException>(callback, parameter, onError);
	}

	public static bool Start<T, TException>(Action<T, string> callback, T parameter, Action<TException> onError) where TException : Exception
	{
		threadsEnqueued++;
		string stackTrace = Global.GetStackIfDebugEnabled();
		return ThreadPool.QueueUserWorkItem(delegate(object param)
		{
			threadsEnqueued--;
			threadsExecuting++;
			try
			{
				callback((T)param, stackTrace);
			}
			catch (TException val)
			{
				if (onError != null)
				{
					onError(val);
				}
				else if (!Cluster.IgnorableError(val))
				{
					ClusterLog.LogException(val, "An error occurred when running a background operation");
					throw;
				}
			}
			finally
			{
				threadsExecuting--;
				if (!string.IsNullOrWhiteSpace(Thread.CurrentThread.Name))
				{
					Global.WriteLineThreadTerminated();
				}
			}
		}, parameter);
	}

	public static void StartExclusive(Action<string> action)
	{
		ExclusiveWorkerQueue.Enqueue(action);
		ExclusiveResetEvent.Set();
	}

	private static void ExclusiveWorkerStarted()
	{
		Thread.CurrentThread.Name = "Exclusive worker queue";
		Thread.CurrentThread.IsBackground = true;
		while (!Global.IsProcessShuttingDown)
		{
			ExclusiveResetEvent.Wait(-1);
			if (!Global.IsProcessShuttingDown)
			{
				ExclusiveResetEvent.Reset();
				Action<string> result;
				while (ExclusiveWorkerQueue.TryDequeue(out result))
				{
					string stackIfDebugEnabled = Global.GetStackIfDebugEnabled();
					result(stackIfDebugEnabled);
				}
				continue;
			}
			break;
		}
	}

	public static void Shutdown()
	{
		ExclusiveResetEvent.Set();
	}
}

