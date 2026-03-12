using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UI.Common;

namespace MS.Internal.FailoverClusters.Framework;

internal class ClusterObjectLoader
{
	private const int CurrentLimitAsyncOperations = 4;

	private const int MiniQueueLimitOperations = 40;

	private const int MiniQueueEnqueuedTimeOut = 100;

	private readonly Stack<Queue<ClusterObjectLoaderParam>> stack = new Stack<Queue<ClusterObjectLoaderParam>>();

	private readonly object queueLock = new object();

	private readonly Stopwatch enqueueTime = new Stopwatch();

	private readonly Semaphore currentOperations = new Semaphore(4, 4);

	private readonly AutoResetEvent loading = new AutoResetEvent(initialState: false);

	private Queue<ClusterObjectLoaderParam> queue;

	private ClusterObject lastClusterObject;

	private int lastDataSelection;

	private bool unloadLoader = true;

	public bool UnloadLoader => unloadLoader;

	public ClusterObjectLoader(string clusterName)
	{
		Load(clusterName);
	}

	~ClusterObjectLoader()
	{
		currentOperations.Dispose();
		loading.Dispose();
	}

	private void Load(string clusterName)
	{
		if (unloadLoader)
		{
			unloadLoader = false;
			Worker.Start(delegate
			{
				RunLoader(clusterName);
			});
		}
	}

	public void Unload()
	{
		unloadLoader = true;
		loading.Set();
	}

	public void EnqueueClusterObject(ClusterObjectLoaderParam clusterObjectParam)
	{
		if (clusterObjectParam.ClusterObject == lastClusterObject && (clusterObjectParam.LoadSelection & lastDataSelection) == clusterObjectParam.LoadSelection && (clusterObjectParam.ClusterObject.LoadSelection & clusterObjectParam.LoadSelection) == clusterObjectParam.LoadSelection)
		{
			ClusterLoadedEventArgs operationResult = new ClusterLoadedEventArgs(clusterObjectParam.ClusterObject, loaded: true, clusterObjectParam.LoadSelection, null);
			try
			{
				if (clusterObjectParam.BackgroundCallback != null)
				{
					clusterObjectParam.BackgroundCallback.SafeCall(operationResult);
				}
				return;
			}
			catch (Exception innerException)
			{
				clusterObjectParam.ClusterObject.Error = new ClusterDefaultException(innerException);
				return;
			}
		}
		lastClusterObject = clusterObjectParam.ClusterObject;
		lastDataSelection = clusterObjectParam.LoadSelection;
		if ((lastDataSelection & 0x20000000) == 536870912)
		{
			lastDataSelection ^= 536870912;
		}
		lock (queueLock)
		{
			if (!enqueueTime.IsRunning)
			{
				enqueueTime.Start();
				queue = new Queue<ClusterObjectLoaderParam>();
				queue.Enqueue(clusterObjectParam);
			}
			else if (enqueueTime.ElapsedMilliseconds < 100 && queue.Count < 40)
			{
				queue.Enqueue(clusterObjectParam);
			}
			else
			{
				stack.Push(queue);
				queue = null;
				enqueueTime.Reset();
				enqueueTime.Start();
				queue = new Queue<ClusterObjectLoaderParam>();
				queue.Enqueue(clusterObjectParam);
			}
		}
		loading.Set();
	}

	private void StealMiniQueue()
	{
		stack.Push(queue);
		queue = null;
		enqueueTime.Stop();
		enqueueTime.Reset();
	}

