using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace KDDSL.ServerClusters.Management;

internal class AddStorageDialog : SnapinForm
{
	private IContainer components;

	private Label instructionsLabel;

	private Label availableDisksLabel;

	private Button okButton;

	private Button cancelButton;

	private ClusterList diskList;

	internal AddStorageDialog(IEnumerable<StorageListItem> availableDisks)
	{
		InitializeComponent();
		if (!UIHelper.DesignMode)
		{
			diskList.BeginUpdate();
			try
			{
				diskList.AddRange((ICollection<ClusterListItem>)(object)availableDisks.ToArray());
				diskList.SetColumns(new string[3]
				{
					Resources.Name_Text,
					Resources.Status_Text,
					Resources.Capacity_Text
				});
				diskList.SetColumnWidth(0, (ColumnWidth)(-1));
				UpdateOkButton();
			}
			finally
			{
				diskList.EndUpdate();
			}
		}
	}

	internal AddStorageDialog(IEnumerable<StorageListItem> availableDisks, string title)
		: this(availableDisks)
	{
		if (title != null)
		{
			((Control)(object)this).Text = title;
		}
	}

	public List<ClusterResource> GetSelectedDisks()
	{
		return (from StorageListItem item in diskList.CheckedItems
			select ((ResourceListItem)item).Resource).ToList();
	}

	private void OkButtonClick(object sender, EventArgs e)
	{
		((Form)this).DialogResult = DialogResult.OK;
	}

	private void CancelButtonClick(object sender, EventArgs e)
	{
		((Form)this).DialogResult = DialogResult.Cancel;
	}

	private void diskList_ItemChecked(object sender, EventArgs e)
	{
		UpdateOkButton();
	}

	private void UpdateOkButton()
	{
		okButton.Enabled = diskList.HasCheckedItems;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (components != null)
			{
				components.Dispose();
			}
			diskList.RemoveRange(new List<ClusterListItem>(diskList.Items));
		}
		((Form)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(AddStorageDialog));
		instructionsLabel = new Label();
		availableDisksLabel = new Label();
		okButton = new Button();
		cancelButton = new Button();
		diskList = new ClusterList();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(instructionsLabel, "instructionsLabel");
		instructionsLabel.Name = "instructionsLabel";
		componentResourceManager.ApplyResources(availableDisksLabel, "availableDisksLabel");
		availableDisksLabel.Name = "availableDisksLabel";
		componentResourceManager.ApplyResources(okButton, "okButton");
		okButton.Name = "okButton";
		okButton.Click += OkButtonClick;
		componentResourceManager.ApplyResources(cancelButton, "cancelButton");
		cancelButton.DialogResult = DialogResult.Cancel;
		cancelButton.Name = "cancelButton";
		cancelButton.Click += CancelButtonClick;
		componentResourceManager.ApplyResources(diskList, "diskList");
		diskList.BorderStyle = BorderStyle.Fixed3D;
		diskList.CheckBoxes = true;
		diskList.HideSelection = true;
		diskList.MultiSelect = false;
		((Control)(object)diskList).Name = "diskList";
		diskList.Scrollable = true;
		diskList.ShowGroups = false;
		diskList.SingleCheckedItem = false;
		diskList.View = View.Details;
		diskList.VirtualMode = false;
		diskList.ItemChecked += diskList_ItemChecked;
		((Form)this).AcceptButton = okButton;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Form)this).CancelButton = cancelButton;
		((Control)this).Controls.Add((Control)(object)diskList);
		((Control)this).Controls.Add(okButton);
		((Control)this).Controls.Add(cancelButton);
		((Control)this).Controls.Add(availableDisksLabel);
		((Control)this).Controls.Add(instructionsLabel);
		((Control)this).Name = "AddStorageDialog";
		((Control)this).ResumeLayout(performLayout: false);
	}
}
