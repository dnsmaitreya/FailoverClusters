using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FailoverClusters.Framework;
using FailoverClusters.UI.Controls;
using FailoverClusters.WinForms;

namespace MS.Internal.ServerClusters.Management;

internal class ClusterGeneralPropertiesPage : PropertyPageControlBase
{
	private string clusterName;

	private string connectedTo;

	private ControlCodesDebugDialog controlCodesDialog;

	private readonly ClusterContext context;

	private readonly Cluster cluster;

	private ClusterNode node;

	private IContainer components;

	private Label nameLabel;

	private TextBox nameTextBox;

	private Label clusterNameLabel;

	private PictureBox clusterIconPictureBox;

	private LinkLabel manageCoreGroupLinkLabel;

	private Label connectedToLabel;

	private Label connectedToValueLabel;

	private GroupBox connectionGroupBox;

	private FlowLayoutPanel connectedToflowLayoutPanel;

	private Label notificationsLabel;

	private ListBox notificationsListBox;

	private CheckBox registryValuesCheckBox;

	private ContextMenuStrip clearCopyContextMenu;

	private ToolStripMenuItem clearMenuItem;

	private ToolStripMenuItem copyMenuItem;

	private Button showControlCodesButton;

	private Button test;

	private Button test1;

	internal ClusterGeneralPropertiesPage(ClusterContext context)
		: base(Resources.General_Text)
	{
		this.context = context;
		InitializeComponent();
		cluster = this.context.Cluster;
		if (DebugLog.PrivateComponentsEnabled)
		{
			cluster.Notifications += ClusterNotifications;
			Cluster obj = cluster;
			obj.ControlCodesMonitor = (ControlCodesEventHandler)Delegate.Combine(obj.ControlCodesMonitor, new ControlCodesEventHandler(ClusterControlCodes));
		}
		else
		{
			((Control)(object)this).Controls.Remove(connectionGroupBox);
		}
	}

	private void ShowControlCodesButton_Click1(object sender, EventArgs e)
	{
		new TaskFactory().StartNew(delegate
		{
			try
			{
				node = cluster.GetNode("galenb-vm1");
				_ = node.State;
			}
			catch (Exception ex)
			{
				if (ex == null)
				{
					throw ex;
				}
			}
		});
	}

	private void ShowControlCodesButton_Click2(object sender, EventArgs e)
	{
		new TaskFactory().StartNew(delegate
		{
			try
			{
				_ = node.State;
			}
			catch (Exception ex)
			{
				if (ex == null)
				{
					throw ex;
				}
			}
		});
	}

	private void ShowControlCodesButton_Click(object sender, EventArgs e)
	{
		try
		{
			if (controlCodesDialog == null)
			{
				controlCodesDialog = new ControlCodesDebugDialog();
				controlCodesDialog.Closed += delegate
				{
					controlCodesDialog = null;
				};
				((Component)(object)this).Disposed += delegate
				{
					if (controlCodesDialog != null)
					{
						controlCodesDialog.Close();
					}
				};
			}
			else
			{
				controlCodesDialog.Activate();
			}
			controlCodesDialog.Show();
		}
		catch (Exception)
		{
		}
	}

	private void ClusterNotifications(object sender, NotificationEventArgs e)
	{
		UIThreadHandlerV<object, NotificationEventArgs> val = ClusterNotifications;
		if (UIHelper.ExecuteOnUIThread<object, NotificationEventArgs>((ISynchronizeInvoke)this, (Delegate)(object)val, sender, e))
		{
			return;
		}
		string text = e.Filter;
		if (text.StartsWith("CLUSTER_CHANGE_", StringComparison.Ordinal))
		{
			text = text.Remove(0, "CLUSTER_CHANGE_".Length);
		}
		if (registryValuesCheckBox.Checked || !text.StartsWith("REGISTRY", StringComparison.Ordinal))
		{
			if (text == "CLUSTER_RECONNECT")
			{
				LoadProperties();
				notificationsListBox.Items.Add(string.Format(CultureInfo.CurrentCulture, "Cluster Reconnected from node {0} to {1}", connectedToValueLabel.Text, connectedTo));
				connectedToValueLabel.Text = connectedTo;
			}
			notificationsListBox.Items.Add(string.Format(CultureInfo.CurrentCulture, "{0} - {1} (Queue {2} Compressed {3})", TitleCase(text), e.Name, e.QueueSize, e.NotificationsDiscarded));
			if (notificationsListBox.SelectedIndex + 3 > notificationsListBox.Items.Count)
			{
				notificationsListBox.TopIndex = notificationsListBox.Items.Count - 1;
				notificationsListBox.SelectedIndex = notificationsListBox.Items.Count - 1;
			}
		}
	}

