using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using FailoverClusters.UI.Common;
using FailoverClusters.UI.Controls;

namespace MS.Internal.ServerClusters.Management;

internal class PhysicalDiskGeneralPropertiesPage : ResourceGeneralPropertiesPage
{
	private string id;

	private ResourceState state;

	private readonly List<ListViewItem> partitions = new List<ListViewItem>();

	private string emptyMsg;

	private bool enableRepairButton;

	private IContainer components;

	private Label diskValueLabel;

	private BaseListView volumesListView;

	private ColumnHeader Volume;

	private ColumnHeader Capacity;

	private ColumnHeader FreeSpace;

	private Button repairButton;

	private LinkLabel repairHelpLinkLabel;

	private ContextMenuStrip copyContextMenuStrip;

	private ToolStripMenuItem copyToolStripMenuItem;

	internal PhysicalDiskGeneralPropertiesPage()
	{
		InitializeComponent();
	}

	internal PhysicalDiskGeneralPropertiesPage(ResourceContext context)
		: base(context, renamable: true)
	{
		emptyMsg = null;
		InitializeComponent();
	}

	private void CopyToolStripMenuItemClick(object sender, EventArgs e)
	{
		UIHelper.SendListViewContentToClipboard((ListView)(object)volumesListView);
	}

	private void VolumesListViewKeyUp(object sender, KeyEventArgs e)
	{
		if (e.KeyData == (Keys.C | Keys.Control))
		{
			UIHelper.SendListViewContentToClipboard((ListView)(object)volumesListView);
		}
	}

	protected override void LoadProperties()
	{
		base.LoadProperties();
		ClusterResource resource = base.Context.Resource;
		state = base.Context.Resource.State;
		enableRepairButton = (base.Context as StorageResourceContext)?.GetRepairPhysicalDiskActionEnabledState() ?? false;
		LoadDiskInfo(resource);
	}

	private void LoadDiskInfo(ClusterResource diskResource)
	{
		partitions.Clear();
		id = string.Empty;
		if (state != ResourceState.Online)
		{
			emptyMsg = Resources.DiskNotOnlineMessage_Text;
		}
		try
		{
			ClusterDisk clusterDisk = diskResource.Storage_GetDiskInfo(includeMountPoints: false);
			id = ((clusterDisk.DiskNumber > -1) ? clusterDisk.DiskNumber.ToString(CultureInfo.CurrentCulture) : string.Empty);
			if (state != ResourceState.Online)
			{
				return;
			}
			if (clusterDisk.PartitionCount == 0)
			{
				emptyMsg = Resources.DiskNoPartition_Text;
				return;
			}
			foreach (ClusterDiskPartition partition in clusterDisk.Partitions)
			{
				ListViewItem listViewItem = new ListViewItem((!string.IsNullOrEmpty(partition.DriveLetter)) ? string.Format(CultureInfo.InvariantCulture, "{0}:", partition.DriveLetter) : partition.Name)
				{
					ImageIndex = Icons.StorageVolumeIndex
				};
				listViewItem.SubItems.Add(FormatHelp.GetStorageSizeStringFromULong(partition.Size));
				listViewItem.SubItems.Add(FormatHelp.GetStorageSizeStringFromULong(partition.FreeSpace));
				partitions.Add(listViewItem);
			}
		}
		catch (ApplicationException caughtException)
		{
			Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(caughtException);
			if (firstException != null)
			{
				if (firstException.NativeErrorCode == -2147018933 || firstException.NativeErrorCode == -2147023728)
				{
					return;
				}
				emptyMsg = string.Format(CultureInfo.CurrentCulture, Resources.DiskInfoFailed_Text, diskResource.DisplayName);
				if (firstException.NativeErrorCode == -2147019873)
				{
					ExceptionHelp.LogException(caughtException, "Error getting disk info for property page");
					return;
				}
			}
			throw;
		}
	}

	protected override void InitializePage()
	{
		base.InitializePage();
		diskValueLabel.Text = id;
		((ListView)(object)volumesListView).SmallImageList = IconsHelp.SmallImageList;
		volumesListView.EmptyText = emptyMsg;
		((ListView)(object)volumesListView).BeginUpdate();
		volumesListView.Items.AddRange(partitions.ToArray());
		((ListView)(object)volumesListView).EndUpdate();
		repairButton.Enabled = enableRepairButton;
		repairHelpLinkLabel.Visible = repairButton.Enabled;
	}

