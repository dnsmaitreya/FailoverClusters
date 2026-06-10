using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using FailoverClusters.Framework;
using MS.Internal.ServerClusters;
using MS.Internal.ServerClusters.Management;

namespace FailoverClusters.WinForms;

internal class FilterGrid2 : DataGridView
{
	private readonly string AndOperator = Resources.AndOperator_Text;

	private readonly string OrOperator = Resources.OrOperator_Text;

	private const int GroupingColumn = 0;

	private const int OperatorColumn = 1;

	private const int ResourceColumn = 2;

	private const int cQueryColumnsCount = 3;

	private const int cMinimumColumnSize = 25;

	private FilterGrouping _filterGrouping;

	private int _deletedRow;

	private bool _painting;

	private string _oldCellValue;

	private Color _groupedLineColor;

	private Color _groupedBackColor;

	private Color _gridForeColor;

	private Color _gridBackColor;

	private Color _borderColor;

	private bool _isAddingColumns;

	private bool _isDeleting;

	private bool _activated;

	private readonly Dictionary<string, Resource> resources;

	private readonly Dictionary<string, string> displayNames;

	public event EventHandler FilterExpressionChanged;

	public FilterGrid2()
	{
		SetStyle(ControlStyles.ContainerControl, value: true);
		InitializeComponent();
		InitializeGrid();
		resources = new Dictionary<string, Resource>(StringComparer.OrdinalIgnoreCase);
		displayNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
	}

	private string GetValue(int column, int row)
	{
		if (row < 0 || row >= base.Rows.Count || base.Rows[row] == null || base.Rows[row].Cells[column] == null || base.Rows[row].Cells[column].Value == null)
		{
			return string.Empty;
		}
		DataGridViewCell dataGridViewCell = base.Rows[row].Cells[column];
		if (dataGridViewCell.IsInEditMode)
		{
			return dataGridViewCell.EditedFormattedValue.ToString();
		}
		return dataGridViewCell.Value.ToString();
	}

	public void DeleteSelectedClause()
	{
		_isDeleting = true;
		try
		{
			List<DataGridViewRow> rowsSelectedForDeletion = GetRowsSelectedForDeletion();
			if (rowsSelectedForDeletion.Count <= 0)
			{
				return;
			}
			for (int num = rowsSelectedForDeletion.Count - 1; num >= 0; num--)
			{
				if (!rowsSelectedForDeletion[num].IsNewRow)
				{
					_filterGrouping.RemoveRow(rowsSelectedForDeletion[num].Index);
					base.Rows.Remove(rowsSelectedForDeletion[num]);
				}
			}
			base.Rows[0].Cells[1].Value = null;
			for (int i = 1; i < base.Rows.Count - 1; i++)
			{
				if (!base.Rows[i].IsNewRow && string.Compare((string)base.Rows[i].Cells[1].Value, OrOperator, StringComparison.OrdinalIgnoreCase) == 0)
				{
					Connection connection = _filterGrouping.GetConnection(i);
					if (connection == Connection.Down || connection == Connection.None)
					{
						base.Rows[i].Cells[1].Value = AndOperator;
					}
				}
			}
			ClearSelection();
			OnFilterExpressionChanged(EventArgs.Empty);
		}
		finally
		{
			_isDeleting = false;
		}
	}

	private List<DataGridViewRow> GetRowsSelectedForDeletion()
	{
		List<DataGridViewRow> list = new List<DataGridViewRow>();
		if (base.SelectedRows.Count == 0)
		{
			list.Add(base.CurrentRow);
		}
		else
		{
			foreach (DataGridViewRow selectedRow in base.SelectedRows)
			{
				list.Add(selectedRow);
			}
		}
		return list;
	}

	public bool CanDeleteSelectedRows()
	{
		List<DataGridViewRow> rowsSelectedForDeletion = GetRowsSelectedForDeletion();
		bool result = rowsSelectedForDeletion.Count > 0;
		if (rowsSelectedForDeletion.Count == 1)
		{
			DataGridViewRow dataGridViewRow = rowsSelectedForDeletion[0];
			result = dataGridViewRow != null && !dataGridViewRow.IsNewRow;
		}
		return result;
	}

