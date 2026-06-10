using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace KDDSL.ServerClusters.Management;

internal class ControlCodesDebugDialog : Form
{
	private class ListViewNoFicker : ListView
	{
		public ListViewNoFicker()
		{
			SetStyle(ControlStyles.OptimizedDoubleBuffer, value: true);
		}
	}

	private Dictionary<int, string> hashHandleNames = new Dictionary<int, string>();

	private List<ListViewItem> controlCodesList = new List<ListViewItem>();

	private bool isAltKeyPressed;

	private string selectedItemsText;

	private int handleIndex;

	private int handleNameIndex;

	private IContainer components;

	private Button refreshHandlesButton;

	private Button callStackButton;

	private ColumnHeader directionColumnHeader;

	private ColumnHeader typeColumnHeader;

	private ColumnHeader nameColumnHeader;

	private ColumnHeader handleColumnHeader;

	private ColumnHeader controlCodeColumnHeader;

	private ColumnHeader statusCodeColumnHeader;

	private ColumnHeader indexColumnHeader;

	private ColumnHeader controlCodeNameColumnHeader;

	private ColumnHeader timeColumnHeader;

	private ContextMenuStrip clearCopycontextMenu;

	private ToolStripMenuItem clearMenuItem;

	private ToolStripMenuItem copyMenuItem;

	private Label selectedItemsLabel;

	private ListViewNoFicker controlCodesListView;

	private ColumnHeader nodeColumnHeader;

	private ToolTip controlCodeListViewToolTip;

	public ControlCodesDebugDialog()
	{
		InitializeComponent();
		base.Icon = Icons.Cluster;
		selectedItemsText = selectedItemsLabel.Text;
		selectedItemsLabel.Text = string.Format(CultureInfo.CurrentCulture, selectedItemsText, "0", "0");
		for (int i = 0; i < controlCodesListView.Columns.Count; i++)
		{
			if (controlCodesListView.Columns[i].Tag != null && (string)controlCodesListView.Columns[i].Tag == "Handle")
			{
				handleIndex = i;
			}
			if (controlCodesListView.Columns[i].Tag != null && (string)controlCodesListView.Columns[i].Tag == "Name")
			{
				handleNameIndex = i;
			}
		}
	}

	private void ControlCodesListView_DoubleClick(object sender, EventArgs e)
	{
		CallStackButton_Click(sender, e);
	}

	private void CallStackButton_Click(object sender, EventArgs e)
	{
		try
		{
			ListViewItem selectedItem = null;
			lock (controlCodesList)
			{
				if (controlCodesListView.SelectedIndices.Count > 0)
				{
					selectedItem = controlCodesList[controlCodesListView.SelectedIndices[0]];
				}
			}
			if (selectedItem != null)
			{
				Background.QueueWorker((WaitCallback)delegate
				{
					MessageBox.Show(string.Format(CultureInfo.CurrentCulture, selectedItem.Tag.ToString()), "Call Stack");
				});
			}
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Error getting the call stack record Control Codes Debug Dialog View");
		}
	}

	private void RefreshHandlesButton_Click(object sender, EventArgs e)
	{
		try
		{
			int num = 0;
			lock (controlCodesList)
			{
				for (int i = 0; i < controlCodesListView.Items.Count; i++)
				{
					ListViewItem listViewItem = controlCodesListView.Items[i];
					if (string.IsNullOrEmpty(listViewItem.SubItems[handleNameIndex].Text))
					{
						string value = null;
						if (hashHandleNames.TryGetValue(int.Parse(listViewItem.SubItems[handleIndex].Text, CultureInfo.CurrentCulture), out value))
						{
							listViewItem.SubItems[handleNameIndex].Text = value;
							num++;
						}
					}
				}
			}
			if (num != 0)
			{
				controlCodesListView.Refresh();
				MessageBox.Show(string.Format(CultureInfo.CurrentCulture, "{0} handle(s) refreshed...", num.ToString(CultureInfo.CurrentCulture)));
			}
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Error Refreshing handles records Control Codes Debug Dialog View");
		}
	}

