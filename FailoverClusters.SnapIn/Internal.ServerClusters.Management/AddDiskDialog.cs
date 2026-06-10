using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using FailoverClusters.UI.Controls;

namespace MS.Internal.ServerClusters.Management;

internal class AddDiskDialog : SnapinForm
{
	private enum SelectionBehavior
	{
		Single,
		Multi
	}

	private struct GetClusterableDisksParameter
	{
		public Cluster Cluster;

		public Guid PoolId;
	}

	private ClusterableDisks clusterableDisks;

	private bool singleCheckedItem;

	private IContainer components;

	private Label instructionsLabel;

	private DiskListView diskListView;

	private ColumnHeader nameColumnHeader;

	private Button cancelButton;

	private Button okButton;

	private ColumnHeader capacityColumnHeader;

	internal ClusterableDisks ClusterableDisks => clusterableDisks;

	internal int CapacityColumnIndex => capacityColumnHeader.Index;

	internal static ClusterableDisks AddDisksDialog(Cluster cluster, INotifyUser notifyUser)
	{
		return AddDisksDialog(cluster, notifyUser, Guid.Empty);
	}

	internal static ClusterableDisks AddDisksDialog(Cluster cluster, INotifyUser notifyUser, Guid poolId)
	{
		return ExecuteDialog(cluster, notifyUser, poolId, SelectionBehavior.Multi, Resources.AddDiskDialogTitle_Text, Resources.AddDiskDialogInstructions_Text, showResourceName: true);
	}

	internal static ClusterableDisks RepairDiskDialog(Cluster cluster, INotifyUser notifyUser, string diskResourceName)
	{
		string instructions = string.Format(CultureInfo.CurrentCulture, Resources.RepairDiskDialogInstructionsFormat_Text, diskResourceName);
		return ExecuteDialog(cluster, notifyUser, Guid.Empty, SelectionBehavior.Single, Resources.RepairDiskDialogTitle_Text, instructions, showResourceName: false);
	}

	private static ClusterableDisks ExecuteDialog(Cluster cluster, INotifyUser notifyUser, Guid poolId, SelectionBehavior selectionBehavior, string title, string instructions, bool showResourceName)
	{
		ClusterableDisks clusterableDisks = FindClusterableDisks(notifyUser, cluster, poolId);
		if (clusterableDisks == null || clusterableDisks.AvailableDisks.Count == 0)
		{
			notifyUser.ShowInformational(Resources.NoClusterableDisks_Text);
			return clusterableDisks;
		}
		AddDiskDialog addDiskDialog = new AddDiskDialog(cluster, notifyUser, clusterableDisks, selectionBehavior, title, instructions, showResourceName);
		AddDiskDialog addDiskDialog2 = addDiskDialog;
		try
		{
			if (addDiskDialog.ClusterableDisks.AvailableDisks.Count > 0)
			{
				notifyUser.ShowDialog((Form)(object)addDiskDialog);
			}
			if (((Form)(object)addDiskDialog).DialogResult == DialogResult.OK)
			{
				return addDiskDialog.ClusterableDisks;
			}
			return null;
		}
		finally
		{
			((IDisposable)addDiskDialog2)?.Dispose();
		}
	}

	private AddDiskDialog()
	{
		InitializeComponent();
	}

	private AddDiskDialog(Cluster cluster, INotifyUser notifyUser, ClusterableDisks clusterableDisks, SelectionBehavior selectionBehavior, string title, string instructions, bool showResourceName)
		: this()
	{
		this.clusterableDisks = clusterableDisks;
		if (!showResourceName)
		{
			((ListView)(object)diskListView).Columns.Remove(nameColumnHeader);
		}
		if (selectionBehavior == SelectionBehavior.Multi)
		{
			singleCheckedItem = false;
			((ListView)(object)diskListView).MultiSelect = true;
		}
		else
		{
			singleCheckedItem = true;
			((ListView)(object)diskListView).MultiSelect = false;
		}
		((Control)(object)this).Text = title;
		instructionsLabel.Text = instructions;
		LoadClusterableDisksListView(cluster, notifyUser, showResourceName);
		((BaseListView)diskListView).SetColumnWidth(0, (ColumnWidth)(-1));
		if (showResourceName)
		{
			((BaseListView)diskListView).SetColumnWidth(1, (ColumnWidth)(-1));
		}
		diskListView.DiskDialog = this;
	}

	private void OkButtonClick(object sender, EventArgs e)
	{
		((Form)this).DialogResult = DialogResult.OK;
		foreach (ListViewItem item2 in ((BaseListView)diskListView).Items)
		{
			if (!item2.Checked)
			{
				ClusterDisk item = (ClusterDisk)item2.Tag;
				clusterableDisks.AvailableDisks.Remove(item);
			}
		}
	}

