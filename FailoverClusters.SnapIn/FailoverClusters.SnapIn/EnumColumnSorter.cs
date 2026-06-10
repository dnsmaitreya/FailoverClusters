using System;
using System.Windows.Forms;
using FailoverClusters.UI.Common;

namespace FailoverClusters.SnapIn;

internal class EnumColumnSorter<TObject> : ColumnSorterBase<TObject> where TObject : class
{
	private readonly Func<TObject, string> func;

	internal EnumColumnSorter(SortOrder sortOrder, Func<TObject, string> func)
		: base(sortOrder)
	{
		Exceptions.ThrowIfNull((object)func, "func");
		this.func = func;
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
		return string.Compare(func(x), func(y), StringComparison.CurrentCulture);
	}
}

