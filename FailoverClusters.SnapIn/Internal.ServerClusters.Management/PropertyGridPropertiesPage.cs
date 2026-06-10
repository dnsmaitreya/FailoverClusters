using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MS.Internal.ServerClusters.Management;

internal class PropertyGridPropertiesPage : ResourcePropertiesPage
{
	private PropertyCollection privateProperties;

	private IContainer components;

	private PropertyGrid propertyGrid;

	private Label instructionLabel;

	internal PropertyGridPropertiesPage(ResourceContext context)
		: base(context, Resources.PropertyGrid_Text)
	{
		InitializeComponent();
	}

	protected override void LoadProperties()
	{
		privateProperties = base.Context.Resource.GetPrivateProperties(PropertyCollectionSet.Both);
	}

	protected override void InitializePage()
	{
		propertyGrid.LoadProperties(privateProperties);
		propertyGrid.PropertiesChanged += propertyGrid_PropertiesChanged;
	}

	protected override bool ValidateProperties()
	{
		privateProperties = propertyGrid.GetUpdatedPropertyValues();
		return true;
	}

	protected override void SaveProperties(CluadminWaitDialog waitDialog)
	{
		try
		{
			SaveProperties(privateProperties);
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Error saving property");
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[1] { Resources.Property_SaveError_Text });
		}
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(PropertyGridPropertiesPage));
		DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
		instructionLabel = new Label();
		propertyGrid = new PropertyGrid();
		((ISupportInitialize)propertyGrid).BeginInit();
		((Control)(object)this).SuspendLayout();
		componentResourceManager.ApplyResources(instructionLabel, "instructionLabel");
		instructionLabel.BackColor = SystemColors.Control;
		instructionLabel.ForeColor = SystemColors.ControlText;
		instructionLabel.Name = "instructionLabel";
		propertyGrid.AllowUserToAddRows = false;
		propertyGrid.AllowUserToDeleteRows = false;
		propertyGrid.AllowUserToResizeRows = false;
		componentResourceManager.ApplyResources(propertyGrid, "propertyGrid");
		propertyGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle.BackColor = SystemColors.Window;
		dataGridViewCellStyle.ForeColor = SystemColors.ControlText;
		dataGridViewCellStyle.SelectionBackColor = SystemColors.Highlight;
		dataGridViewCellStyle.SelectionForeColor = SystemColors.HighlightText;
		dataGridViewCellStyle.WrapMode = DataGridViewTriState.False;
		propertyGrid.DefaultCellStyle = dataGridViewCellStyle;
		propertyGrid.EditMode = DataGridViewEditMode.EditOnEnter;
		propertyGrid.MultiSelect = false;
		propertyGrid.Name = "propertyGrid";
		propertyGrid.RowHeadersVisible = false;
		componentResourceManager.ApplyResources(this, "$this");
		((SnapinUserControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)(object)this).Controls.Add(propertyGrid);
		((Control)(object)this).Controls.Add(instructionLabel);
		((Control)(object)this).ForeColor = SystemColors.ControlText;
		((Control)(object)this).Name = "PropertyGridPropertiesPage";
		((Control)(object)this).Controls.SetChildIndex(instructionLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(propertyGrid, 0);
		((ISupportInitialize)propertyGrid).EndInit();
		((Control)(object)this).ResumeLayout(performLayout: false);
	}

	private void propertyGrid_PropertiesChanged(object sender, EventArgs e)
	{
		base.IsDirty = true;
	}
}
