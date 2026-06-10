using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using FailoverClusters.UI.Common;
using FailoverClusters.UI.Controls;

namespace MS.Internal.ServerClusters.Management;

internal class OpenClusterDlg : SnapinForm
{
	private ICollection<string> clusterMRU;

	private IContainer components;

	private Button oKButton;

	private Button cancelButton;

	private Label clusterNameLabel;

	private SeparatorComboBox clusterNameComboBox;

	private Button browseButton;

	private Panel backgroundPanel;

	private Label clusterDescriptionLabel;

	private PictureBox clusterIconPictureBox;

	internal string ClusterName
	{
		get
		{
			string text = clusterNameComboBox.Text.Trim();
			if (text == Resources.ThisServerEntry_Text)
			{
				text = ".";
			}
			return text;
		}
	}

	public OpenClusterDlg(ICollection<string> clusterMRU)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		InitializeComponent();
		clusterNameComboBox.DropDownStyle = ComboBoxStyle.DropDown;
		clusterNameComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
		UIHelper.SystemSettingsChanging += new SystemSettingsChangingEventHandler(InitSystemColors);
		InitSystemColors(this, EventArgs.Empty);
		((Form)this).Icon = Icons.Cluster;
		clusterIconPictureBox.Image = IconsHelp.LargeImageList.Images[Icons.ClusterIndex];
		((SnapinFormBase)this).SetShowIcon(true);
		oKButton.Enabled = false;
		this.clusterMRU = clusterMRU;
	}

	private void BrowseButton_Click(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		ClusterBrowserDialog val = new ClusterBrowserDialog();
		if (((Form)(object)val).ShowDialog((IWin32Window)this) == DialogResult.OK)
		{
			clusterNameComboBox.Text = val.ClusterName;
			oKButton.Focus();
		}
	}

	private void OKButton_Click(object sender, EventArgs e)
	{
		((Form)this).DialogResult = DialogResult.OK;
	}

	private void ClusterNameComboBox_TextChanged(object sender, EventArgs e)
	{
		ComboBox comboBox = sender as ComboBox;
		string obj = ((comboBox.SelectedItem != null) ? comboBox.SelectedItem.ToString() : null);
		string text = comboBox.Text;
		oKButton.Enabled = false;
		if (obj != null)
		{
			oKButton.Enabled = clusterNameComboBox.Text.Trim().Length > 0;
		}
		else if (!string.IsNullOrEmpty(text))
		{
			oKButton.Enabled = true;
		}
	}

	private void InitSystemColors(object sender, EventArgs e)
	{
		if (UIHelper.IsThemeEnabled)
		{
			((Control)(object)this).BackColor = SystemColors.Window;
		}
		else
		{
			((Control)(object)this).BackColor = SystemColors.Control;
		}
	}

	private void LoadMRUList()
	{
		if (clusterMRU != null && clusterMRU.Count > 0)
		{
			clusterNameComboBox.Items.AddRange(clusterMRU.Distinct(StringComparer.CurrentCultureIgnoreCase).Select((Func<string, object>)((string clusterName) => clusterName.ToLower(CultureInfo.CurrentCulture))).ToArray());
			clusterNameComboBox.Invalidate(invalidateChildren: true);
		}
	}

	protected override void OnLoad(EventArgs e)
	{
		((Form)this).OnLoad(e);
		Cursor cursor = ((Control)(object)this).Cursor;
		try
		{
			((Control)(object)this).Cursor = Cursors.WaitCursor;
			clusterNameComboBox.Items.Clear();
			NodeClusterState nodeClusterState = NodeClusterState.None;
			try
			{
				nodeClusterState = ClusterNode.GetClusterState(null);
			}
			catch (ApplicationException ex)
			{
				ClusterLog.LogException((Exception)ex, "There was an error getting the cluster node state for the local machine.");
			}
			if (nodeClusterState == NodeClusterState.Running || nodeClusterState == NodeClusterState.NotRunning)
			{
				if (clusterMRU != null && clusterMRU.Count > 0)
				{
					SeparatorComboBox.SeparatorItem item = new SeparatorComboBox.SeparatorItem(Resources.ThisServerEntry_Text);
					clusterNameComboBox.Items.Insert(0, item);
				}
				else
				{
					clusterNameComboBox.Items.Insert(0, Resources.ThisServerEntry_Text);
				}
			}
			LoadMRUList();
			if (clusterNameComboBox.Items.Count > 0)
			{
				clusterNameComboBox.SelectedIndex = 0;
			}
		}
		finally
		{
			((Control)(object)this).Cursor = cursor;
		}
		((Form)this).CenterToParent();
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		((Form)this).OnPaint(e);
		if (UIHelper.IsThemeEnabled)
		{
			using (Pen pen = new Pen(SystemColors.ControlLight))
			{
				e.Graphics.DrawLine(pen, backgroundPanel.Left, backgroundPanel.Top - 1, backgroundPanel.Width, backgroundPanel.Top - 1);
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		if (disposing && components != null)
		{
			UIHelper.SystemSettingsChanging -= new SystemSettingsChangingEventHandler(InitSystemColors);
			components.Dispose();
		}
		((Form)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(OpenClusterDlg));
		oKButton = new Button();
		cancelButton = new Button();
		clusterNameLabel = new Label();
		browseButton = new Button();
		backgroundPanel = new Panel();
		clusterDescriptionLabel = new Label();
		clusterIconPictureBox = new PictureBox();
		clusterNameComboBox = new SeparatorComboBox();
		backgroundPanel.SuspendLayout();
		((ISupportInitialize)clusterIconPictureBox).BeginInit();
		((Control)this).SuspendLayout();
		oKButton.AccessibleRole = AccessibleRole.PushButton;
		componentResourceManager.ApplyResources(oKButton, "oKButton");
		oKButton.DialogResult = DialogResult.OK;
		oKButton.Name = "oKButton";
		oKButton.Click += OKButton_Click;
		cancelButton.AccessibleRole = AccessibleRole.PushButton;
		componentResourceManager.ApplyResources(cancelButton, "cancelButton");
		cancelButton.DialogResult = DialogResult.Cancel;
		cancelButton.Name = "cancelButton";
		clusterNameLabel.AccessibleRole = AccessibleRole.StaticText;
		componentResourceManager.ApplyResources(clusterNameLabel, "clusterNameLabel");
		clusterNameLabel.Name = "clusterNameLabel";
		browseButton.AccessibleRole = AccessibleRole.PushButton;
		componentResourceManager.ApplyResources(browseButton, "browseButton");
		browseButton.Name = "browseButton";
		browseButton.UseVisualStyleBackColor = true;
		browseButton.Click += BrowseButton_Click;
		componentResourceManager.ApplyResources(backgroundPanel, "backgroundPanel");
		backgroundPanel.BackColor = SystemColors.Control;
		backgroundPanel.Controls.Add(oKButton);
		backgroundPanel.Controls.Add(browseButton);
		backgroundPanel.Controls.Add(cancelButton);
		backgroundPanel.Name = "backgroundPanel";
		clusterDescriptionLabel.AccessibleRole = AccessibleRole.StaticText;
		componentResourceManager.ApplyResources(clusterDescriptionLabel, "clusterDescriptionLabel");
		clusterDescriptionLabel.Name = "clusterDescriptionLabel";
		componentResourceManager.ApplyResources(clusterIconPictureBox, "clusterIconPictureBox");
		clusterIconPictureBox.Name = "clusterIconPictureBox";
		clusterIconPictureBox.TabStop = false;
		clusterNameComboBox.AccessibleRole = AccessibleRole.ComboBox;
		componentResourceManager.ApplyResources(clusterNameComboBox, "clusterNameComboBox");
		clusterNameComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
		clusterNameComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
		clusterNameComboBox.DrawMode = DrawMode.OwnerDrawVariable;
		clusterNameComboBox.DropDownHeight = 200;
		clusterNameComboBox.DropDownWidth = 240;
		clusterNameComboBox.FormattingEnabled = true;
		clusterNameComboBox.Name = "clusterNameComboBox";
		clusterNameComboBox.TextChanged += ClusterNameComboBox_TextChanged;
		((Form)this).AcceptButton = oKButton;
		((Control)this).AccessibleRole = AccessibleRole.Dialog;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Form)this).CancelButton = cancelButton;
		((Control)this).Controls.Add(clusterIconPictureBox);
		((Control)this).Controls.Add(clusterDescriptionLabel);
		((Control)this).Controls.Add(backgroundPanel);
		((Control)this).Controls.Add(clusterNameComboBox);
		((Control)this).Controls.Add(clusterNameLabel);
		((Form)this).FormBorderStyle = FormBorderStyle.FixedSingle;
		((Control)this).Name = "OpenClusterDlg";
		backgroundPanel.ResumeLayout(performLayout: false);
		((ISupportInitialize)clusterIconPictureBox).EndInit();
		((Control)this).ResumeLayout(performLayout: false);
	}
}

