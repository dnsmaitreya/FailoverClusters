using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MS.Internal.ServerClusters.Controls;

namespace MS.Internal.ServerClusters.Management;

internal class AddRemoveColumnsDialog : SnapinForm
{
	private IContainer components;

	private Button okButton;

	private Button cancelButton;

	private Label instructionsLabel;

	private OrderedListView columnsList;

	public AddRemoveColumnsDialog(ColumnHeader[] columns)
	{
		InitializeComponent();
		foreach (ColumnHeader columnHeader in columns)
		{
			ListViewItem listViewItem = new ListViewItem
			{
				Text = columnHeader.Text,
				Name = columnHeader.Text,
				Tag = columnHeader
			};
			columnsList.Items.Add(listViewItem);
		}
	}

	public void Select(string header)
	{
		ListViewItem[] array = columnsList.Items.Find(header);
		if (array.Length != 0)
		{
			array[0].Checked = true;
		}
	}

	public ColumnHeader[] GetSelectedColumns()
	{
		ColumnHeader[] array = new ColumnHeader[columnsList.CheckedItems.Count];
		for (int i = 0; i < columnsList.CheckedItems.Count; i++)
		{
			array[i] = (ColumnHeader)columnsList.CheckedItems[i].Tag;
		}
		return array;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		((Form)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(AddRemoveColumnsDialog));
		okButton = new Button();
		cancelButton = new Button();
		instructionsLabel = new Label();
		columnsList = new OrderedListView();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(okButton, "okButton");
		okButton.DialogResult = DialogResult.OK;
		okButton.Name = "okButton";
		okButton.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(cancelButton, "cancelButton");
		cancelButton.DialogResult = DialogResult.Cancel;
		cancelButton.Name = "cancelButton";
		cancelButton.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(instructionsLabel, "instructionsLabel");
		instructionsLabel.ForeColor = SystemColors.ControlText;
		instructionsLabel.Name = "instructionsLabel";
		componentResourceManager.ApplyResources(columnsList, "columnsList");
		((Control)(object)columnsList).BackColor = SystemColors.Control;
		columnsList.ImageList = null;
		((Control)(object)columnsList).Name = "columnsList";
		((Form)this).AcceptButton = okButton;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Form)this).CancelButton = cancelButton;
		((Control)this).Controls.Add(instructionsLabel);
		((Control)this).Controls.Add(cancelButton);
		((Control)this).Controls.Add(okButton);
		((Control)this).Controls.Add((Control)(object)columnsList);
		((Control)this).Name = "AddRemoveColumnsDialog";
		((Control)this).ResumeLayout(performLayout: false);
	}
}