	private void ClearMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			lock (controlCodesList)
			{
				controlCodesList.Clear();
				controlCodesListView.VirtualListSize = 0;
				UpdateRecordCount();
			}
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Error cleaning Control Codes Debug Dialog View");
		}
	}

	private void ControlCodesDebugDialog_KeyDown(object sender, KeyEventArgs e)
	{
		isAltKeyPressed = e.Alt;
	}

	private void ControlCodesDebugDialog_KeyUp(object sender, KeyEventArgs e)
	{
		isAltKeyPressed = e.Alt;
		try
		{
			if (!e.Control || e.KeyCode != Keys.A)
			{
				return;
			}
			lock (controlCodesList)
			{
				for (int i = 0; i < controlCodesListView.Items.Count; i++)
				{
					controlCodesListView.Items[i].Selected = true;
				}
			}
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Error selecting Control Codes records in the Debug Dialog View");
		}
	}

	private void ControlCodesListView_KeyUp(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.C && e.Control)
		{
			CopyMenuItem_Click(sender, EventArgs.Empty);
		}
	}

	private void CopyMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			lock (controlCodesList)
			{
				StringBuilder stringBuilder = new StringBuilder();
				List<int> list = new List<int>();
				if (controlCodesListView.SelectedIndices.Count <= 1)
				{
					for (int i = 0; i < controlCodesListView.Items.Count; i++)
					{
						list.Add(i);
					}
				}
				else
				{
					for (int j = 0; j < controlCodesListView.SelectedIndices.Count; j++)
					{
						list.Add(controlCodesListView.SelectedIndices[j]);
					}
				}
				for (int k = 0; k < list.Count; k++)
				{
					foreach (ListViewItem.ListViewSubItem subItem in controlCodesListView.Items[list[k]].SubItems)
					{
						stringBuilder.Append(subItem.Text);
						stringBuilder.Append(',');
					}
					stringBuilder.Append(Environment.NewLine);
				}
				Clipboard.SetText(stringBuilder.ToString());
			}
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Error sending to clipboard Control Codes records in the Debug Dialog View");
		}
	}

	private void ControlCodesListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
	{
		try
		{
			lock (controlCodesList)
			{
				e.Item = controlCodesList[e.ItemIndex];
			}
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Error creating ControlCodesEventArgs record on the Debug Dialog View");
		}
	}

	public void AddRecord(ControlCodesEventArgs record)
	{
		if (base.InvokeRequired)
		{
			UIThreadHandlerV<ControlCodesEventArgs> method = delegate(ControlCodesEventArgs rec)
			{
				AddRecord(rec);
			};
			BeginInvoke((Delegate)(object)method, record);
			return;
		}
		try
		{
			lock (controlCodesList)
			{
				string value = record.ControlCodeOwnerName;
				if (value == null)
				{
					hashHandleNames.TryGetValue(record.Handle, out value);
				}
				else if (!hashHandleNames.TryGetValue(record.Handle, out value))
				{
					hashHandleNames[record.Handle] = record.ControlCodeOwnerName;
				}
				ListViewItem listViewItem = new ListViewItem();
				listViewItem.Text = (controlCodesList.Count + 1).ToString(CultureInfo.CurrentCulture);
				listViewItem.SubItems.Add(((DateTime)(object)record.RequestTime).ToString("h:mm:ss.fff", CultureInfo.CurrentCulture));
				listViewItem.SubItems.Add((!record.IsResponse) ? "Out" : "In");
				listViewItem.SubItems.Add(record.ControlCodeGroup);
				listViewItem.SubItems.Add(value);
				listViewItem.SubItems.Add(record.ControlCodeAlias);
				listViewItem.SubItems.Add(record.StatusCode.ToString(CultureInfo.CurrentCulture));
				listViewItem.SubItems.Add(record.ControlCode.ToString(CultureInfo.CurrentCulture));
				listViewItem.SubItems.Add(record.Handle.ToString(CultureInfo.CurrentCulture));
				listViewItem.SubItems.Add((record.Node != null) ? record.Node.Name : "All nodes");
				listViewItem.Tag = record.CallStack;
				listViewItem.BackColor = ((record.StatusCode != 0) ? Color.Coral : ((!record.IsResponse) ? Color.LightBlue : Color.PaleGreen));
				controlCodesList.Add(listViewItem);
				controlCodesListView.VirtualListSize = controlCodesList.Count;
				if (controlCodesListView.Items.Count > 1 && controlCodesListView.Items[controlCodesListView.Items.Count - 2].Selected)
				{
					controlCodesListView.Items[controlCodesListView.Items.Count - 2].Selected = false;
					listViewItem.Selected = true;
					listViewItem.Focused = true;
					listViewItem.EnsureVisible();
				}
				UpdateRecordCount();
			}
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Error creating ControlCodesEventArgs record on the Debug Dialog View");
		}
	}

	private void ControlCodesListView_MouseUp(object sender, MouseEventArgs e)
	{
		try
		{
			lock (controlCodesList)
			{
				if (controlCodesListView.SelectedIndices.Count != 1 || !isAltKeyPressed)
				{
					return;
				}
				ListViewItem listViewItem = controlCodesListView.Items[controlCodesListView.SelectedIndices[0]];
				for (int i = 0; i < listViewItem.SubItems.Count; i++)
				{
					ListViewItem.ListViewSubItem listViewSubItem = listViewItem.SubItems[i];
					bool flag = false;
					if (i == 0)
					{
						int num = 0;
						for (int j = 1; j < listViewItem.SubItems.Count; j++)
						{
							num += listViewItem.SubItems[j].Bounds.Width;
						}
						int num2 = listViewItem.SubItems[0].Bounds.Width - num;
						if (e.X < num2)
						{
							flag = true;
						}
					}
					if (!flag && (i == 0 || !listViewSubItem.Bounds.Contains(e.Location)))
					{
						continue;
					}
					controlCodesListView.SelectedIndices.Clear();
					for (int k = 0; k < controlCodesListView.Items.Count; k++)
					{
						if (controlCodesListView.Items[k].SubItems[i].Text == listViewSubItem.Text)
						{
							controlCodesListView.SelectedIndices.Add(k);
						}
					}
				}
			}
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Error selecting ControlCodes records on the Debug Dialog View");
		}
	}

	private void ControlCodesListView_VirtualItemsSelectionRangeChanged(object sender, ListViewVirtualItemsSelectionRangeChangedEventArgs e)
	{
		UpdateRecordCount();
	}

	private void ControlCodesListView_SelectedIndexChanged(object sender, EventArgs e)
	{
		UpdateRecordCount();
	}

	private void UpdateRecordCount()
	{
		try
		{
			selectedItemsLabel.Text = string.Format(CultureInfo.CurrentCulture, selectedItemsText, controlCodesListView.Items.Count, controlCodesListView.SelectedIndices.Count.ToString(CultureInfo.CurrentCulture));
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Error selecting ControlCodes records on the Debug Dialog View");
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
		this.components = new System.ComponentModel.Container();
		this.controlCodesListView = new KDDSL.ServerClusters.Management.ControlCodesDebugDialog.ListViewNoFicker();
		this.indexColumnHeader = new System.Windows.Forms.ColumnHeader();
		this.timeColumnHeader = new System.Windows.Forms.ColumnHeader();
		this.directionColumnHeader = new System.Windows.Forms.ColumnHeader();
		this.typeColumnHeader = new System.Windows.Forms.ColumnHeader();
		this.nameColumnHeader = new System.Windows.Forms.ColumnHeader();
		this.controlCodeNameColumnHeader = new System.Windows.Forms.ColumnHeader();
		this.statusCodeColumnHeader = new System.Windows.Forms.ColumnHeader();
		this.controlCodeColumnHeader = new System.Windows.Forms.ColumnHeader();
		this.handleColumnHeader = new System.Windows.Forms.ColumnHeader();
		this.nodeColumnHeader = new System.Windows.Forms.ColumnHeader();
		this.clearCopycontextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.clearMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.copyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.refreshHandlesButton = new System.Windows.Forms.Button();
		this.callStackButton = new System.Windows.Forms.Button();
		this.selectedItemsLabel = new System.Windows.Forms.Label();
		this.controlCodeListViewToolTip = new System.Windows.Forms.ToolTip(this.components);
		this.clearCopycontextMenu.SuspendLayout();
		base.SuspendLayout();
		this.controlCodesListView.AllowColumnReorder = true;
		this.controlCodesListView.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.controlCodesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[10] { this.indexColumnHeader, this.timeColumnHeader, this.directionColumnHeader, this.typeColumnHeader, this.nameColumnHeader, this.controlCodeNameColumnHeader, this.statusCodeColumnHeader, this.controlCodeColumnHeader, this.handleColumnHeader, this.nodeColumnHeader });
		this.controlCodesListView.ContextMenuStrip = this.clearCopycontextMenu;
		this.controlCodesListView.FullRowSelect = true;
		this.controlCodesListView.GridLines = true;
		this.controlCodesListView.HideSelection = false;
		this.controlCodesListView.Location = new System.Drawing.Point(12, 12);
		this.controlCodesListView.Name = "controlCodesListView";
		this.controlCodesListView.Size = new System.Drawing.Size(1144, 530);
		this.controlCodesListView.TabIndex = 0;
		this.controlCodeListViewToolTip.SetToolTip(this.controlCodesListView, "Alt-Click on a subitem to make a ranged selection.");
		this.controlCodesListView.UseCompatibleStateImageBehavior = false;
		this.controlCodesListView.View = System.Windows.Forms.View.Details;
		this.controlCodesListView.VirtualMode = true;
		this.controlCodesListView.VirtualItemsSelectionRangeChanged += new System.Windows.Forms.ListViewVirtualItemsSelectionRangeChangedEventHandler(ControlCodesListView_VirtualItemsSelectionRangeChanged);
		this.controlCodesListView.SelectedIndexChanged += new System.EventHandler(ControlCodesListView_SelectedIndexChanged);
		this.controlCodesListView.MouseUp += new System.Windows.Forms.MouseEventHandler(ControlCodesListView_MouseUp);
		this.controlCodesListView.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(ControlCodesListView_RetrieveVirtualItem);
		this.controlCodesListView.KeyUp += new System.Windows.Forms.KeyEventHandler(ControlCodesListView_KeyUp);
		this.controlCodesListView.DoubleClick += new System.EventHandler(ControlCodesListView_DoubleClick);
		this.indexColumnHeader.Tag = "Index";
		this.indexColumnHeader.Text = "Index";
		this.timeColumnHeader.Tag = "Time";
		this.timeColumnHeader.Text = "Time";
		this.timeColumnHeader.Width = 112;
		this.directionColumnHeader.Tag = "Direction";
		this.directionColumnHeader.Text = "Direction";
		this.typeColumnHeader.Tag = "Type";
		this.typeColumnHeader.Text = "Type";
		this.typeColumnHeader.Width = 73;
		this.nameColumnHeader.Tag = "Name";
		this.nameColumnHeader.Text = "Name";
		this.nameColumnHeader.Width = 145;
		this.controlCodeNameColumnHeader.Tag = "ControlCodeName";
		this.controlCodeNameColumnHeader.Text = "Control Code Name";
		this.controlCodeNameColumnHeader.Width = 347;
		this.statusCodeColumnHeader.Tag = "statusCode";
		this.statusCodeColumnHeader.Text = "Status Code";
		this.statusCodeColumnHeader.Width = 89;
		this.controlCodeColumnHeader.Tag = "ControlCode";
		this.controlCodeColumnHeader.Text = "Control Code";
		this.controlCodeColumnHeader.Width = 83;
		this.handleColumnHeader.Tag = "Handle";
		this.handleColumnHeader.Text = "Handle";
		this.handleColumnHeader.Width = 86;
		this.nodeColumnHeader.Tag = "Node";
		this.nodeColumnHeader.Text = "Node";
		this.clearCopycontextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.clearMenuItem, this.copyMenuItem });
		this.clearCopycontextMenu.Name = "clearCopycontextMenu";
		this.clearCopycontextMenu.Size = new System.Drawing.Size(103, 48);
		this.clearMenuItem.Name = "clearMenuItem";
		this.clearMenuItem.Size = new System.Drawing.Size(102, 22);
		this.clearMenuItem.Text = "Clear";
		this.clearMenuItem.Click += new System.EventHandler(ClearMenuItem_Click);
		this.copyMenuItem.Name = "copyMenuItem";
		this.copyMenuItem.Size = new System.Drawing.Size(102, 22);
		this.copyMenuItem.Text = "Copy";
		this.copyMenuItem.Click += new System.EventHandler(CopyMenuItem_Click);
		this.refreshHandlesButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.refreshHandlesButton.Location = new System.Drawing.Point(1037, 548);
		this.refreshHandlesButton.Name = "refreshHandlesButton";
		this.refreshHandlesButton.Size = new System.Drawing.Size(119, 23);
		this.refreshHandlesButton.TabIndex = 1;
		this.refreshHandlesButton.Text = "Refresh Handles";
		this.refreshHandlesButton.UseVisualStyleBackColor = true;
		this.refreshHandlesButton.Click += new System.EventHandler(RefreshHandlesButton_Click);
		this.callStackButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.callStackButton.Location = new System.Drawing.Point(910, 548);
		this.callStackButton.Name = "callStackButton";
		this.callStackButton.Size = new System.Drawing.Size(119, 23);
		this.callStackButton.TabIndex = 1;
		this.callStackButton.Text = "Call Stack";
		this.callStackButton.UseVisualStyleBackColor = true;
		this.callStackButton.Click += new System.EventHandler(CallStackButton_Click);
		this.selectedItemsLabel.AutoSize = true;
		this.selectedItemsLabel.Location = new System.Drawing.Point(12, 548);
		this.selectedItemsLabel.Name = "selectedItemsLabel";
		this.selectedItemsLabel.Size = new System.Drawing.Size(125, 13);
		this.selectedItemsLabel.TabIndex = 2;
		this.selectedItemsLabel.Text = "{0} records, {1} selected.";
		this.selectedItemsLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1168, 583);
		base.Controls.Add(this.selectedItemsLabel);
		base.Controls.Add(this.refreshHandlesButton);
		base.Controls.Add(this.callStackButton);
		base.Controls.Add(this.controlCodesListView);
		this.ForeColor = System.Drawing.SystemColors.ControlText;
		base.KeyPreview = true;
		base.Name = "ControlCodesDebugDialog";
		this.Text = "Control Codes";
		base.KeyUp += new System.Windows.Forms.KeyEventHandler(ControlCodesDebugDialog_KeyUp);
		base.KeyDown += new System.Windows.Forms.KeyEventHandler(ControlCodesDebugDialog_KeyDown);
		this.clearCopycontextMenu.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
