using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace KDDSL.ServerClusters.Management;

internal class AddResourceTypeDialog : SnapinForm
{
	private INotifyUser notifyUser;

	private IContainer components;

	private Button cancelButton;

	private Button okButton;

	private TextBox resourceNameTextBox;

	private Label resourceNameLabel;

	private Button browseButton;

	private TextBox resourceDllTextBox;

	private TextBox resourceDisplayNameTextBox;

	private Label resourceDisplayNameLabel;

	private Label labelResourcePath;

	public string ResourceTypeName => resourceNameTextBox.Text.Trim();

	public string ResourceTypeDisplayName => resourceDisplayNameTextBox.Text.Trim();

	public string ResourceDll => resourceDllTextBox.Text.Trim();

	public AddResourceTypeDialog()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		InitializeComponent();
		notifyUser = (INotifyUser)new MessageBoxNotifyUser((IWin32Window)this);
	}

	private void BrowseButtonClick(object sender, EventArgs e)
	{
		OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.CheckFileExists = true;
		openFileDialog.Filter = Resources.DLLFilter_Text;
		openFileDialog.InitialDirectory = ".";
		openFileDialog.RestoreDirectory = true;
		openFileDialog.Title = Resources.SelectResourceDll_Text;
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			resourceDllTextBox.Text = openFileDialog.FileName;
			resourceNameTextBox.Focus();
		}
		else
		{
			resourceDllTextBox.Focus();
		}
	}

	private void OkButtonClick(object sender, EventArgs e)
	{
		if (ResourceDll.Length == 0)
		{
			notifyUser.ShowWarning(Resources.InputResourceDll_Text);
		}
		else if (!File.Exists(ResourceDll))
		{
			notifyUser.ShowWarning(Resources.ResourceDllDoesNotExist_Text);
		}
		else if (ResourceTypeName.Length == 0)
		{
			notifyUser.ShowWarning(Resources.InputResourceTypeName_Text);
		}
		else if (ResourceTypeDisplayName.Length == 0)
		{
			notifyUser.ShowWarning(Resources.InputResourceTypeDisplayName_Text);
		}
		else
		{
			((Form)this).DialogResult = DialogResult.OK;
		}
	}

	private void CancelButtonClick(object sender, EventArgs e)
	{
		((Form)this).DialogResult = DialogResult.Cancel;
	}

	private void AddResourceTypeDialog_Load(object sender, EventArgs e)
	{
		resourceDllTextBox.Select();
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(AddResourceTypeDialog));
		cancelButton = new Button();
		okButton = new Button();
		resourceNameTextBox = new TextBox();
		resourceNameLabel = new Label();
		browseButton = new Button();
		resourceDllTextBox = new TextBox();
		resourceDisplayNameTextBox = new TextBox();
		resourceDisplayNameLabel = new Label();
		labelResourcePath = new Label();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(cancelButton, "cancelButton");
		cancelButton.DialogResult = DialogResult.Cancel;
		cancelButton.Name = "cancelButton";
		cancelButton.Click += CancelButtonClick;
		componentResourceManager.ApplyResources(okButton, "okButton");
		okButton.Name = "okButton";
		okButton.Click += OkButtonClick;
		componentResourceManager.ApplyResources(resourceNameTextBox, "resourceNameTextBox");
		resourceNameTextBox.Name = "resourceNameTextBox";
		componentResourceManager.ApplyResources(resourceNameLabel, "resourceNameLabel");
		resourceNameLabel.ForeColor = SystemColors.ControlText;
		resourceNameLabel.Name = "resourceNameLabel";
		browseButton.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(browseButton, "browseButton");
		browseButton.Name = "browseButton";
		browseButton.Click += BrowseButtonClick;
		componentResourceManager.ApplyResources(resourceDllTextBox, "resourceDllTextBox");
		resourceDllTextBox.Name = "resourceDllTextBox";
		componentResourceManager.ApplyResources(resourceDisplayNameTextBox, "resourceDisplayNameTextBox");
		resourceDisplayNameTextBox.Name = "resourceDisplayNameTextBox";
		componentResourceManager.ApplyResources(resourceDisplayNameLabel, "resourceDisplayNameLabel");
		resourceDisplayNameLabel.ForeColor = SystemColors.ControlText;
		resourceDisplayNameLabel.Name = "resourceDisplayNameLabel";
		componentResourceManager.ApplyResources(labelResourcePath, "labelResourcePath");
		labelResourcePath.Name = "labelResourcePath";
		((Form)this).AcceptButton = okButton;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Form)this).CancelButton = cancelButton;
		((Control)this).Controls.Add(labelResourcePath);
		((Control)this).Controls.Add(resourceDisplayNameTextBox);
		((Control)this).Controls.Add(resourceDisplayNameLabel);
		((Control)this).Controls.Add(resourceNameTextBox);
		((Control)this).Controls.Add(resourceNameLabel);
		((Control)this).Controls.Add(browseButton);
		((Control)this).Controls.Add(resourceDllTextBox);
		((Control)this).Controls.Add(okButton);
		((Control)this).Controls.Add(cancelButton);
		((Form)this).FormBorderStyle = FormBorderStyle.FixedDialog;
		((Control)this).Name = "AddResourceTypeDialog";
		((Form)this).Load += AddResourceTypeDialog_Load;
		((Control)this).ResumeLayout(performLayout: false);
		((Control)this).PerformLayout();
	}
}
