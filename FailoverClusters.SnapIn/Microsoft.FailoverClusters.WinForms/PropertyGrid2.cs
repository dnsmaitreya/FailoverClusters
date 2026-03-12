using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.FailoverClusters.Framework;
using MS.Internal.ServerClusters;
using MS.Internal.ServerClusters.Management;

namespace Microsoft.FailoverClusters.WinForms;

internal class PropertyGrid2 : DataGridView
{
	private readonly IContainer components;

	private readonly DataGridViewTextBoxColumn nameColumn;

	private readonly DataGridViewTextBoxColumn typeColumn;

	private readonly DataGridViewTextBoxColumn valueColumn;

	private ClusterPropertyCollection properties;

	public event EventHandler PropertiesChanged;

	public PropertyGrid2()
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

	public void LoadProperties(ClusterPropertyCollection props)
	{
		properties = props;
		foreach (ClusterProperty prop in props)
		{
			if (prop.PropertyKind != 0)
			{
				string propertyType = GetPropertyType(prop);
				string propertyValue = GetPropertyValue(prop);
				object[] values = new object[3] { prop.Name, propertyType, propertyValue };
				int index = base.Rows.Add(values);
				base.Rows[index].Tag = prop;
			}
		}
	}

	private string GetPropertyType(ClusterProperty prop)
	{
		if (prop.IsReadOnly)
		{
			return Resources.Property_ReadOnly_Text;
		}
		if (prop.PropertyType == Microsoft.FailoverClusters.Framework.ClusterPropertyType.Binary)
		{
			return Resources.Property_NotEditable_Text;
		}
		return Resources.Property_ReadWrite_Text;
	}

