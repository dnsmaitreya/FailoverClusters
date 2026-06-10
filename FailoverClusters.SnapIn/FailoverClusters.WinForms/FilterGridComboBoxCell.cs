using System;
using System.Windows.Forms;

namespace FailoverClusters.WinForms;

internal class FilterGridComboBoxCell : DataGridViewTextBoxCell
{
	public override Type EditType => typeof(FilterGridComboBox);

	public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
	{
		base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
		FilterGridComboBox filterGridComboBox = (FilterGridComboBox)base.DataGridView.EditingControl;
		filterGridComboBox.UpdatingControl = true;
		try
		{
			filterGridComboBox.DataSource = ((FilterGrid2)base.DataGridView).GetDataSource(base.RowIndex, base.ColumnIndex);
			filterGridComboBox.SelectedIndex = -1;
			filterGridComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			filterGridComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
			filterGridComboBox.AutoCompleteMode = AutoCompleteMode.Append;
			filterGridComboBox.Text = (string)initialFormattedValue;
		}
		finally
		{
			filterGridComboBox.UpdatingControl = false;
		}
	}
}