	private void LoadClusterableDisksListView(Cluster cluster, INotifyUser notifyUser, bool showResourceName)
	{
		((ListView)(object)diskListView).SmallImageList = IconsHelp.SmallImageList;
		List<ListViewItem> list = new List<ListViewItem>();
		foreach (ClusterDisk availableDisk in clusterableDisks.AvailableDisks)
		{
			ListViewItem listViewItem;
			if (showResourceName)
			{
				listViewItem = new ListViewItem(availableDisk.Name);
				listViewItem.SubItems.Add(string.Format(CultureInfo.CurrentCulture, Resources.DiskNumberOnNode_Text, (availableDisk.DiskNumber > -1) ? availableDisk.DiskNumber.ToString(CultureInfo.CurrentCulture) : string.Empty, availableDisk.Node));
				listViewItem.SubItems.Add(FormatHelp.GetStorageSizeStringFromULong(availableDisk.Size));
				listViewItem.SubItems.Add(availableDisk.DiskId.ToString());
			}
			else
			{
				listViewItem = new ListViewItem(string.Format(CultureInfo.CurrentCulture, Resources.DiskNumberOnNode_Text, (availableDisk.DiskNumber > -1) ? availableDisk.DiskNumber.ToString(CultureInfo.CurrentCulture) : string.Empty, availableDisk.Node));
				listViewItem.SubItems.Add(FormatHelp.GetStorageSizeStringFromULong(availableDisk.Size));
				listViewItem.SubItems.Add(availableDisk.DiskId.ToString());
			}
			listViewItem.ImageIndex = Icons.PhysicalDiskIndex;
			listViewItem.Tag = availableDisk;
			if (!singleCheckedItem)
			{
				listViewItem.Checked = true;
			}
			list.Add(listViewItem);
		}
		((ListView)(object)diskListView).BeginUpdate();
		((BaseListView)diskListView).Items.AddRange(list.ToArray());
		((ListView)(object)diskListView).EndUpdate();
	}

	private void CancelButtonClick(object sender, EventArgs e)
	{
		((Form)this).DialogResult = DialogResult.Cancel;
		clusterableDisks.AvailableDisks.Clear();
	}

	private static ClusterableDisks FindClusterableDisks(INotifyUser notifyUser, Cluster cluster, Guid poolId)
	{
		CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.SearchingForClusterableDisks_Text, Resources.FindingClusterableDisks_Text);
		ClusterableDisks clusterableDisks = null;
		GetClusterableDisksParameter getClusterableDisksParameter = default(GetClusterableDisksParameter);
		getClusterableDisksParameter.Cluster = cluster;
		getClusterableDisksParameter.PoolId = poolId;
		GetClusterableDisksParameter data = getClusterableDisksParameter;
		using (cluadminWaitDialog)
		{
			return cluadminWaitDialog.ShowDialog(notifyUser, DetermineClusterableDisks, data);
		}
	}

	private static ClusterableDisks DetermineClusterableDisks(CluadminWaitDialog waitDialog, GetClusterableDisksParameter parameter)
	{
		ClusterableDisks obj = new ClusterableDisks();
		obj.DetermineClusterableDisks(parameter.Cluster, parameter.PoolId);
		return obj;
	}

	private void OnItemChecked(object sender, ItemCheckedEventArgs e)
	{
		if (e.Item.Checked && singleCheckedItem)
		{
			foreach (ListViewItem checkedItem in ((ListView)(object)diskListView).CheckedItems)
			{
				if (checkedItem != e.Item)
				{
					checkedItem.Checked = false;
				}
			}
		}
		okButton.Enabled = ((ListView)(object)diskListView).CheckedItems.Count > 0;
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(AddDiskDialog));
		cancelButton = new Button();
		okButton = new Button();
		nameColumnHeader = new ColumnHeader();
		instructionsLabel = new Label();
		diskListView = new DiskListView();
		capacityColumnHeader = new ColumnHeader();
		ColumnHeader columnHeader = new ColumnHeader();
		ColumnHeader columnHeader2 = new ColumnHeader();
		Label label = new Label();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(capacityColumnHeader, "capacityColumnHeader");
		componentResourceManager.ApplyResources(columnHeader, "diskInfoColumnHeader");
		componentResourceManager.ApplyResources(columnHeader2, "signatureColumnHeader");
		componentResourceManager.ApplyResources(label, "availableDisksLabel");
		label.Name = "availableDisksLabel";
		componentResourceManager.ApplyResources(cancelButton, "cancelButton");
		cancelButton.DialogResult = DialogResult.Cancel;
		cancelButton.Name = "cancelButton";
		cancelButton.Click += CancelButtonClick;
		componentResourceManager.ApplyResources(okButton, "okButton");
		okButton.DialogResult = DialogResult.OK;
		okButton.Name = "okButton";
		okButton.Click += OkButtonClick;
		componentResourceManager.ApplyResources(nameColumnHeader, "nameColumnHeader");
		componentResourceManager.ApplyResources(instructionsLabel, "instructionsLabel");
		instructionsLabel.Name = "instructionsLabel";
		componentResourceManager.ApplyResources(diskListView, "diskListView");
		((ListView)(object)diskListView).CheckBoxes = true;
		((ListView)(object)diskListView).Columns.AddRange(new ColumnHeader[4] { nameColumnHeader, columnHeader, capacityColumnHeader, columnHeader2 });
		((BaseListView)diskListView).EnableAutoResizeColumns = true;
		((ListView)(object)diskListView).FullRowSelect = true;
		((BaseListView)diskListView).HeaderStyle = ColumnHeaderStyle.Clickable;
		((BaseListView)diskListView).HideSelection = true;
		((BaseListView)diskListView).IsSortable = true;
		((Control)(object)diskListView).Name = "diskListView";
		((ListView)(object)diskListView).ShowGroups = false;
		((ListView)(object)diskListView).UseCompatibleStateImageBehavior = false;
		((ListView)(object)diskListView).View = View.Details;
		((ListView)(object)diskListView).ItemChecked += OnItemChecked;
		((Form)this).AcceptButton = okButton;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Form)this).CancelButton = cancelButton;
		((Control)this).Controls.Add((Control)(object)diskListView);
		((Control)this).Controls.Add(okButton);
		((Control)this).Controls.Add(cancelButton);
		((Control)this).Controls.Add(label);
		((Control)this).Controls.Add(instructionsLabel);
		((Control)this).Name = "AddDiskDialog";
		((Control)this).ResumeLayout(performLayout: false);
	}
}

