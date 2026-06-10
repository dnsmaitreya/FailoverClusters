using System.Windows.Forms;

namespace MS.Internal.ServerClusters.Management;

internal class FilterGridComboBoxColumn : DataGridViewColumn
{
	public FilterGridComboBoxColumn()
		: base(new FilterGridComboBoxCell())
	{
	}
}
