using System;
using System.Windows.Forms;

namespace Microsoft.FailoverClusters.WinForms;

internal class FilterGridComboBox : ComboBox, IDataGridViewEditingControl
{
	private DataGridView _gridView;

	private int _rowIndex;

	private bool _valueChanged;

	private bool _updatingControl;

	private int _previousSelectedIndex;

	public DataGridView EditingControlDataGridView
	{
		get
		{
			return _gridView;
		}
		set
		{
			_gridView = value;
		}
	}

	public Cursor EditingPanelCursor => Cursor;

	public int EditingControlRowIndex
	{
		get
		{
			return _rowIndex;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			_rowIndex = value;
		}
	}

	public bool EditingControlValueChanged
	{
		get
		{
			return _valueChanged;
		}
		set
		{
			_valueChanged = value;
		}
	}

	public object EditingControlFormattedValue
	{
		get
		{
			return Text;
		}
		set
		{
			Text = (string)value;
		}
	}

	public bool RepositionEditingControlOnValueChange => false;

	public bool UpdatingControl
	{
		get
		{
			return _updatingControl;
		}
		set
		{
			_updatingControl = value;
		}
	}

	public FilterGridComboBox()
	{
		base.TabStop = false;
		_valueChanged = false;
		_previousSelectedIndex = SelectedIndex;
	}

	public void ApplyCellStyleToEditingControl(DataGridViewCellStyle gridViewCellStyle)
	{
		if (gridViewCellStyle == null)
		{
			throw new ArgumentNullException("gridViewCellStyle");
		}
		Font = gridViewCellStyle.Font;
		BackColor = gridViewCellStyle.BackColor;
		ForeColor = gridViewCellStyle.ForeColor;
	}

	public void PrepareEditingControlForEdit(bool selectAll)
	{
		if (selectAll)
		{
			SelectAll();
		}
	}

	public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
	{
		if (dataGridViewWantsInputKey)
		{
			Keys keys = keyData & Keys.KeyCode;
			if ((keyData & Keys.Alt) == Keys.Alt && keys == Keys.Down)
			{
				return true;
			}
			if ((keyData & Keys.Return) == Keys.Return)
			{
				return false;
			}
			if (base.DroppedDown)
			{
				return true;
			}
			return false;
		}
		return true;
	}

	public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
	{
		return Text;
	}

	private void NotifyDataGridViewOfValueChange()
	{
		if (!_updatingControl)
		{
			_valueChanged = true;
			if (_gridView != null)
			{
				_gridView.NotifyCurrentCellDirty(dirty: true);
			}
		}
	}

	protected override void OnSelectedIndexChanged(EventArgs e)
	{
		if (SelectedIndex != _previousSelectedIndex)
		{
			base.OnSelectedIndexChanged(e);
			NotifyDataGridViewOfValueChange();
			if (!_updatingControl && _gridView != null)
			{
				((FilterGrid2)_gridView).NotifyComboBoxSelectionChange(base.SelectedValue);
			}
			_previousSelectedIndex = SelectedIndex;
		}
	}

	protected override void OnTextChanged(EventArgs e)
	{
		base.OnTextChanged(e);
		NotifyDataGridViewOfValueChange();
	}
}
