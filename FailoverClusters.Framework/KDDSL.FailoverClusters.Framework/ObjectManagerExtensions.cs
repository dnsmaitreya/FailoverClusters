using System;
using System.Collections.Generic;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace KDDSL.FailoverClusters.Framework;

internal static class ObjectManagerExtensions
{
	public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
	{
		foreach (T item in list)
		{
			action(item);
		}
	}

	internal static void CreateObject<T>(this T clusterObject, Guid id) where T : ClusterObject
	{
		Cluster cluster = clusterObject as Cluster;
		if (cluster != null)
		{
			PCluster pCluster = new PCluster(id);
			CacheManager.AddCluster(pCluster);
			cluster.CacheId = pCluster.CacheId;
			cluster.SetIdInternal(pCluster.Id);
			cluster.SubscribeToEvents(pCluster);
			return;
		}
		using ILockable lockable = clusterObject.Cluster.GetLock(LockAccess.Reader);
		if (lockable == null)
		{
			throw new ClusterObjectNotFoundException(clusterObject.Cluster.Name, clusterObject.Cluster.Id, clusterObject.GetType());
		}
		PClusterObject pClusterObject = null;
		if (clusterObject is Group)
		{
			pClusterObject = PGroup.Constructor((PCluster)lockable.Owner, Guid.NewGuid(), clusterObject.Name, ((Group)(object)clusterObject).GroupType);
		}
		else if (clusterObject is Resource)
		{
			pClusterObject = PResource.Constructor((PCluster)lockable.Owner, Guid.NewGuid(), clusterObject.Name, new PResourceType((PCluster)lockable.Owner, ((Resource)(object)clusterObject).ResourceType.ResourceKind));
		}
		else
		{
			object[] args = new object[2]
			{
				lockable.Owner,
				Guid.NewGuid()
			};
			pClusterObject = (PClusterObject)Activator.CreateInstance(clusterObject.OwnerType, args);
		}
		pClusterObject.Name = clusterObject.Name;
		if (pClusterObject is PGroup || pClusterObject is PResource || pClusterObject is PNode || pClusterObject is PResourceType || pClusterObject is PNetwork || pClusterObject is PNetworkInterface)
		{
			pClusterObject.Cluster.CacheManager.AddObject(pClusterObject);
			clusterObject.SetIdInternal(pClusterObject.Id);
			clusterObject.SubscribeToEvents(pClusterObject);
			return;
		}
		throw new NotImplementedException(string.Concat("The type ", clusterObject.GetType(), " is not implemented"));
	}

	internal static ILockable GetLock(this ClusterObject clusterObject, LockAccess lockAccess)
	{
		if (clusterObject is Cluster)
		{
			return CacheManager.GetCluster(((Cluster)clusterObject).CacheId, lockAccess);
		}
		return ((ClusterObject)clusterObject.Cluster).ExecuteMethod((Func<ILockable, ILockable>)((ILockable clusterLock) => ((PCluster)clusterLock.Owner).CacheManager?.Get(clusterObject, lockAccess)), LockAccess.Reader, setErrorOnObject: true);
	}

	internal static void ExecuteMethod(this ClusterObject clusterObject, Action<ILockable> function, OperationType operationType, LockAccess lockAccess, bool setErrorOnObject = true)
	{
		if (operationType == OperationType.Sync)
		{
			clusterObject.ExecuteMethod(function, lockAccess, setErrorOnObject);
		}
		else
		{
			clusterObject.ExecuteMethod(function, null, lockAccess, setErrorOnObject);
		}
	}

	internal static void ExecuteMethod(this ClusterObject clusterObject, Action<ILockable> function, OperationType operationType, Action<OperationResult> operationResultFunction, LockAccess lockAccess, bool setErrorOnObject = true)
	{
		if (operationType == OperationType.Sync)
		{
			clusterObject.ExecuteMethod(function, lockAccess, setErrorOnObject);
			operationResultFunction.SafeCall(new OperationResult(clusterObject, null));
		}
		else
		{
			clusterObject.ExecuteMethod(function, operationResultFunction, lockAccess, setErrorOnObject);
		}
	}

	internal static IEnumerable<TResult> ExecuteMethod<TResult>(this ClusterObject clusterObject, Func<ILockable, IEnumerable<TResult>> function, LockAccess lockAccess, bool setErrorOnObject = true)
	{
		try
		{
			clusterObject.SendError(clusterObject.Id, null, OperationType.Sync);
			return InternalFunction2(clusterObject, function, lockAccess);
		}
		catch (ClusterException ex)
		{
			if (setErrorOnObject)
			{
				clusterObject.SendError(clusterObject.Id, ex, OperationType.Sync);
			}
			throw;
		}
	}

	internal static void ExecuteMethod(this ClusterObject clusterObject, Action<ILockable> function, LockAccess lockAccess, bool setErrorOnObject = true)
	{
		try
		{
			if (setErrorOnObject)
			{
				clusterObject.SendError(clusterObject.Id, null, OperationType.Sync);
			}
			InternalFunctionV(clusterObject, function, lockAccess);
		}
		catch (ClusterException ex)
		{
			if (setErrorOnObject)
			{
				clusterObject.SendError(clusterObject.Id, ex, OperationType.Sync);
				return;
			}
			throw;
		}
	}

