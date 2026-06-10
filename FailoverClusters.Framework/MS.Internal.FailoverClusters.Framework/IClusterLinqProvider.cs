using System.Collections.Generic;
using System.Linq.Expressions;
using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal interface IClusterLinqProvider
{
	IEnumerable<TResult> Execute<TResult>(ClusterList<TResult> clusterLinqQuery, OperationType operationType) where TResult : ClusterObject;

	TResult Execute<TResult>(ClusterList<TResult> clusterLinqQuery, Expression query, OperationType operationType) where TResult : ClusterObject;

	string GetQueryText(Expression query);
}

