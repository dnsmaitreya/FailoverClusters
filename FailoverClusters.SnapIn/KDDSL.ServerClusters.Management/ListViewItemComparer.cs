using System.Collections.Generic;
using System.Windows.Forms;

namespace KDDSL.ServerClusters.Management;

internal class ListViewItemComparer : IComparer<EventListViewItem>
{
	private IComparer<EventLogEvent> comparer;

	private SortOrder direction;

	public ListViewItemComparer(IComparer<EventLogEvent> comparer, SortOrder direction)
	{
		this.comparer = comparer;
		this.direction = direction;
	}

	public int Compare(EventListViewItem first, EventListViewItem second)
	{
		if (direction == SortOrder.Descending)
		{
			return -1 * comparer.Compare(first.Event, second.Event);
		}
		return comparer.Compare(first.Event, second.Event);
	}
}