	internal static void ExecuteMethod<TResult>(this ClusterObject clusterObject, Func<ILockable, IEnumerable<TResult>> function, Action<OperationResult> operationResultFunction, LockAccess lockAccess, bool setErrorOnObject = true)
	{
		if (ClusterObject.ExecuteSynchronous)
		{
			InternalFunction2(clusterObject, function, lockAccess);
			return;
		}
		Worker.Start(delegate
		{
			clusterObject.SendError(clusterObject.Id, null, OperationType.Sync);
			InternalFunction2(clusterObject, function, lockAccess);
			operationResultFunction.SafeCall(new OperationResult(clusterObject, null));
		}, delegate(ClusterException exception)
		{
			if (operationResultFunction != null)
			{
				operationResultFunction(new OperationResult(clusterObject, exception));
			}
			else if (setErrorOnObject)
			{
				clusterObject.SendError(clusterObject.Id, exception, OperationType.Async);
			}
		});
	}

	internal static void ExecuteMethod(this ClusterObject clusterObject, Action<ILockable> function, Action<OperationResult> operationResultFunction, LockAccess lockAccess, bool setErrorOnObject = true)
	{
		Func<ILockable, bool> functionWithOpResultProc = delegate(ILockable lockObject)
		{
			function(lockObject);
			return false;
		};
		clusterObject.ExecuteMethod(functionWithOpResultProc, operationResultFunction, lockAccess, setErrorOnObject);
	}

	internal static void ExecuteMethod(this ClusterObject clusterObject, Func<ILockable, bool> functionWithOpResultProc, Action<OperationResult> operationResultFunction, LockAccess lockAccess, bool setErrorOnObject = true)
	{
		bool operationResultWasCalled = false;
		Action<ILockable> function = delegate(ILockable lockObject)
		{
			operationResultWasCalled = functionWithOpResultProc(lockObject);
		};
		Func<ClusterException, bool> callOperationResult = delegate(ClusterException ex)
		{
			if (operationResultFunction != null)
			{
				if (!operationResultWasCalled)
				{
					operationResultFunction(new OperationResult(clusterObject, ex));
				}
				return true;
			}
			return false;
		};
		if (ClusterObject.ExecuteSynchronous)
		{
			try
			{
				InternalFunctionV(clusterObject, function, lockAccess);
				callOperationResult(null);
				return;
			}
			catch (ClusterException ex2)
			{
				if (!callOperationResult(ex2) && !Cluster.IgnorableError(ex2))
				{
					ClusterLog.LogException(ex2, "An error occurred when running a background operation");
					throw;
				}
				return;
			}
		}
		Worker.Start(delegate
		{
			if (operationResultFunction != null && setErrorOnObject)
			{
				clusterObject.SendError(clusterObject.Id, null, OperationType.Sync);
			}
			InternalFunctionV(clusterObject, function, lockAccess);
			callOperationResult(null);
		}, delegate(ClusterException exception)
		{
			if (!callOperationResult(exception) && setErrorOnObject)
			{
				clusterObject.SendError(clusterObject.Id, exception, OperationType.Async);
			}
		});
	}

	internal static void ExecuteLoadMethod(this ClusterObject clusterObject, Func<ILockable, ClusterLoadedEventArgs> function, Action<OperationResult<ClusterLoadedEventArgs>> operationResultFunction, LockAccess lockAccess)
	{
		Worker.Start(delegate
		{
			clusterObject.SendError(clusterObject.Id, null, OperationType.Sync);
			ILockable @lock = clusterObject.GetLock(lockAccess);
			ClusterLoadedEventArgs resultObject = null;
			try
			{
				if (@lock == null)
				{
					throw new ClusterObjectNotFoundException(clusterObject.Name, clusterObject.Id, clusterObject.GetType());
				}
				resultObject = function(@lock);
			}
			finally
			{
				if (@lock != null)
				{
					switch (lockAccess)
					{
					case LockAccess.Reader:
						@lock.UnlockReader();
						break;
					case LockAccess.UpgradableReader:
						@lock.UnlockUpgradeableReader();
						break;
					case LockAccess.Writer:
						@lock.UnlockWriter();
						break;
					}
				}
			}
			try
			{
				operationResultFunction.SafeCall(new OperationResult<ClusterLoadedEventArgs>(clusterObject, resultObject, null));
			}
			catch (Exception innerException)
			{
				clusterObject.SendError(clusterObject.Id, new ClusterDefaultException(innerException), OperationType.Sync);
			}
		}, delegate(ClusterException exception)
		{
			if (operationResultFunction != null)
			{
				operationResultFunction(new OperationResult<ClusterLoadedEventArgs>(clusterObject, null, exception));
			}
			else
			{
				clusterObject.SendError(clusterObject.Id, exception, OperationType.Async);
			}
		});
	}

