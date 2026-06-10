using System.Collections;
using System.Windows.Forms;

namespace FailoverClusters.SnapIn;

internal abstract class ColumnSorterBase<TObject> : IComparer where TObject : class
{
	private SortOrder SortOrder { get; set; }

	public ColumnSorterBase(SortOrder sortOrder)
	{
		SortOrder = sortOrder;
	}

	public int Compare(object x, object y)
	{
		ListViewItem listViewItem = x as ListViewItem;
		ListViewItem listViewItem2 = y as ListViewItem;
		if (listViewItem == null || listViewItem2 == null)
		{
			return 0;
		}
		TObject x2 = (TObject)listViewItem.Tag;
		TObject y2 = (TObject)listViewItem2.Tag;
		return Compare(x2, y2) * ((SortOrder == SortOrder.Ascending) ? 1 : (-1));
	}

	protected abstract int Compare(TObject x, TObject y);
}