	private void InitializeGrid()
	{
		SuspendLayout();
		base.AutoGenerateColumns = false;
		base.BackgroundColor = SystemColors.Window;
		base.EditMode = DataGridViewEditMode.EditOnEnter;
		base.RowHeadersWidth = 25;
		base.AllowUserToResizeColumns = true;
		base.AllowUserToResizeRows = false;
		AutoSize = false;
		base.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
		base.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithAutoHeaderText;
		base.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		base.ColumnHeadersVisible = true;
		base.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
		base.RowsDefaultCellStyle.WrapMode = DataGridViewTriState.False;
		base.TabIndex = 0;
		base.ScrollBars = ScrollBars.Both;
		base.CellPainting += OnCellPainting;
		base.CellFormatting += OnCellFormatting;
		_filterGrouping = new FilterGrouping();
		try
		{
			_isAddingColumns = true;
			DataGridViewColumn dataGridViewColumn = new DataGridViewImageColumn();
			dataGridViewColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
			dataGridViewColumn.ReadOnly = true;
			dataGridViewColumn.Resizable = DataGridViewTriState.False;
			dataGridViewColumn.HeaderText = string.Empty;
			dataGridViewColumn.Width = 11;
			base.Columns.Add(dataGridViewColumn);
			dataGridViewColumn = new FilterGridComboBoxColumn();
			dataGridViewColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
			dataGridViewColumn.HeaderText = Resources.FilterGridAndOr;
			dataGridViewColumn.Name = "AndOr";
			dataGridViewColumn.Width = 64;
			dataGridViewColumn.MinimumWidth = 25;
			base.Columns.Add(dataGridViewColumn);
			dataGridViewColumn = new FilterGridComboBoxColumn();
			dataGridViewColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
			dataGridViewColumn.HeaderText = Resources.Resource_Text;
			dataGridViewColumn.Name = "Field";
			dataGridViewColumn.MinimumWidth = 25;
			base.Columns.Add(dataGridViewColumn);
		}
		finally
		{
			_isAddingColumns = false;
		}
		_borderColor = SystemColors.ControlDark;
		_groupedLineColor = SystemColors.GrayText;
		_groupedBackColor = SystemColors.Window;
		_gridForeColor = SystemColors.ButtonFace;
		_gridBackColor = SystemColors.Window;
		base.ColumnHeadersDefaultCellStyle.BackColor = _gridForeColor;
		base.ColumnHeadersDefaultCellStyle.Font = null;
		base.RowHeadersDefaultCellStyle.BackColor = _gridForeColor;
		base.BackgroundColor = _gridBackColor;
		base.GridColor = _gridForeColor;
		DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle(base.Columns[0].DefaultCellStyle);
		dataGridViewCellStyle.BackColor = _gridForeColor;
		base.Columns[0].DefaultCellStyle = dataGridViewCellStyle;
		base.BorderStyle = BorderStyle.FixedSingle;
		base.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Outset;
		base.AdvancedRowHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Outset;
		ResumeLayout(performLayout: false);
		base.VerticalScrollBar.VisibleChanged += VerticalScrollBar_VisibleChanged;
	}

	private void StretchLastColumn()
	{
		if (base.Columns.Count > 0)
		{
			int num = ((base.BorderStyle == BorderStyle.FixedSingle) ? 2 : 0);
			int num2 = ((base.VerticalScrollBar != null && base.VerticalScrollBar.Visible) ? base.VerticalScrollBar.Width : 0);
			int num3 = base.ClientSize.Width - base.RowHeadersWidth - num2 - num;
			for (int i = 0; i < 2; i++)
			{
				num3 -= base.Columns[i].Width;
			}
			if (num3 < base.Columns[2].MinimumWidth)
			{
				num3 = base.Columns[2].MinimumWidth;
			}
			base.Columns[2].Width = num3;
		}
	}