	private string GetPropertyValue(ClusterProperty prop)
	{
		StringBuilder stringBuilder = new StringBuilder();
		switch (prop.PropertyType)
		{
		case Microsoft.FailoverClusters.Framework.ClusterPropertyType.Binary:
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
		case Microsoft.FailoverClusters.Framework.ClusterPropertyType.StringCollection:
			foreach (string item in (IEnumerable<string>)prop.Value)
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.AppendFormat(CultureInfo.CurrentCulture, ", {0}", item);
				}
				else
				{
					stringBuilder.Append(item);
				}
			}
			break;
		default:
			stringBuilder.Append(prop.Value.ToString());
			break;
		}
		return stringBuilder.ToString();
	}

	private void AssignPropertyValue(ClusterProperty prop, string value)
	{
		switch (prop.PropertyType)
		{
		case Microsoft.FailoverClusters.Framework.ClusterPropertyType.ExpandString:
			((ClusterPropertyExpandString)prop).TypedValue = value ?? string.Empty;
			break;
		case Microsoft.FailoverClusters.Framework.ClusterPropertyType.ExpandedString:
			((ClusterPropertyExpandedString)prop).TypedValue = value ?? string.Empty;
			break;
		case Microsoft.FailoverClusters.Framework.ClusterPropertyType.String:
			((ClusterPropertyString)prop).TypedValue = value ?? string.Empty;
			break;
		case Microsoft.FailoverClusters.Framework.ClusterPropertyType.Int:
			((ClusterPropertyInt)prop).TypedValue = int.Parse(value, CultureInfo.InvariantCulture);
			break;
		case Microsoft.FailoverClusters.Framework.ClusterPropertyType.Int64:
			((ClusterPropertyLong)prop).TypedValue = long.Parse(value, CultureInfo.InvariantCulture);
			break;
		case Microsoft.FailoverClusters.Framework.ClusterPropertyType.UnsignedInt:
			((ClusterPropertyUInt)prop).TypedValue = uint.Parse(value, CultureInfo.InvariantCulture);
			break;
		case Microsoft.FailoverClusters.Framework.ClusterPropertyType.UnsignedInt64:
			((ClusterPropertyULong)prop).TypedValue = ulong.Parse(value, CultureInfo.InvariantCulture);
			break;
		case Microsoft.FailoverClusters.Framework.ClusterPropertyType.UnsignedShort:
			((ClusterPropertyUShort)prop).TypedValue = ushort.Parse(value, CultureInfo.InvariantCulture);
			break;
		case Microsoft.FailoverClusters.Framework.ClusterPropertyType.DateTime:
			((ClusterPropertyDateTime)prop).TypedValue = DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal);
			break;
		default:
			DebugLog.LogWarning("Code needed to set this property type: " + prop.PropertyType);
			break;
		case Microsoft.FailoverClusters.Framework.ClusterPropertyType.Unknown:
		case Microsoft.FailoverClusters.Framework.ClusterPropertyType.Binary:
		case Microsoft.FailoverClusters.Framework.ClusterPropertyType.StringCollection:
		case Microsoft.FailoverClusters.Framework.ClusterPropertyType.SecurityDescriptor:
			break;
		}
	}

	private ClusterProperty GetPropertyFromRow(int rowNum)
	{
		return (ClusterProperty)base.Rows[rowNum].Tag;
	}

	private void DisplayMultiStringDialog(DataGridViewRow row, ClusterPropertyMultipleStrings prop)
	{
		MultiStringCollectionDialog multiStringCollectionDialog = new MultiStringCollectionDialog();
		try
		{
			StringCollection stringCollection = new StringCollection();
			stringCollection.AddRange(prop.TypedValue.ToArray());
			multiStringCollectionDialog.StringCollection = stringCollection;
			if (DialogResult.OK == ((Form)(object)multiStringCollectionDialog).ShowDialog((IWin32Window)this))
			{
				prop.SetValue(multiStringCollectionDialog.StringCollection.Cast<string>().ToList().AsReadOnly());
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

	public ClusterPropertyCollection GetUpdatedPropertyValues()
	{
		EndEdit();
		foreach (DataGridViewRow item in (IEnumerable)base.Rows)
		{
			string text = (string)item.Cells[2].Value;
			ClusterProperty clusterProperty = (ClusterProperty)item.Tag;
			if (clusterProperty.IsReadOnly)
			{
				continue;
			}
			try
			{
				ClusterProperty clusterProperty2 = (ClusterProperty)clusterProperty.Clone();
				AssignPropertyValue(clusterProperty, text);
				clusterProperty2.OverrideCurrentValue(clusterProperty.Value);
				item.Tag = clusterProperty2;
				if (string.Compare(text, properties[clusterProperty.Name].Value.ToString(), StringComparison.OrdinalIgnoreCase) != 0)
				{
					properties.Add(clusterProperty);
				}
			}
			catch (Exception inner)
			{
				throw new ClusterInputValidationException(string.Format(CultureInfo.CurrentCulture, Resources.Property_InvalidValue_Text, clusterProperty.Name), inner);
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
		base.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(PropertyGridCellBeginEdit);
		base.CurrentCellDirtyStateChanged += new System.EventHandler(PropertyGridCurrentCellDirtyStateChanged);
		((System.ComponentModel.ISupportInitialize)this).EndInit();
		base.ResumeLayout(false);
	}

	private void PropertyGridCellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
	{
		ClusterProperty propertyFromRow = GetPropertyFromRow(e.RowIndex);
		if (propertyFromRow.IsReadOnly)
		{
			e.Cancel = true;
			return;
		}
		switch (propertyFromRow.PropertyType)
		{
		case Microsoft.FailoverClusters.Framework.ClusterPropertyType.Binary:
			e.Cancel = true;
			break;
		case Microsoft.FailoverClusters.Framework.ClusterPropertyType.StringCollection:
		{
			e.Cancel = true;
			DataGridViewRow row = base.Rows[e.RowIndex];
			DisplayMultiStringDialog(row, (ClusterPropertyMultipleStrings)propertyFromRow);
			break;
		}
		}
	}

	private void PropertyGridCurrentCellDirtyStateChanged(object sender, EventArgs e)
	{
		OnPropertiesChanged();
	}
}
