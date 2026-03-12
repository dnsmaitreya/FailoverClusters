using System;

namespace Microsoft.FailoverClusters.Framework;

public class ObservableCollectionFilter<T>
{
	public Func<T, bool> FilterFx { get; private set; }

	public string FilterQuery { get; private set; }

	public ObservableCollectionFilter(Func<T, bool> filterFx)
		: this(filterFx, (string)null)
	{
	}

	public ObservableCollectionFilter(Func<T, bool> filterFx, string filterQuery)
	{
		FilterFx = filterFx;
		FilterQuery = filterQuery;
	}
}
