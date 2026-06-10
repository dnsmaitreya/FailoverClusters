using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace KDDSL.ServerClusters.Management;

internal class PropertyGrid : DataGridView
{
	private IContainer components;

	private DataGridViewTextBoxColumn nameColumn;

	private DataGridViewTextBoxColumn typeColumn;

	private DataGridViewTextBoxColumn valueColumn;

	private PropertyCollection properties;

	public event EventHandler PropertiesChanged;

	public PropertyGrid()
	{
		InitializeComponent();
		SetStyle(ControlStyles.ContainerControl, value: true);
		nameColumn = new DataGridViewTextBoxColumn();
		typeColumn = new DataGridViewTextBoxColumn();
		valueColumn = new DataGridViewTextBoxColumn();
		nameColumn.HeaderText = Resources.PropertyGrid_NameColumn_Header_Text;
		nameColumn.Width = Resources.PropertyGrid_NameColumn_Width;
		nameColumn.ReadOnly = true;
		typeColumn.HeaderText = Resources.PropertyGrid_TypeColumn_Header_Text;
		typeColumn.Width = Resources.PropertyGrid_TypeColumn_Width;
		typeColumn.ReadOnly = true;
		valueColumn.HeaderText = Resources.PropertyGrid_ValueColumn_Header_Text;
		valueColumn.Width = Resources.PropertyGrid_ValueColumn_Width;
		base.Columns.AddRange(nameColumn, typeColumn, valueColumn);
	}

	private void OnPropertiesChanged()
	{
		this.PropertiesChanged?.Invoke(this, EventArgs.Empty);
	}

	public void LoadProperties(PropertyCollection props)
	{
		properties = props;
		foreach (Property prop in props)
		{
			string propertyType = GetPropertyType(prop);
			string propertyValue = GetPropertyValue(prop);
			object[] values = new object[3] { prop.Name, propertyType, propertyValue };
			int index = base.Rows.Add(values);
			base.Rows[index].Tag = prop;
		}
	}

	private string GetPropertyType(Property prop)
	{
		if (prop.IsReadOnly)
		{
			return Resources.Property_ReadOnly_Text;
		}
		if (prop.PropertyType == ClusterPropertyType.ByteArray)
		{
			return Resources.Property_NotEditable_Text;
		}
		return Resources.Property_ReadWrite_Text;
	}