	private void ClusterControlCodes(object sender, ControlCodesEventArgs e)
	{
		if (controlCodesDialog != null)
		{
			controlCodesDialog.AddRecord(e);
		}
	}

	private void ClearMenuItem_Click(object sender, EventArgs e)
	{
		notificationsListBox.Items.Clear();
	}

	private void CopyMenuItem_Click(object sender, EventArgs e)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string item in notificationsListBox.Items)
		{
			stringBuilder.AppendLine(item);
		}
		Clipboard.SetText(stringBuilder.ToString());
	}

	private void ClusterNameChanged(object sender, EventArgs e)
	{
		base.IsDirty = true;
	}

	private void ManageCoreGroupLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		try
		{
			CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.FindCoreClusterGroup_Text, Resources.FindingCoreClusterGroup_Text, cluster.Name);
			GroupContext groupContext;
			using (cluadminWaitDialog)
			{
				groupContext = cluadminWaitDialog.ShowDialog<object, GroupContext>(base.NotifyUser, GetCoreGroupContext, null);
				if (cluadminWaitDialog.IsCanceled)
				{
					return;
				}
			}
			List<SnapinPropertyPageControlBase> pageControls = new List<SnapinPropertyPageControlBase>
			{
				new GroupGeneralPropertyPage((FailoverClusters.Framework.Cluster)(object)groupContext.Cluster.FrameworkCluster, groupContext.Group.Id),
				new GroupFailoverPropertyPage((FailoverClusters.Framework.Cluster)(object)groupContext.Cluster.FrameworkCluster, groupContext.Group.Id)
			};
			SnapinPropertySheet snapinPropertySheet = new SnapinPropertySheet(groupContext.DisplayName, pageControls);
			try
			{
				base.NotifyUser.ShowDialog((Form)(object)snapinPropertySheet);
			}
			finally
			{
				((IDisposable)snapinPropertySheet)?.Dispose();
			}
		}
		catch (Exception ex)
		{
			base.NotifyUser.ShowError(ex, Resources.CannotOpenClusterGroupPropertyPages_Text);
		}
	}

	private string TitleCase(string text)
	{
		StringBuilder stringBuilder = new StringBuilder(text.Length);
		bool flag = true;
		foreach (char c in text)
		{
			switch (c)
			{
			case '-':
			case '.':
			case '_':
				stringBuilder.Append(' ');
				flag = true;
				continue;
			case ' ':
				flag = true;
				break;
			}
			stringBuilder.Append(flag ? char.ToUpper(c, CultureInfo.CurrentCulture) : char.ToLower(c, CultureInfo.CurrentCulture));
			flag = false;
		}
		return stringBuilder.ToString();
	}

	private GroupContext GetCoreGroupContext(CluadminWaitDialog waitDialog, object data)
	{
		return ContextFactory.CreateContext(cluster.GetCoreClusterGroup(), context);
	}

	protected override void LoadProperties()
	{
		clusterName = cluster.Name;
		connectedTo = cluster.ConnectedTo;
	}

	protected override void InitializePage()
	{
		WinFormsHelp.SetPictureBoxImage(clusterIconPictureBox, Icons.Cluster);
		clusterNameLabel.Text = clusterName;
		connectedToValueLabel.Text = connectedTo;
		nameTextBox.Text = clusterName;
	}

	protected override bool ValidateProperties()
	{
		ResourceState resourceState = ResourceState.Unknown;
		using (CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.Rename_Cluster_Text, Resources.FindingClusterNetworkName_Text, clusterName))
		{
			resourceState = cluadminWaitDialog.ShowDialog(base.NotifyUser, (CluadminWaitDialog _003Cp0_003E, object _003Cp1_003E) => cluster.GetCoreClusterNetworkName()?.State ?? ResourceState.Unknown, null);
			if (cluadminWaitDialog.IsCanceled)
			{
				return false;
			}
		}
		if (resourceState != ResourceState.Unknown && ClientAccessPointHelp.DisplayAdminClientInterruptionWarning(resourceState, base.NotifyUser) != DialogResult.Yes)
		{
			return false;
		}
		clusterName = InputValidator.ValidateNonemptyString(nameTextBox.Text, Resources.ClusterName_Text);
		return true;
	}

	protected override void SaveProperties(CluadminWaitDialog waitDialog)
	{
		try
		{
			cluster.Rename(clusterName, "SnapIn!ClusterGeneralPropertiesPage.SaveProperties");
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Error saving cluster general properites");
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.ClusterRenameError_Text,
				context.DisplayName
			});
		}
	}

	protected override void CompleteSaveProperties()
	{
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (components != null)
			{
				components.Dispose();
			}
			if (context != null)
			{
				Cluster obj = cluster;
				obj.ControlCodesMonitor = (ControlCodesEventHandler)Delegate.Remove(obj.ControlCodesMonitor, new ControlCodesEventHandler(ClusterControlCodes));
				cluster.Notifications -= ClusterNotifications;
			}
		}
		((SnapinUserControl)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ClusterGeneralPropertiesPage));
		nameLabel = new Label();
		nameTextBox = new TextBox();
		clusterNameLabel = new Label();
		clusterIconPictureBox = new PictureBox();
		manageCoreGroupLinkLabel = new LinkLabel();
		connectedToLabel = new Label();
		connectedToValueLabel = new Label();
		connectionGroupBox = new GroupBox();
		clearCopyContextMenu = new ContextMenuStrip(components);
		clearMenuItem = new ToolStripMenuItem();
		copyMenuItem = new ToolStripMenuItem();
		registryValuesCheckBox = new CheckBox();
		notificationsLabel = new Label();
		notificationsListBox = new ListBox();
		connectedToflowLayoutPanel = new FlowLayoutPanel();
		showControlCodesButton = new Button();
		test = new Button();
		test1 = new Button();
		((ISupportInitialize)clusterIconPictureBox).BeginInit();
		connectionGroupBox.SuspendLayout();
		clearCopyContextMenu.SuspendLayout();
		connectedToflowLayoutPanel.SuspendLayout();
		((Control)(object)this).SuspendLayout();
		componentResourceManager.ApplyResources(nameLabel, "nameLabel");
		nameLabel.ForeColor = SystemColors.ControlText;
		nameLabel.Name = "nameLabel";
		componentResourceManager.ApplyResources(nameTextBox, "nameTextBox");
		nameTextBox.BackColor = SystemColors.Window;
		nameTextBox.Name = "nameTextBox";
		nameTextBox.TextChanged += ClusterNameChanged;
		componentResourceManager.ApplyResources(clusterNameLabel, "clusterNameLabel");
		clusterNameLabel.ForeColor = SystemColors.ControlText;
		clusterNameLabel.Name = "clusterNameLabel";
		componentResourceManager.ApplyResources(clusterIconPictureBox, "clusterIconPictureBox");
		clusterIconPictureBox.Name = "clusterIconPictureBox";
		clusterIconPictureBox.TabStop = false;
		componentResourceManager.ApplyResources(manageCoreGroupLinkLabel, "manageCoreGroupLinkLabel");
		manageCoreGroupLinkLabel.Name = "manageCoreGroupLinkLabel";
		manageCoreGroupLinkLabel.TabStop = true;
		manageCoreGroupLinkLabel.LinkClicked += ManageCoreGroupLinkLabel_LinkClicked;
		componentResourceManager.ApplyResources(connectedToLabel, "connectedToLabel");
		connectedToLabel.ForeColor = SystemColors.ControlText;
		connectedToLabel.Name = "connectedToLabel";
		componentResourceManager.ApplyResources(connectedToValueLabel, "connectedToValueLabel");
		connectedToValueLabel.ForeColor = SystemColors.ControlText;
		connectedToValueLabel.Name = "connectedToValueLabel";
		componentResourceManager.ApplyResources(connectionGroupBox, "connectionGroupBox");
		connectionGroupBox.Controls.Add(showControlCodesButton);
		connectionGroupBox.Controls.Add(test);
		connectionGroupBox.Controls.Add(test1);
		connectionGroupBox.Controls.Add(registryValuesCheckBox);
		connectionGroupBox.Controls.Add(notificationsLabel);
		connectionGroupBox.Controls.Add(notificationsListBox);
		connectionGroupBox.Controls.Add(connectedToflowLayoutPanel);
		connectionGroupBox.Name = "connectionGroupBox";
		connectionGroupBox.TabStop = false;
		clearCopyContextMenu.Items.AddRange(new ToolStripItem[2] { clearMenuItem, copyMenuItem });
		clearCopyContextMenu.Name = "clearCopyContextMenu";
		componentResourceManager.ApplyResources(clearCopyContextMenu, "clearCopyContextMenu");
		clearMenuItem.Name = "clearMenuItem";
		componentResourceManager.ApplyResources(clearMenuItem, "clearMenuItem");
		clearMenuItem.Click += ClearMenuItem_Click;
		copyMenuItem.Name = "copyMenuItem";
		componentResourceManager.ApplyResources(copyMenuItem, "copyMenuItem");
		copyMenuItem.Click += CopyMenuItem_Click;
		componentResourceManager.ApplyResources(registryValuesCheckBox, "registryValuesCheckBox");
		registryValuesCheckBox.ForeColor = SystemColors.ControlText;
		registryValuesCheckBox.Name = "registryValuesCheckBox";
		registryValuesCheckBox.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(notificationsLabel, "notificationsLabel");
		notificationsLabel.ForeColor = SystemColors.ControlText;
		notificationsLabel.Name = "notificationsLabel";
		notificationsListBox.ContextMenuStrip = clearCopyContextMenu;
		notificationsListBox.FormattingEnabled = true;
		componentResourceManager.ApplyResources(notificationsListBox, "notificationsListBox");
		notificationsListBox.Name = "notificationsListBox";
		componentResourceManager.ApplyResources(connectedToflowLayoutPanel, "connectedToflowLayoutPanel");
		connectedToflowLayoutPanel.Controls.Add(connectedToLabel);
		connectedToflowLayoutPanel.Controls.Add(connectedToValueLabel);
		connectedToflowLayoutPanel.Name = "connectedToflowLayoutPanel";
		showControlCodesButton.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(showControlCodesButton, "showControlCodesButton");
		showControlCodesButton.Name = "showControlCodesButton";
		showControlCodesButton.UseVisualStyleBackColor = true;
		showControlCodesButton.Click += ShowControlCodesButton_Click;
		test.ForeColor = SystemColors.ControlText;
		test.Name = "showControlCodesButton";
		test.UseVisualStyleBackColor = true;
		test.Click += ShowControlCodesButton_Click1;
		test.Top = 100;
		test1.ForeColor = SystemColors.ControlText;
		test1.Name = "showControlCodesButton";
		test1.UseVisualStyleBackColor = true;
		test1.Click += ShowControlCodesButton_Click2;
		test1.Top = 110;
		componentResourceManager.ApplyResources(this, "$this");
		((SnapinUserControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)(object)this).Controls.Add(connectionGroupBox);
		((Control)(object)this).Controls.Add(manageCoreGroupLinkLabel);
		((Control)(object)this).Controls.Add(clusterNameLabel);
		((Control)(object)this).Controls.Add(clusterIconPictureBox);
		((Control)(object)this).Controls.Add(nameTextBox);
		((Control)(object)this).Controls.Add(nameLabel);
		((Control)(object)this).ForeColor = SystemColors.Control;
		((Control)(object)this).Name = "ClusterGeneralPropertiesPage";
		((Control)(object)this).Controls.SetChildIndex(nameLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(nameTextBox, 0);
		((Control)(object)this).Controls.SetChildIndex(clusterIconPictureBox, 0);
		((Control)(object)this).Controls.SetChildIndex(clusterNameLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(manageCoreGroupLinkLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(connectionGroupBox, 0);
		((ISupportInitialize)clusterIconPictureBox).EndInit();
		connectionGroupBox.ResumeLayout(performLayout: false);
		connectionGroupBox.PerformLayout();
		clearCopyContextMenu.ResumeLayout(performLayout: false);
		connectedToflowLayoutPanel.ResumeLayout(performLayout: false);
		connectedToflowLayoutPanel.PerformLayout();
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}

