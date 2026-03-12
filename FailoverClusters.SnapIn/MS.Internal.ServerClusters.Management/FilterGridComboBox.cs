using System;
using System.Windows.Forms;

namespace MS.Internal.ServerClusters.Management;

internal class FilterGridComboBox : ComboBox, IDataGridViewEditingControl
{
	private DataGridView m_gridView;

	private int m_rowIndex;

	private bool m_valueChanged;

	private bool m_updatingControl;

	private int m_previousSelectedIndex;

	public DataGridView EditingControlDataGridView
	{
		get
		{
			return m_gridView;
		}
		set
		{
			m_gridView = value;
		}
	}

	public Cursor EditingPanelCursor => Cursor;

	public int EditingControlRowIndex
	{
		get
		{
			return m_rowIndex;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			m_rowIndex = value;
		}
	}

	public bool EditingControlValueChanged
	{
		get
		{
			return m_valueChanged;
		}
		set
		{
			m_valueChanged = value;
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
			return m_updatingControl;
		}
		set
		{
			m_updatingControl = value;
		}
	}

	public FilterGridComboBox()
	{
		base.TabStop = false;
		m_valueChanged = false;
		m_previousSelectedIndex = SelectedIndex;
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
		if (!m_updatingControl)
		{
			m_valueChanged = true;
			if (m_gridView != null)
			{
				m_gridView.NotifyCurrentCellDirty(dirty: true);
			}
		}
	}

	protected override void OnSelectedIndexChanged(EventArgs e)
	{
		if (SelectedIndex != m_previousSelectedIndex)
		{
			base.OnSelectedIndexChanged(e);
			NotifyDataGridViewOfValueChange();
			if (!m_updatingControl && m_gridView != null)
			{
				((FilterGrid)m_gridView).NotifyComboBoxSelectionChange(base.SelectedValue);
			}
			m_previousSelectedIndex = SelectedIndex;
		}
	}

	protected override void OnTextChanged(EventArgs e)
	{
		base.OnTextChanged(e);
		NotifyDataGridViewOfValueChange();
	}
}
