using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using FailoverClusters.Framework;
using FailoverClusters.UI.Controls;
using KDDSL.ServerClusters;
using KDDSL.ServerClusters.Management;

namespace FailoverClusters.SnapIn;

internal class AddPoolDialog : SnapinForm
{
	private enum SelectionBehavior
	{
		Single,
		Multi
	}

	private ClusterableStoragePoolsCollection clusterableStoragePools;

	private bool singleCheckedItem;

	private IContainer components;

	private Label instructionsLabel;

	private PoolListView poolsListView;

	private ColumnHeader nameColumnHeader;

	private Button cancelButton;

	private Button okButton;

	private ColumnHeader totalCapacityColumnHeader;

	internal ClusterableStoragePoolsCollection ClusterableStoragePools => clusterableStoragePools;

	internal int TotalCapacityColumnIndex => totalCapacityColumnHeader.Index;

	internal static ClusterableStoragePoolsCollection AddPoolsDialog(FailoverClusters.Framework.Cluster cluster, INotifyUser notifyUser)
	{
		return ExecuteDialog(cluster, notifyUser, SelectionBehavior.Multi, Resources.AddPoolDialogTitle_Text, Resources.AddPoolDialogInstructions_Text);
	}

	private static ClusterableStoragePoolsCollection ExecuteDialog(FailoverClusters.Framework.Cluster cluster, INotifyUser notifyUser, SelectionBehavior selectionBehavior, string title, string instructions)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		ClusterableStoragePoolsCollection clusterableStoragePoolsCollection = FindClusterablePools(notifyUser, cluster);
		if (clusterableStoragePoolsCollection == null)
		{
			return null;
		}
		if (clusterableStoragePoolsCollection.Error != null)
		{
			ExceptionHelp.LogException(clusterableStoragePoolsCollection.Error, "There was an error getting the clusterable storage pool disks");
			ClusterDialogException.ShowTaskDialog(clusterableStoragePoolsCollection.Error);
			return null;
		}
		if (clusterableStoragePoolsCollection.Count == 0)
		{
			notifyUser.ShowInformational(Resources.NoClusterableStoragePools_Text);
			return clusterableStoragePoolsCollection;
		}
		AddPoolDialog addPoolDialog = new AddPoolDialog(cluster, notifyUser, clusterableStoragePoolsCollection, selectionBehavior, title, instructions);
		AddPoolDialog addPoolDialog2 = addPoolDialog;
		try
		{
			if (addPoolDialog.clusterableStoragePools.Count > 0)
			{
				notifyUser.ShowDialog((Form)(object)addPoolDialog);
			}
			if (((Form)(object)addPoolDialog).DialogResult == DialogResult.OK)
			{
				ClusterableStoragePoolsCollection clusterableStoragePoolsCollection2 = new ClusterableStoragePoolsCollection();
				foreach (ListViewItem checkedItem in ((ListView)(object)addPoolDialog.poolsListView).CheckedItems)
				{
					clusterableStoragePoolsCollection2.Add((ClusterableStoragePool)checkedItem.Tag);
				}
				return clusterableStoragePoolsCollection2;
			}
			return null;
		}
		finally
		{
			((IDisposable)addPoolDialog2)?.Dispose();
		}
	}

	private AddPoolDialog()
	{
		InitializeComponent();
	}

	private AddPoolDialog(FailoverClusters.Framework.Cluster cluster, INotifyUser notifyUser, ClusterableStoragePoolsCollection clusterableStoragePools, SelectionBehavior selectionBehavior, string title, string instructions)
		: this()
	{
		this.clusterableStoragePools = clusterableStoragePools;
		if (selectionBehavior == SelectionBehavior.Multi)
		{
			singleCheckedItem = false;
			((ListView)(object)poolsListView).MultiSelect = true;
		}
		else
		{
			singleCheckedItem = true;
			((ListView)(object)poolsListView).MultiSelect = false;
		}
		((Control)(object)this).Text = title;
		instructionsLabel.Text = instructions;
		LoadClusterablePoolsListView(cluster, notifyUser);
		((BaseListView)poolsListView).EnableAutoResizeColumns = false;
		poolsListView.PoolDialog = this;
	}

	private void OkButtonClick(object sender, EventArgs e)
	{
		((Form)this).DialogResult = DialogResult.OK;
		foreach (ListViewItem item2 in ((BaseListView)poolsListView).Items)
		{
			if (!item2.Checked)
			{
				ClusterableStoragePool item = (ClusterableStoragePool)item2.Tag;
				clusterableStoragePools.Remove(item);
			}
		}
	}

	private void LoadClusterablePoolsListView(FailoverClusters.Framework.Cluster cluster, INotifyUser notifyUser)
	{
		((ListView)(object)poolsListView).SmallImageList = IconsHelp.SmallImageList;
		List<ListViewItem> list = new List<ListViewItem>();
		foreach (ClusterableStoragePool clusterableStoragePool in clusterableStoragePools)
		{
			ListViewItem listViewItem = new ListViewItem(clusterableStoragePool.DisplayName);
			listViewItem.SubItems.Add(FormatHelp.GetStorageSizeStringFromULong(clusterableStoragePool.TotalCapacity));
			listViewItem.ImageIndex = Icons.PhysicalDiskIndex;
			listViewItem.Tag = clusterableStoragePool;
			if (!singleCheckedItem)
			{
				listViewItem.Checked = true;
			}
			list.Add(listViewItem);
		}
		((ListView)(object)poolsListView).BeginUpdate();
		((BaseListView)poolsListView).Items.AddRange(list.ToArray());
		((ListView)(object)poolsListView).EndUpdate();
	}

	private void CancelButtonClick(object sender, EventArgs e)
	{
		((Form)this).DialogResult = DialogResult.Cancel;
		clusterableStoragePools.Clear();
	}

	private static ClusterableStoragePoolsCollection FindClusterablePools(INotifyUser notifyUser, FailoverClusters.Framework.Cluster cluster)
	{
		CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.SearchingForClusterableStoragePools_Text, Resources.FindingClusterableStoragePools_Text);
		ClusterableStoragePoolsCollection clusterableStoragePoolsCollection = null;
		using (cluadminWaitDialog)
		{
			return cluadminWaitDialog.ShowDialog(notifyUser, DetermineClusterablePools, cluster);
		}
	}

	private static ClusterableStoragePoolsCollection DetermineClusterablePools(CluadminWaitDialog waitDialog, FailoverClusters.Framework.Cluster frameworkCluster)
	{
		ClusterableStoragePoolsCollection clusterableStoragePoolsCollection = null;
		AutoResetEvent autoResetEvent = new AutoResetEvent(initialState: false);
		try
		{
			clusterableStoragePoolsCollection = frameworkCluster.GetClusterablePools(null, delegate
			{
				autoResetEvent.Set();
			});
			autoResetEvent.WaitOne();
			return clusterableStoragePoolsCollection;
		}
		finally
		{
			if (autoResetEvent != null)
			{
				((IDisposable)autoResetEvent).Dispose();
			}
		}
	}

	private void OnItemChecked(object sender, ItemCheckedEventArgs e)
	{
		if (e.Item.Checked && singleCheckedItem)
		{
			foreach (ListViewItem checkedItem in ((ListView)(object)poolsListView).CheckedItems)
			{
				if (checkedItem != e.Item)
				{
					checkedItem.Checked = false;
				}
			}
		}
		okButton.Enabled = ((ListView)(object)poolsListView).CheckedItems.Count > 0;
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(AddPoolDialog));
		cancelButton = new Button();
		okButton = new Button();
		nameColumnHeader = new ColumnHeader();
		instructionsLabel = new Label();
		poolsListView = new PoolListView();
		totalCapacityColumnHeader = new ColumnHeader();
		Label label = new Label();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(totalCapacityColumnHeader, "totalCapacityColumnHeader");
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
		componentResourceManager.ApplyResources(poolsListView, "poolsListView");
		((ListView)(object)poolsListView).CheckBoxes = true;
		((ListView)(object)poolsListView).Columns.AddRange(new ColumnHeader[2] { nameColumnHeader, totalCapacityColumnHeader });
		((BaseListView)poolsListView).EnableAutoResizeColumns = true;
		((ListView)(object)poolsListView).FullRowSelect = true;
		((BaseListView)poolsListView).HeaderStyle = ColumnHeaderStyle.Clickable;
		((BaseListView)poolsListView).HideSelection = true;
		((BaseListView)poolsListView).IsSortable = true;
		((Control)(object)poolsListView).Name = "poolsListView";
		((ListView)(object)poolsListView).ShowGroups = false;
		((ListView)(object)poolsListView).UseCompatibleStateImageBehavior = false;
		((ListView)(object)poolsListView).View = View.Details;
		((ListView)(object)poolsListView).ItemChecked += OnItemChecked;
		((Form)this).AcceptButton = okButton;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Form)this).CancelButton = cancelButton;
		((Control)this).Controls.Add((Control)(object)poolsListView);
		((Control)this).Controls.Add(okButton);
		((Control)this).Controls.Add(cancelButton);
		((Control)this).Controls.Add(label);
		((Control)this).Controls.Add(instructionsLabel);
		((Control)this).Name = "AddPoolDialog";
		((Control)this).ResumeLayout(performLayout: false);
	}
}

