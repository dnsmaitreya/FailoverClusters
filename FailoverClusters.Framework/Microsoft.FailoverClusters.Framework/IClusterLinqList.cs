using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FailoverClusters.Framework;

public interface IClusterLinqList
{
	Type ElementType { get; }
}
public interface IClusterLinqList<out TResult> : IClusterLinqList, IOrderedQueryable<TResult>, IQueryable<TResult>, IEnumerable<TResult>, IEnumerable, IQueryable, IOrderedQueryable, IQueryProvider
{
}