	private string GetPropertyValue(Property prop)
	{
		StringBuilder stringBuilder = new StringBuilder();
		switch (prop.PropertyType)
		{
		case ClusterPropertyType.ByteArray:
		{
			byte[] array = (byte[])prop.Value;
			int num = 0;
			byte[] array2 = array;
			foreach (byte b in array2)
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.AppendFormat(CultureInfo.CurrentCulture, " {0:x2}", b);
				}
				else
				{
					stringBuilder.AppendFormat(CultureInfo.CurrentCulture, "{0:x2}", b);
				}
				num++;
				if (num == 4)
				{
					break;
				}
			}
			if (array.Length > 4)
			{
				stringBuilder.Append(" ...");
			}
			stringBuilder.AppendFormat(CultureInfo.CurrentCulture, " ({0} {1})", array.Length, Resources.Bytes_Text);
			break;
		}
		case ClusterPropertyType.StringCollection:
		{
			StringEnumerator enumerator = ((StringCollection)prop.Value).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					if (stringBuilder.Length != 0)
					{
						stringBuilder.AppendFormat(CultureInfo.CurrentCulture, ", {0}", current);
					}
					else
					{
						stringBuilder.Append(current);
					}
				}
			}
			finally
			{
				if (enumerator is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
			break;
		}
		case ClusterPropertyType.DateTime:
			stringBuilder.Append(DateTime.Parse(prop.Value.ToString(), CultureInfo.CurrentCulture).ToLocalTime().ToString(CultureInfo.CurrentCulture));
			break;
		default:
			stringBuilder.Append(prop.Value.ToString());
			break;
		}
		return stringBuilder.ToString();
	}

	private void AssignPropertyValue(Property prop, string value)
	{
		object obj = null;
		switch (prop.PropertyType)
		{
		case ClusterPropertyType.Unknown:
		case ClusterPropertyType.ByteArray:
			DebugLog.LogWarning("These types aren't allowed to be changed");
			break;
		case ClusterPropertyType.String:
		case ClusterPropertyType.ExpandString:
		case ClusterPropertyType.ExpandedString:
			obj = ((value != null) ? value : string.Empty);
			break;
		case ClusterPropertyType.Int32:
			obj = int.Parse(value, CultureInfo.InvariantCulture);
			break;
		case ClusterPropertyType.Int64:
			obj = long.Parse(value, CultureInfo.InvariantCulture);
			break;
		case ClusterPropertyType.UInt16:
			obj = ushort.Parse(value, CultureInfo.InvariantCulture);
			break;
		case ClusterPropertyType.UInt32:
			obj = uint.Parse(value, CultureInfo.InvariantCulture);
			break;
		case ClusterPropertyType.UInt64:
			obj = ulong.Parse(value, CultureInfo.InvariantCulture);
			break;
		case ClusterPropertyType.DateTime:
			obj = DateTime.Parse(value, CultureInfo.InvariantCulture).ToUniversalTime();
			break;
		default:
			DebugLog.LogWarning("Code needed to set this property type: " + prop.PropertyType);
			break;
		case ClusterPropertyType.StringCollection:
			break;
		}
		if (obj != null)
		{
			prop.Value = obj;
		}
	}

	private Property GetPropertyFromRow(int rowNum)
	{
		return (Property)base.Rows[rowNum].Tag;
	}

	private void DisplayMultiStringDialog(DataGridViewRow row, Property prop)
	{
		MultiStringCollectionDialog multiStringCollectionDialog = new MultiStringCollectionDialog();
		try
		{
			multiStringCollectionDialog.StringCollection = (StringCollection)prop.Value;
			if (DialogResult.OK == ((Form)(object)multiStringCollectionDialog).ShowDialog((IWin32Window)this))
			{
				prop.Value = multiStringCollectionDialog.StringCollection;
				string propertyValue = GetPropertyValue(prop);
				row.Cells[2].Value = propertyValue;
				OnPropertiesChanged();
			}
		}
		finally
		{
			((IDisposable)multiStringCollectionDialog)?.Dispose();
		}
	}

	public PropertyCollection GetUpdatedPropertyValues()
	{
		EndEdit();
		foreach (DataGridViewRow item in (IEnumerable)base.Rows)
		{
			string value = (string)item.Cells[2].Value;
			Property property = (Property)item.Tag;
			if (!property.IsReadOnly)
			{
				try
				{
					AssignPropertyValue(property, value);
				}
				catch (Exception innerException)
				{
					throw ExceptionHelp.Build<ClusterInputValidationException>(innerException, new string[2]
					{
						Resources.Property_InvalidValue_Text,
						property.Name
					});
				}
			}
		}
		return properties;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
		((System.ComponentModel.ISupportInitialize)this).BeginInit();
		base.SuspendLayout();
		base.AllowUserToAddRows = false;
		base.AllowUserToDeleteRows = false;
		base.AllowUserToResizeRows = false;
		base.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
		dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		base.DefaultCellStyle = dataGridViewCellStyle;
		base.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
		base.MultiSelect = false;
		base.RowHeadersVisible = false;
		base.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(PropertyGrid_CellBeginEdit);
		base.CurrentCellDirtyStateChanged += new System.EventHandler(PropertyGrid_CurrentCellDirtyStateChanged);
		((System.ComponentModel.ISupportInitialize)this).EndInit();
		base.ResumeLayout(false);
	}

	private void PropertyGrid_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
	{
		Property propertyFromRow = GetPropertyFromRow(e.RowIndex);
		if (propertyFromRow.IsReadOnly)
		{
			e.Cancel = true;
			return;
		}
		switch (propertyFromRow.PropertyType)
		{
		case ClusterPropertyType.ByteArray:
			e.Cancel = true;
			break;
		case ClusterPropertyType.StringCollection:
		{
			e.Cancel = true;
			DataGridViewRow row = base.Rows[e.RowIndex];
			DisplayMultiStringDialog(row, propertyFromRow);
			break;
		}
		}
	}

	private void PropertyGrid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
	{
		OnPropertiesChanged();
	}
}
