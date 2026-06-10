using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace FailoverClusters.Framework;

public interface IClusterList<T> : IClusterLinqList<T>, IClusterLinqList, IOrderedQueryable<T>, IQueryable<T>, IEnumerable<T>, IEnumerable, IQueryable, IOrderedQueryable, IQueryProvider, IClusterList, IDisposable, IList<T>, ICollection<T> where T : IClusterObject
{
	IClusterList<T> ForceLoadStart();

	void ExecuteQuery(Action<OperationResult<IClusterList<T>>> operationResult);

	void ExecuteQuery(Action<OperationResult<IClusterList<T>>> operationResult, object parameter);

	void ExecuteQuery(ResultExecution resultExecution, Action<OperationResult<IClusterList<T>>> operationResult);

	void ExecuteQuery(ResultExecution resultExecution, Action<OperationResult<IClusterList<T>>> operationResult, object parameter);
}
public interface IClusterList : IDisposable
{
	Cluster Cluster { get; }

	bool IsLoaded { get; }

	event EventHandler Loaded;

	event PropertyChangedEventHandler PropertyChanged;

	event NotifyCollectionChangedEventHandler CollectionChanged;

	void TrimExcess();
}