	private void OnRepairButtonClicked(object sender, EventArgs e)
	{
		((StorageResourceContext)base.Context).RepairPhysicalDisk(base.NotifyUser);
	}

	private void RepairHelpLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		HelpProvider.ShowHelp(HelpTopics.FailoverClustersOverviewFwlink);
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
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(PhysicalDiskGeneralPropertiesPage));
		repairHelpLinkLabel = new LinkLabel();
		diskValueLabel = new Label();
		volumesListView = new BaseListView();
		Volume = new ColumnHeader();
		Capacity = new ColumnHeader();
		FreeSpace = new ColumnHeader();
		copyContextMenuStrip = new ContextMenuStrip(components);
		copyToolStripMenuItem = new ToolStripMenuItem();
		repairButton = new Button();
		Label label = new Label();
		copyContextMenuStrip.SuspendLayout();
		((Control)(object)this).SuspendLayout();
		label.AutoEllipsis = true;
		label.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(label, "diskLabel");
		label.Name = "diskLabel";
		componentResourceManager.ApplyResources(repairHelpLinkLabel, "repairHelpLinkLabel");
		repairHelpLinkLabel.Name = "repairHelpLinkLabel";
		repairHelpLinkLabel.TabStop = true;
		repairHelpLinkLabel.LinkClicked += RepairHelpLinkLabelLinkClicked;
		componentResourceManager.ApplyResources(diskValueLabel, "diskValueLabel");
		diskValueLabel.ForeColor = SystemColors.ControlText;
		diskValueLabel.Name = "diskValueLabel";
		componentResourceManager.ApplyResources(volumesListView, "volumesListView");
		((ListView)(object)volumesListView).Columns.AddRange(new ColumnHeader[3] { Volume, Capacity, FreeSpace });
		((Control)(object)volumesListView).ContextMenuStrip = copyContextMenuStrip;
		volumesListView.EnableAutoResizeColumns = true;
		volumesListView.HideSelection = true;
		((Control)(object)volumesListView).Name = "volumesListView";
		((ListView)(object)volumesListView).ShowItemToolTips = true;
		((ListView)(object)volumesListView).UseCompatibleStateImageBehavior = false;
		((ListView)(object)volumesListView).View = View.Details;
		((Control)(object)volumesListView).KeyUp += VolumesListViewKeyUp;
		componentResourceManager.ApplyResources(Volume, "Volume");
		componentResourceManager.ApplyResources(Capacity, "Capacity");
		componentResourceManager.ApplyResources(FreeSpace, "FreeSpace");
		copyContextMenuStrip.Items.AddRange(new ToolStripItem[1] { copyToolStripMenuItem });
		copyContextMenuStrip.Name = "copyContextMenuStrip";
		componentResourceManager.ApplyResources(copyContextMenuStrip, "copyContextMenuStrip");
		copyToolStripMenuItem.Name = "copyToolStripMenuItem";
		componentResourceManager.ApplyResources(copyToolStripMenuItem, "copyToolStripMenuItem");
		copyToolStripMenuItem.Click += CopyToolStripMenuItemClick;
		componentResourceManager.ApplyResources(repairButton, "repairButton");
		repairButton.AccessibleRole = AccessibleRole.PushButton;
		repairButton.ForeColor = SystemColors.ControlText;
		repairButton.Name = "repairButton";
		repairButton.UseVisualStyleBackColor = true;
		repairButton.Click += OnRepairButtonClicked;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)(object)this).Controls.Add(repairHelpLinkLabel);
		((Control)(object)this).Controls.Add(repairButton);
		((Control)(object)this).Controls.Add((Control)(object)volumesListView);
		((Control)(object)this).Controls.Add(label);
		((Control)(object)this).Controls.Add(diskValueLabel);
		((Control)(object)this).Name = "PhysicalDiskGeneralPropertiesPage";
		((Control)(object)this).Controls.SetChildIndex(diskValueLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(label, 0);
		((Control)(object)this).Controls.SetChildIndex((Control)(object)volumesListView, 0);
		((Control)(object)this).Controls.SetChildIndex(repairButton, 0);
		((Control)(object)this).Controls.SetChildIndex(repairHelpLinkLabel, 0);
		copyContextMenuStrip.ResumeLayout(performLayout: false);
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}

