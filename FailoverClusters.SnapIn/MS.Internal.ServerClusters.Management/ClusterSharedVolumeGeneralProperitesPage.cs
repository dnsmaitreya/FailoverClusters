using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.FailoverClusters.UI.Controls;

namespace MS.Internal.ServerClusters.Management;

internal class ClusterSharedVolumeGeneralProperitesPage : ResourceGeneralPropertiesPage
{
	private ResourceState state;

	private readonly List<ListViewItem> volumes = new List<ListViewItem>();

	private string emptyMsg;

	private IContainer components;

	private BaseListView volumesListView;

	private ContextMenuStrip copyContextMenuStrip;

	private ToolStripMenuItem copyToolStripMenuItem;

	private ColumnHeader volume;

	private ColumnHeader redirected;

	private ColumnHeader capacity;

	private ColumnHeader freeSpace;

	public ClusterSharedVolumeGeneralProperitesPage()
	{
		InitializeComponent();
	}

	internal ClusterSharedVolumeGeneralProperitesPage(ResourceContext context)
		: base(context, renamable: true)
	{
		emptyMsg = string.Empty;
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
		LoadSharedVolumeInfo(resource);
	}

	private void LoadSharedVolumeInfo(ClusterResource sharedVolume)
	{
		volumes.Clear();
		if (state != ResourceState.Online)
		{
			emptyMsg = Resources.SharedVolumeNotOnline_Text;
		}
		try
		{
			if (state != ResourceState.Online)
			{
				return;
			}
			ICollection<ClusterSharedVolumeInfo> collection = sharedVolume.PhysicalDisk_GetSharedVolumeInfo();
			if (collection.Count == 0)
			{
				emptyMsg = Resources.SharedVolumesNoVolumes_Text;
				return;
			}
			foreach (ClusterSharedVolumeInfo item in collection)
			{
				ListViewItem listViewItem = new ListViewItem(item.FriendlyVolumeName)
				{
					ImageIndex = Icons.SharedClusterVolumeIndex
				};
				listViewItem.SubItems.Add((item.FaultState == ClusterSharedVolumeFaultState.NoDirectIO) ? Resources.Yes_Text : Resources.No_Text);
				listViewItem.SubItems.Add(FormatHelp.GetStorageSizeStringFromULong(item.Partition.Size));
				listViewItem.SubItems.Add(FormatHelp.GetStorageSizeStringFromULong(item.Partition.FreeSpace));
				volumes.Add(listViewItem);
			}
		}
		catch (Exception caughtException)
		{
			emptyMsg = string.Format(CultureInfo.CurrentCulture, Resources.SharedVolumeInfoFailed_Text, sharedVolume.DisplayName);
			Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(caughtException);
			if (firstException != null && (firstException.NativeErrorCode == -2147023728 || firstException.NativeErrorCode == -2147019873))
			{
				ExceptionHelp.LogException(caughtException, "Error getting cluster shared volume info for property page");
				return;
			}
			throw;
		}
	}

	private void ResizeColumns()
	{
		foreach (ColumnHeader column in ((ListView)(object)volumesListView).Columns)
		{
			column.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
			int width = column.Width;
			column.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
			if (width > column.Width)
			{
				column.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
			}
		}
		((Control)(object)volumesListView).Refresh();
	}

	protected override void InitializePage()
	{
		base.InitializePage();
		((ListView)(object)volumesListView).SmallImageList = IconsHelp.SmallImageList;
		volumesListView.EmptyText = emptyMsg;
		foreach (ListViewItem volume in volumes)
		{
			volumesListView.Items.Add(volume);
		}
		ResizeColumns();
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
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ClusterSharedVolumeGeneralProperitesPage));
		volumesListView = new BaseListView();
		volume = new ColumnHeader();
		redirected = new ColumnHeader();
		capacity = new ColumnHeader();
		freeSpace = new ColumnHeader();
		copyContextMenuStrip = new ContextMenuStrip(components);
		copyToolStripMenuItem = new ToolStripMenuItem();
		copyContextMenuStrip.SuspendLayout();
		((Control)(object)this).SuspendLayout();
		componentResourceManager.ApplyResources(volumesListView, "volumesListView");
		((ListView)(object)volumesListView).Columns.AddRange(new ColumnHeader[4] { volume, redirected, capacity, freeSpace });
		((Control)(object)volumesListView).ContextMenuStrip = copyContextMenuStrip;
		volumesListView.EnableAutoResizeColumns = false;
		volumesListView.HideSelection = true;
		((ListView)(object)volumesListView).MultiSelect = false;
		((Control)(object)volumesListView).Name = "volumesListView";
		((ListView)(object)volumesListView).ShowItemToolTips = true;
		((ListView)(object)volumesListView).UseCompatibleStateImageBehavior = false;
		((ListView)(object)volumesListView).View = View.Details;
		((Control)(object)volumesListView).KeyUp += VolumesListViewKeyUp;
		componentResourceManager.ApplyResources(volume, "volume");
		componentResourceManager.ApplyResources(redirected, "redirected");
		componentResourceManager.ApplyResources(capacity, "capacity");
		componentResourceManager.ApplyResources(freeSpace, "freeSpace");
		copyContextMenuStrip.Items.AddRange(new ToolStripItem[1] { copyToolStripMenuItem });
		copyContextMenuStrip.Name = "copyContextMenuStrip";
		componentResourceManager.ApplyResources(copyContextMenuStrip, "copyContextMenuStrip");
		copyToolStripMenuItem.Name = "copyToolStripMenuItem";
		componentResourceManager.ApplyResources(copyToolStripMenuItem, "copyToolStripMenuItem");
		copyToolStripMenuItem.Click += CopyToolStripMenuItemClick;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)(object)this).Controls.Add((Control)(object)volumesListView);
		((Control)(object)this).Name = "ClusterSharedVolumeGeneralProperitesPage";
		((Control)(object)this).Controls.SetChildIndex((Control)(object)volumesListView, 0);
		copyContextMenuStrip.ResumeLayout(performLayout: false);
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}
