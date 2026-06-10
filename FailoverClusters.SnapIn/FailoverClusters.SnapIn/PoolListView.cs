using System.Windows.Forms;
using FailoverClusters.Framework;
using KDDSL.ServerClusters.Management;

namespace FailoverClusters.SnapIn;

internal class PoolListView : BaseListView
{
	internal AddPoolDialog PoolDialog { get; set; }

	protected override void Sort(SortOrder direction, int columnIndex)
	{
		if (columnIndex == PoolDialog.TotalCapacityColumnIndex)
		{
			((ListView)this).ListViewItemSorter = new CapacityColumnSorter<ClusterableStoragePool>(direction, (ClusterableStoragePool pool) => pool?.TotalCapacity ?? 0);
		}
		else
		{
			((BaseListView)this).Sort(direction, columnIndex);
		}
	}
}

