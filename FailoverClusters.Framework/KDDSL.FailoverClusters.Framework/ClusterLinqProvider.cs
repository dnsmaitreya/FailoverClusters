using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class ClusterLinqProvider : IClusterLinqProvider
{
	private readonly Cluster cluster;

	public ClusterLinqProvider(Cluster cluster)
	{
		this.cluster = cluster;
	}

	public IEnumerable<TResult> Execute<TResult>(ClusterList<TResult> clusterLinqQuery, OperationType operationType) where TResult : ClusterObject
	{
		if (clusterLinqQuery == null || clusterLinqQuery.QueryInfo == null)
		{
			return new List<TResult>();
		}
		if (operationType == OperationType.Async)
		{
			cluster.ExecuteMethod(delegate(ILockable lockObject)
			{
				IEnumerable<PClusterObject> privateClusterObjects2 = ((PCluster)lockObject.Owner).Select(clusterLinqQuery.QueryInfo);
				foreach (TResult item in CreateObjects(clusterLinqQuery, privateClusterObjects2, OperationType.Async))
				{
					_ = item;
				}
			}, OperationType.Async, delegate(OperationResult operationResult)
			{
				if (operationResult.Error != null)
				{
					clusterLinqQuery.LoadFinish(operationResult.Error);
				}
			}, LockAccess.Reader);
			return new List<TResult>();
		}
		IEnumerable<PClusterObject> privateClusterObjects = cluster.ExecuteMethod((ILockable lockObject) => ((PCluster)lockObject.Owner).Select(clusterLinqQuery.QueryInfo), LockAccess.Reader);
		return CreateObjects(clusterLinqQuery, privateClusterObjects, OperationType.Sync);
	}

	public TResult Execute<TResult>(ClusterList<TResult> clusterLinqQuery, Expression query, OperationType operationType) where TResult : ClusterObject
	{
		QueryTranslator queryTranslator = new QueryTranslator();
		QueryInfo queryInfo = queryTranslator.Translate(query);
		TResult result = null;
		if (!clusterLinqQuery.IsLoaded)
		{
			return cluster.ExecuteMethod(delegate(ILockable lockObject)
			{
				IEnumerable<PClusterObject> enumerable = null;
				enumerable = ((PCluster)lockObject.Owner).Select(queryInfo);
				TResult result2 = null;
				using (IEnumerator<TResult> enumerator2 = CreateObjects(clusterLinqQuery, enumerable, OperationType.Sync).GetEnumerator())
				{
					if (enumerator2.MoveNext())
					{
						result2 = enumerator2.Current;
						return result2;
					}
				}
				return result2;
			}, LockAccess.Reader);
		}
		Delegate whereLambdaExpressionFx = clusterLinqQuery.QueryInfo.WhereLambdaExpressionFx;
		foreach (TResult item in clusterLinqQuery)
		{
			if ((object)whereLambdaExpressionFx == null || (bool)whereLambdaExpressionFx.DynamicInvoke(item))
			{
				result = item;
				return result;
			}
		}
		return result;
	}

	private IEnumerable<TResult> CreateObjects<TResult>(ClusterList<TResult> clusterLinqQuery, IEnumerable<PClusterObject> privateClusterObjects, OperationType operationType) where TResult : ClusterObject
	{
		Delegate filterFx = clusterLinqQuery.QueryInfo.WhereLambdaExpressionFx;
		int expectedCount = 0;
		foreach (PClusterObject privateClusterObject in privateClusterObjects)
		{
			using ILockable clusterLock = cluster.GetLock(LockAccess.Reader);
			if (clusterLock == null)
			{
				throw new ClusterObjectNotFoundException(cluster.Name, cluster.Id, cluster.GetType());
			}
			ClusterObject clusterObject = null;
			using (ILockable lockable = ((PCluster)clusterLock.Owner).CacheManager.Get(privateClusterObject.Id, privateClusterObject.IdentityType, LockAccess.Reader))
			{
				if (lockable == null)
				{
					continue;
				}
				clusterObject = ((IPClusterObject<TResult>)lockable.Owner).GetProxy();
				if ((object)filterFx != null && !(bool)filterFx.DynamicInvoke(clusterObject))
				{
					continue;
				}
				expectedCount++;
				if (operationType == OperationType.Async)
				{
					clusterLinqQuery.AddInternal((TResult)clusterObject);
				}
				goto IL_0199;
			}
			IL_0199:
			if (operationType == OperationType.Sync)
			{
				yield return (TResult)clusterObject;
			}
		}
		clusterLinqQuery.WaitFor((TResult _) => clusterLinqQuery.Count >= expectedCount);
		clusterLinqQuery.LoadFinish(null);
	}

	public string GetQueryText(Expression query)
	{
		return new QueryTranslator().Translate(query).QueryText;
	}
}

