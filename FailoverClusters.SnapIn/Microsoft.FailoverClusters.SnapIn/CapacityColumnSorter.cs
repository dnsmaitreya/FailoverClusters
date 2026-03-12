using System;
using System.Windows.Forms;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.SnapIn;

internal class CapacityColumnSorter<TObject> : ColumnSorterBase<TObject> where TObject : class
{
	private readonly Func<TObject, ulong> getSizeFunc;

	internal CapacityColumnSorter(SortOrder sortOrder, Func<TObject, ulong> getSizeFunc)
		: base(sortOrder)
	{
		Exceptions.ThrowIfNull((object)getSizeFunc, "getSizeFunc");
		this.getSizeFunc = getSizeFunc;
	}

	protected override int Compare(TObject x, TObject y)
	{
		if (x == null)
		{
			return 1;
		}
		if (y == null)
		{
			return -1;
		}
		ulong num = getSizeFunc(x);
		ulong num2 = getSizeFunc(y);
		int result = 0;
		if (num > num2)
		{
			result = 1;
		}
		else if (num < num2)
		{
			result = -1;
		}
		return result;
	}
}
