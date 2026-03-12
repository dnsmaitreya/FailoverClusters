using System.Windows.Forms;
using Microsoft.FailoverClusters.SnapIn;

namespace MS.Internal.ServerClusters.Management;

internal class DiskListView : BaseListView
{
	internal AddDiskDialog DiskDialog { get; set; }

	protected override void Sort(SortOrder direction, int columnIndex)
	{
		if (columnIndex == DiskDialog.CapacityColumnIndex)
		{
			((ListView)this).ListViewItemSorter = new CapacityColumnSorter<ClusterDisk>(direction, (ClusterDisk disk) => disk?.Size ?? 0);
		}
		else
		{
			((BaseListView)this).Sort(direction, columnIndex);
		}
	}
}
