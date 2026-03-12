using System.Windows.Forms;

namespace Microsoft.FailoverClusters.WinForms;

internal class FilterGridComboBoxColumn : DataGridViewColumn
{
	public FilterGridComboBoxColumn()
		: base(new FilterGridComboBoxCell())
	{
	}
}