	private void InitializeComponent()
	{
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		if (e != null)
		{
			Rectangle rect = new Rectangle(0, 0, base.Width - 1, base.Height - 1);
			e.Graphics.DrawRectangle(new Pen(new SolidBrush(_borderColor)), rect);
		}
	}

	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);
		StretchLastColumn();
	}

	protected override void OnColumnWidthChanged(DataGridViewColumnEventArgs e)
	{
		base.OnColumnWidthChanged(e);
		if (!_isAddingColumns)
		{
			StretchLastColumn();
		}
	}

	private void VerticalScrollBar_VisibleChanged(object sender, EventArgs e)
	{
		StretchLastColumn();
	}

	protected override void OnDefaultValuesNeeded(DataGridViewRowEventArgs e)
	{
		if (e != null)
		{
			e.Row.Cells[1].Value = ((e.Row.Index > 0) ? AndOperator : string.Empty);
			e.Row.Cells[2].Value = string.Empty;
			base.OnDefaultValuesNeeded(e);
		}
	}

	private void OnCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
	{
		if (e.ColumnIndex > 0)
		{
			e.CellStyle.BackColor = base.BackgroundColor;
			if (_filterGrouping.RowIsInGroup(e.RowIndex))
			{
				e.CellStyle.BackColor = _groupedBackColor;
			}
		}
	}

	private void OnCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
	{
		if (_painting || base.DesignMode)
		{
			return;
		}
		_painting = true;
		try
		{
			if (e.RowIndex == -1 || e.ColumnIndex == -1)
			{
				using (Brush brush = new SolidBrush(e.CellStyle.BackColor))
				{
					e.Graphics.FillRectangle(brush, e.CellBounds);
					return;
				}
			}
			if (base.Rows[e.RowIndex].IsNewRow)
			{
				PaintUncommittedRow(e);
			}
			else if (e.ColumnIndex == 0)
			{
				PaintGroupingCell(e);
			}
		}
		finally
		{
			_painting = false;
		}
	}

	private void PaintUncommittedRow(DataGridViewCellPaintingEventArgs e)
	{
		e.Handled = true;
		int columnsWidth = base.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
		Rectangle rectangle = new Rectangle(base.RowHeadersWidth + 1 - base.HorizontalScrollingOffset, e.CellBounds.Top, columnsWidth - 1, e.CellBounds.Height);
		string filterGridClickHereToAddClause = Resources.FilterGridClickHereToAddClause;
		e.Graphics.FillRectangle(new SolidBrush(_gridBackColor), rectangle);
		StringFormat stringFormat = new StringFormat();
		stringFormat.Alignment = StringAlignment.Near;
		stringFormat.LineAlignment = StringAlignment.Center;
		stringFormat.Trimming = StringTrimming.None;
		stringFormat.FormatFlags |= StringFormatFlags.NoClip;
		e.Graphics.DrawString(filterGridClickHereToAddClause, e.CellStyle.Font, new SolidBrush(_groupedLineColor), rectangle, stringFormat);
		using Pen pen = new Pen(_gridForeColor);
		e.Graphics.DrawLine(pen, rectangle.Left, rectangle.Bottom - 1, rectangle.Right, rectangle.Bottom - 1);
		e.Graphics.DrawLine(pen, rectangle.Right - 1, rectangle.Top, rectangle.Right - 1, rectangle.Bottom - 1);
	}

	private void PaintGroupingCell(DataGridViewCellPaintingEventArgs e)
	{
		if (e.CellBounds.Right - e.CellBounds.Left < base.Columns[0].Width)
		{
			return;
		}
		e.Handled = true;
		using Pen pen = new Pen(_groupedLineColor, 2f);
		using Brush brush = new SolidBrush(_groupedBackColor);
		e.Graphics.FillRectangle(brush, e.CellBounds);
		int num = 6;
		Rectangle r = new Rectangle(e.CellBounds.Right - num, e.CellBounds.Top, num, e.CellBounds.Height);
		PaintGroupingBox(e.Graphics, brush, pen, r, _filterGrouping.GetConnection(e.RowIndex));
	}

	private void PaintGroupingBox(Graphics g, Brush b, Pen pen, Rectangle r, Connection c)
	{
		switch (c)
		{
		case Connection.Across:
		{
			Point pt = new Point(r.Left + 1, r.Top);
			Point pt2 = new Point(r.Left + 1, r.Bottom);
			g.FillRectangle(b, r);
			g.DrawLine(pen, pt, pt2);
			break;
		}
		case Connection.Down:
		{
			Point pt = new Point(r.Left + 1, r.Top + 1);
			Point pt2 = new Point(r.Left + 1, r.Bottom);
			Point pt3 = new Point(r.Right, r.Top + 1);
			g.FillRectangle(b, r);
			g.DrawLine(pen, pt, pt2);
			g.DrawLine(pen, pt, pt3);
			break;
		}
		case Connection.Up:
		{
			Point pt = new Point(r.Left + 1, r.Bottom - 3);
			Point pt2 = new Point(r.Left + 1, r.Top);
			Point pt3 = new Point(r.Right, r.Bottom - 3);
			g.FillRectangle(b, r.X, r.Y, r.Width, r.Height - 1);
			g.DrawLine(pen, pt, pt2);
			g.DrawLine(pen, pt, pt3);
			break;
		}
		}
	}

	protected override void OnUserAddedRow(DataGridViewRowEventArgs e)
	{
		base.OnUserAddedRow(e);
		if (e != null)
		{
			_filterGrouping.AddRow(e.Row.Index);
			OnFilterExpressionChanged(EventArgs.Empty);
		}
	}

	protected override void OnUserDeletedRow(DataGridViewRowEventArgs e)
	{
		base.OnUserDeletedRow(e);
		_filterGrouping.RemoveRow(_deletedRow);
		base.Rows[0].Cells[1].Value = null;
		OnFilterExpressionChanged(EventArgs.Empty);
	}

	protected override void OnUserDeletingRow(DataGridViewRowCancelEventArgs e)
	{
		base.OnUserDeletingRow(e);
		if (e != null)
		{
			_deletedRow = e.Row.Index;
			e.Cancel = false;
		}
	}

	protected override void OnGotFocus(EventArgs e)
	{
		base.OnGotFocus(e);
		_activated = true;
	}

	protected override void OnSelectionChanged(EventArgs e)
	{
		if (_activated && !_isDeleting)
		{
			int lowIndex;
			int highIndex;
			if (base.SelectedCells.Count == 1)
			{
				int rowIndex = base.SelectedCells[0].RowIndex;
				if (base.Rows[rowIndex].IsNewRow)
				{
					InsertRow(rowIndex);
				}
			}
			else if (GetSelectedIndexRange(out lowIndex, out highIndex))
			{
				for (int i = lowIndex; i <= highIndex; i++)
				{
					if (!base.Rows[i].Selected)
					{
						base.Rows[i].Selected = true;
					}
				}
			}
			foreach (DataGridViewRow selectedRow in base.SelectedRows)
			{
				if (selectedRow.IsNewRow && selectedRow.Selected)
				{
					selectedRow.Selected = false;
				}
			}
		}
		base.OnSelectionChanged(e);
	}

	protected override void OnCellBeginEdit(DataGridViewCellCancelEventArgs e)
	{
		if (e != null)
		{
			if (base.SelectedRows.Count > 0 || (e.ColumnIndex == 1 && e.RowIndex == 0))
			{
				e.Cancel = true;
				return;
			}
			base.OnCellBeginEdit(e);
			FilterGridComboBoxCell cell = (FilterGridComboBoxCell)base.Rows[e.RowIndex].Cells[e.ColumnIndex];
			_oldCellValue = GetCellValueAsString(cell);
		}
	}

	protected override void OnCellEndEdit(DataGridViewCellEventArgs e)
	{
		base.OnCellEndEdit(e);
		if (e != null)
		{
			FilterGridComboBoxCell cell = (FilterGridComboBoxCell)base.Rows[e.RowIndex].Cells[e.ColumnIndex];
			if (_oldCellValue != GetCellValueAsString(cell))
			{
				OnFilterExpressionChanged(EventArgs.Empty);
			}
		}
	}

	public void NotifyComboBoxSelectionChange(object selectedValue)
	{
		if (base.CurrentCell.ColumnIndex == 1)
		{
			string strA = (string)selectedValue;
			if (string.Compare(strA, OrOperator, StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (!_filterGrouping.AddRowToAdjacentGroup(base.CurrentCell.RowIndex))
				{
					_filterGrouping.AddGrouping(base.CurrentCell.RowIndex - 1, base.CurrentCell.RowIndex);
				}
				Refresh();
			}
			else if (string.Compare(strA, AndOperator, StringComparison.OrdinalIgnoreCase) == 0 && _filterGrouping.RemoveRowFromGroup(base.CurrentCell.RowIndex))
			{
				Refresh();
			}
		}
		OnFilterExpressionChanged(EventArgs.Empty);
	}

	private string GetCellValueAsString(FilterGridComboBoxCell cell)
	{
		string text = cell.Value as string;
		if (text != null)
		{
			text = string.Empty;
		}
		return text;
	}

	private bool GetSelectedIndexRange(out int lowIndex, out int highIndex)
	{
		lowIndex = 0;
		highIndex = 0;
		if (base.SelectedCells.Count > 1)
		{
			lowIndex = base.SelectedCells[0].RowIndex;
			highIndex = base.SelectedCells[0].RowIndex;
			for (int i = 1; i < base.SelectedCells.Count; i++)
			{
				if (base.SelectedCells[i].RowIndex < lowIndex)
				{
					lowIndex = base.SelectedCells[i].RowIndex;
				}
				if (base.SelectedCells[i].RowIndex > highIndex)
				{
					highIndex = base.SelectedCells[i].RowIndex;
				}
			}
			return true;
		}
		return false;
	}

	protected virtual void OnFilterExpressionChanged(EventArgs e)
	{
		this.FilterExpressionChanged?.Invoke(this, e);
	}

	public void SetResources(List<Resource> resourceList)
	{
		resources.Clear();
		displayNames.Clear();
		foreach (Resource resource in resourceList)
		{
			string key = resource.Name;
			int num = 1;
			while (resources.ContainsKey(key))
			{
				key = string.Format(CultureInfo.InvariantCulture, "{0} ({1})", resource.Name, num);
				num++;
			}
			resources.Add(resource.DisplayName, resource);
			displayNames.Add(key, resource.DisplayName);
		}
	}

	public void SetDependencyExpression(DependencyRelationship relationship)
	{
		try
		{
			base.Rows.Clear();
			_filterGrouping.Clear();
			foreach (RelatedResource childResource in relationship.ChildResources)
			{
				AddRow(relationship.RelationType, displayNames[childResource.DisplayName]);
			}
			if (relationship.RelationType == RelationshipType.Or && relationship.ChildResources.Count > 1)
			{
				_filterGrouping.AddGrouping(0, relationship.ChildResources.Count - 1);
			}
			foreach (DependencyRelationship childRelationship in relationship.ChildRelationships)
			{
				int num = base.Rows.Count - 1;
				for (int i = 0; i < childRelationship.ChildResources.Count; i++)
				{
					AddRow((i == 0) ? relationship.RelationType : childRelationship.RelationType, displayNames[childRelationship.ChildResources[i].DisplayName]);
				}
				_filterGrouping.AddGrouping(num, num + childRelationship.ChildResources.Count - 1);
			}
			OnFilterExpressionChanged(EventArgs.Empty);
		}
		finally
		{
			PerformLayout();
		}
	}

	private void AddRow(RelationshipType logicalOperator, string resource)
	{
		DataGridViewRow dataGridViewRow = new DataGridViewRow();
		dataGridViewRow.CreateCells(this);
		if (base.Rows.Count > 1)
		{
			dataGridViewRow.Cells[1].Value = ((logicalOperator == RelationshipType.And) ? AndOperator : OrOperator);
		}
		dataGridViewRow.Cells[2].Value = resource;
		base.Rows.Add(dataGridViewRow);
	}

	public void InsertRow(int index)
	{
		DataGridViewRow dataGridViewRow = new DataGridViewRow();
		dataGridViewRow.CreateCells(this);
		_filterGrouping.AddRow(index);
		if (index == 0)
		{
			base.Rows[0].Cells[1].Value = AndOperator;
		}
		else if (_filterGrouping.RowIsInGroup(index))
		{
			dataGridViewRow.Cells[1].Value = OrOperator;
		}
		else
		{
			dataGridViewRow.Cells[1].Value = AndOperator;
		}
		base.Rows.Insert(index, dataGridViewRow);
		base.CurrentCell = base.Rows[index].Cells[(index != 0) ? 1 : 2];
		OnFilterExpressionChanged(EventArgs.Empty);
	}

	public string GetDependencyExpression()
	{
		return GetDependencyExpression(friendly: false);
	}

	public string GetFriendlyDependencyExpression()
	{
		return GetDependencyExpression(friendly: true);
	}

	private string GetDependencyExpression(bool friendly)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		CommitCurrentCell();
		IntPtr handle = base.Handle;
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < base.Rows.Count; i++)
		{
			if (base.Rows[i].IsNewRow)
			{
				continue;
			}
			string text = GetValue(1, i);
			string text2 = GetValue(2, i);
			if (text.Length == 0 && i != 0)
			{
				if (!friendly)
				{
					ClusterDialogException.ShowTaskDialog(new ClusterInputValidationException(Resources.MissingOperator_Text), handle);
				}
				text = Resources.UnknownDependencyToken_Text;
			}
			if (text2.Length == 0)
			{
				if (!friendly)
				{
					ClusterDialogException.ShowTaskDialog(new ClusterInputValidationException(Resources.MissingDependecy_Text), handle);
					return null;
				}
				text2 = Resources.UnknownDependencyToken_Text;
			}
			Connection connection = _filterGrouping.GetConnection(i);
			string text3 = ((connection == Connection.Down) ? "(" : string.Empty);
			string text4 = ((connection == Connection.Up) ? ")" : string.Empty);
			if (friendly)
			{
				stringBuilder.AppendFormat(CultureInfo.CurrentCulture, "{0} {1}{2}{3} ", text, text3, text2, text4);
			}
			else
			{
				stringBuilder.AppendFormat(CultureInfo.CurrentCulture, "{0} {1}[{2}]{3} ", GetInvariantLogicalOperator(text), text3, resources[text2].Id.ToString(), text4);
			}
		}
		if (stringBuilder.Length == 0 && friendly)
		{
			stringBuilder.Append(Resources.NoDependencies_Text);
		}
		return stringBuilder.ToString().Trim();
	}

	private string GetInvariantLogicalOperator(string localizedOperator)
	{
		if (localizedOperator.Length == 0)
		{
			return string.Empty;
		}
		if (string.Compare(localizedOperator, AndOperator, StringComparison.OrdinalIgnoreCase) == 0)
		{
			return "AND";
		}
		if (string.Compare(localizedOperator, OrOperator, StringComparison.OrdinalIgnoreCase) == 0)
		{
			return "OR";
		}
		return Resources.UnknownDependencyToken_Text;
	}

	public Resource[] GetDependencies()
	{
		CommitCurrentCell();
		Resource[] array = new Resource[base.Rows.Count - 1];
		for (int i = 0; i < base.Rows.Count - 1; i++)
		{
			string value = GetValue(2, i);
			if (!string.IsNullOrEmpty(value))
			{
				array[i] = resources[value];
			}
		}
		return array;
	}

	private void CommitCurrentCell()
	{
		if (base.IsCurrentCellInEditMode)
		{
			CommitEdit(DataGridViewDataErrorContexts.Commit);
		}
	}

	public IList<string> GetDataSource(int row, int column)
	{
		switch (column)
		{
		case 2:
		{
			List<string> list = new List<string>();
			for (int i = 0; i < base.Rows.Count; i++)
			{
				if (i != row)
				{
					list.Add((string)base.Rows[i].Cells[2].Value);
				}
			}
			List<string> list2 = new List<string>();
			foreach (string key in resources.Keys)
			{
				if (!list.Contains(key))
				{
					list2.Add(key);
				}
			}
			list2.Sort();
			return list2;
		}
		case 1:
			return new string[2] { AndOperator, OrOperator };
		default:
			return null;
		}
	}
}