	private void ProcessMiniQueue(IEnumerable<ClusterObjectLoaderParam> miniQueue)
	{
		foreach (ClusterObjectLoaderParam item in miniQueue)
		{
			if (unloadLoader)
			{
				break;
			}
			currentOperations.WaitOne();
			ClusterObjectLoaderParam paramObject = item;
			if (paramObject == null)
			{
				continue;
			}
			Worker.Start(delegate
			{
				ILockable @lock = paramObject.ClusterObject.GetLock(LockAccess.UpgradableReader);
				LockAccess lockAccess = LockAccess.UpgradableReader;
				ClusterLoadedEventArgs clusterLoadedEventArgs = null;
				bool flag = false;
				try
				{
					if (@lock == null)
					{
						throw new ClusterObjectNotFoundException(paramObject.ClusterObject.Name, paramObject.ClusterObject.Id, paramObject.ClusterObject.GetType());
					}
					@lock.Writer();
					lockAccess = LockAccess.Writer;
					try
					{
						IPClusterObject<ClusterObject> iPClusterObject = (IPClusterObject<ClusterObject>)@lock.Owner;
						if ((iPClusterObject.LoadedSelection & paramObject.LoadSelection) != paramObject.LoadSelection && iPClusterObject.Cluster.IsOpen)
						{
							int loadSelection = (((paramObject.LoadSelection & 0x20000000) == 536870912) ? paramObject.LoadSelection : (paramObject.LoadSelection & ~iPClusterObject.LoadedSelection));
							clusterLoadedEventArgs = iPClusterObject.LoadObject(loadSelection);
							clusterLoadedEventArgs.Sender = paramObject.ClusterObject;
							if (paramObject.ClusterObject.BoundToPrivate && (paramObject.ClusterObject.LoadSelection & paramObject.LoadSelection) != paramObject.LoadSelection)
							{
								paramObject.ClusterObject.TransferInternalData(@lock.Owner, subscribeToEvents: false, ignorePossibleOwners: true);
							}
							@lock.UnlockWriter();
							lockAccess = LockAccess.UpgradableReader;
							@lock.Reader();
							@lock.UnlockUpgradeableReader();
							lockAccess = LockAccess.Reader;
							iPClusterObject.Cluster.RealtimeCollections.Change(iPClusterObject, "LoadSelection");
						}
						else
						{
							if (paramObject.ClusterObject.BoundToPrivate && (paramObject.ClusterObject.LoadSelection & paramObject.LoadSelection) != paramObject.LoadSelection)
							{
								paramObject.ClusterObject.TransferInternalData(@lock.Owner, subscribeToEvents: false, ignorePossibleOwners: true);
							}
							@lock.UnlockWriter();
							lockAccess = LockAccess.UpgradableReader;
							@lock.Reader();
							@lock.UnlockUpgradeableReader();
							lockAccess = LockAccess.Reader;
							clusterLoadedEventArgs = new ClusterLoadedEventArgs(paramObject.ClusterObject, loaded: true, @lock.Owner.LoadedSelection, null);
						}
					}
					finally
					{
						switch (lockAccess)
						{
						case LockAccess.Writer:
							@lock.UnlockWriter();
							@lock.UnlockUpgradeableReader();
							break;
						case LockAccess.UpgradableReader:
							@lock.UnlockUpgradeableReader();
							break;
						case LockAccess.Reader:
							@lock.UnlockReader();
							break;
						}
					}
				}
				catch (ClusterException exception2)
				{
					clusterLoadedEventArgs = new ClusterLoadedEventArgs(paramObject.ClusterObject, loaded: false, paramObject.LoadSelection, exception2);
				}
				catch (Exception)
				{
					flag = true;
					throw;
				}
				finally
				{
					try
					{
						currentOperations.Release();
					}
					catch (SemaphoreFullException exception3)
					{
						ClusterLog.LogException(exception3, "A semaphore exception was thrown on ClusterObjectLoader");
					}
					if (!flag)
					{
						try
						{
							if (paramObject.BackgroundCallback != null)
							{
								paramObject.BackgroundCallback.SafeCall(clusterLoadedEventArgs);
							}
						}
						catch (Exception innerException2)
						{
							paramObject.ClusterObject.Error = new ClusterDefaultException(innerException2);
						}
					}
				}
			}, delegate(ClusterException exception)
			{
				ClusterLoadedEventArgs operationResult = new ClusterLoadedEventArgs(paramObject.ClusterObject, loaded: false, paramObject.LoadSelection, exception);
				try
				{
					if (paramObject.BackgroundCallback != null)
					{
						paramObject.BackgroundCallback.SafeCall(operationResult);
					}
				}
				catch (Exception innerException)
				{
					paramObject.ClusterObject.Error = new ClusterDefaultException(innerException);
				}
			});
		}
	}

	private void RunLoader(string clusterName)
	{
		Thread.CurrentThread.Name = "Virtual Item Loader '{0}'".FormatCurrentCulture(clusterName);
		while (true)
		{
			loading.WaitOne();
			if (unloadLoader)
			{
				break;
			}
			while (queue != null || stack.Count > 0)
			{
				if (stack.Count == 0)
				{
					lock (queueLock)
					{
						StealMiniQueue();
					}
				}
				while (stack.Count > 0)
				{
					lock (queueLock)
					{
						if (queue != null && (enqueueTime.ElapsedMilliseconds > 50 || queue.Count > 20))
						{
							StealMiniQueue();
						}
					}
					Queue<ClusterObjectLoaderParam> miniQueue = stack.Pop();
					if (unloadLoader)
					{
						return;
					}
					ProcessMiniQueue(miniQueue);
				}
			}
		}
	}
}
