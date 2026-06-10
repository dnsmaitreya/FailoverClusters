using System.Windows.Forms;

namespace FailoverClusters.WinForms;

internal class FilterGridComboBoxColumn : DataGridViewColumn
{
	public FilterGridComboBoxColumn()
		: base(new FilterGridComboBoxCell())
	{
	}
}