	internal static TResult ExecuteMethod<TResult>(this ClusterObject clusterObject, Func<ILockable, TResult> function, LockAccess lockAccess, bool setErrorOnObject = true)
	{
		try
		{
			clusterObject.SendError(clusterObject.Id, null, OperationType.Sync);
			return InternalFunction(clusterObject, function, lockAccess);
		}
		catch (ClusterException ex)
		{
			if (setErrorOnObject)
			{
				clusterObject.SendError(clusterObject.Id, ex, OperationType.Sync);
			}
			throw;
		}
	}

	internal static void ExecuteMethod<TResult>(this ClusterObject clusterObject, Func<ILockable, TResult> function, Action<OperationResult<TResult>> operationResultFunction, LockAccess lockType, bool setErrorOnObject = true)
	{
		clusterObject.ExecuteMethod(ResultExecution.DoNotCare, function, operationResultFunction, lockType, setErrorOnObject);
	}

	internal static void ExecuteMethod<TResult>(this ClusterObject clusterObject, ResultExecution resultExecution, Func<ILockable, TResult> function, Action<OperationResult<TResult>> operationResultFunction, LockAccess lockAccess, bool setErrorOnObject = true)
	{
		if (ClusterObject.ExecuteSynchronous)
		{
			InternalFunction(clusterObject, function, lockAccess);
			return;
		}
		Worker.Start(delegate
		{
			clusterObject.SendError(clusterObject.Id, null, OperationType.Sync);
			TResult retValue = InternalFunction(clusterObject, function, lockAccess);
			if (operationResultFunction != null)
			{
				if (resultExecution == ResultExecution.OnDispatcher)
				{
					UIHelper.ExecuteOnDispatcher(delegate
					{
						operationResultFunction(new OperationResult<TResult>(clusterObject, retValue, null));
					}, OperationType.Sync);
				}
				else
				{
					operationResultFunction(new OperationResult<TResult>(clusterObject, retValue, null));
				}
			}
		}, delegate(ClusterException exception)
		{
			if (operationResultFunction != null)
			{
				if (resultExecution == ResultExecution.OnDispatcher)
				{
					UIHelper.ExecuteOnDispatcher(delegate
					{
						operationResultFunction(new OperationResult<TResult>(clusterObject, default(TResult), exception));
					}, OperationType.Sync);
				}
				else
				{
					operationResultFunction(new OperationResult<TResult>(clusterObject, default(TResult), exception));
				}
			}
			else if (setErrorOnObject)
			{
				clusterObject.SendError(clusterObject.Id, exception, OperationType.Async);
			}
		});
	}

	private static void InternalFunctionV(ClusterObject clusterObject, Action<ILockable> function, LockAccess lockAccess)
	{
		ILockable @lock = clusterObject.GetLock(lockAccess);
		try
		{
			if (@lock == null)
			{
				throw new ClusterObjectNotFoundException(clusterObject.Name, clusterObject.Id, clusterObject.GetType());
			}
			function(@lock);
		}
		finally
		{
			if (@lock != null)
			{
				switch (lockAccess)
				{
				case LockAccess.Reader:
					@lock.UnlockReader();
					break;
				case LockAccess.UpgradableReader:
					@lock.UnlockUpgradeableReader();
					break;
				case LockAccess.Writer:
					@lock.UnlockWriter();
					break;
				}
			}
		}
	}

	private static TResult InternalFunction<TResult>(ClusterObject clusterObject, Func<ILockable, TResult> function, LockAccess lockAccess)
	{
		ILockable @lock = clusterObject.GetLock(lockAccess);
		try
		{
			if (@lock == null)
			{
				throw new ClusterObjectNotFoundException(clusterObject.Name, clusterObject.Id, clusterObject.GetType());
			}
			return function(@lock);
		}
		finally
		{
			if (@lock != null)
			{
				switch (lockAccess)
				{
				case LockAccess.Reader:
					@lock.UnlockReader();
					break;
				case LockAccess.UpgradableReader:
					@lock.UnlockUpgradeableReader();
					break;
				case LockAccess.Writer:
					@lock.UnlockWriter();
					break;
				}
			}
		}
	}

	private static IEnumerable<TResult> InternalFunction2<TResult>(ClusterObject clusterObject, Func<ILockable, IEnumerable<TResult>> function, LockAccess lockAccess)
	{
		ILockable @lock = clusterObject.GetLock(lockAccess);
		try
		{
			if (@lock == null)
			{
				throw new ClusterObjectNotFoundException(clusterObject.Name, clusterObject.Id, typeof(TResult));
			}
			return function(@lock);
		}
		finally
		{
			if (@lock != null)
			{
				switch (lockAccess)
				{
				case LockAccess.Reader:
					@lock.UnlockReader();
					break;
				case LockAccess.UpgradableReader:
					@lock.UnlockUpgradeableReader();
					break;
				case LockAccess.Writer:
					@lock.UnlockWriter();
					break;
				}
			}
		}
	}
}

