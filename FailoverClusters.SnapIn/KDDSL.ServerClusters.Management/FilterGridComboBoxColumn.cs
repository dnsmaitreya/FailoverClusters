using System.Windows.Forms;

namespace KDDSL.ServerClusters.Management;

internal class FilterGridComboBoxColumn : DataGridViewColumn
{
	public FilterGridComboBoxColumn()
		: base(new FilterGridComboBoxCell())
	{
	}
}
