using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using KDDSL.ServerClusters.Controls;

namespace KDDSL.ServerClusters.Management;

internal class FileShareAdvancedSettingsDialog : SnapinForm
{
	private INotifyUser notifyUser;

	private uint maxUsers;

	private uint shareFlags;

	private bool shareSubDirs;

	private bool hideSubDirShares;

	private IContainer components;

	private SnapinTabControl tabControl;

	private Button cancelButton;

	private Button okButton;

	private TabPage userLimitsPage;

	private TabPage cachingPage;

	private TabPage subdirectoriesPage;

	private RadioButton maximumAllowedButton;

	private Label userLimitLabel;

	private RadioButton allowThisNumberButton;

	private NumericUpDown usersAllowedUpDown;

	private Label userLimitInstructionsLabel;

	private HorizontalLine horizontalLine;

	private CheckBox accessBasedEnumerationCheckbox;

	private Label label1;

	private RadioButton noCachingButton;

	private RadioButton automaticCachingButton;

	private RadioButton manualCachingButton;

	private CheckBox optimizedCachingCheckbox;

	private Label instructionsLabel;

	private CheckBox hideSubdirectoriesCheckBox;

	private CheckBox shareSubdirectoriesCheckBox;

	private Label shareSubdirectoriesLabel;

	private Label hideSubdirectoriesLabel;

	public uint MaxUsers => maxUsers;

	public uint ShareFlags => shareFlags;

	public bool ShareSubDirs => shareSubDirs;

	public bool HideSubDirShares => hideSubDirShares;

	public FileShareAdvancedSettingsDialog(uint maxUsers, uint shareFlags)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		InitializeComponent();
		notifyUser = (INotifyUser)new MessageBoxNotifyUser((IWin32Window)this);
		usersAllowedUpDown.Minimum = 1m;
		if (maxUsers == uint.MaxValue)
		{
			maximumAllowedButton.Checked = true;
			usersAllowedUpDown.Value = usersAllowedUpDown.Minimum;
		}
		else
		{
			allowThisNumberButton.Checked = true;
			usersAllowedUpDown.Value = maxUsers;
		}
		usersAllowedUpDown.Enabled = allowThisNumberButton.Checked;
		accessBasedEnumerationCheckbox.Checked = (shareFlags & 0x800) != 0;
		switch (shareFlags & 0x30)
		{
		case 0u:
			manualCachingButton.Checked = true;
			break;
		case 16u:
			automaticCachingButton.Checked = true;
			break;
		case 32u:
			automaticCachingButton.Checked = true;
			optimizedCachingCheckbox.Checked = true;
			break;
		case 48u:
			noCachingButton.Checked = true;
			break;
		}
		((Control)(object)tabControl).Controls.Remove(subdirectoriesPage);
		this.maxUsers = maxUsers;
		this.shareFlags = shareFlags;
		shareSubDirs = false;
		hideSubDirShares = false;
	}

	public void EnableSubdirectoriesPage(bool shareSubDirectories, bool hideSubDirectories)
	{
		shareSubdirectoriesCheckBox.Checked = shareSubDirectories;
		hideSubdirectoriesCheckBox.Checked = hideSubDirectories;
		hideSubdirectoriesCheckBox.Enabled = shareSubdirectoriesCheckBox.Checked;
		((Control)(object)tabControl).Controls.Add(subdirectoriesPage);
		shareSubDirs = shareSubDirectories;
		hideSubDirShares = hideSubDirectories;
	}

	private void OnUserLimitChanged(object sender, EventArgs e)
	{
		usersAllowedUpDown.Enabled = allowThisNumberButton.Checked;
		if (!usersAllowedUpDown.Enabled)
		{
			usersAllowedUpDown.Value = usersAllowedUpDown.Minimum;
		}
		if (maximumAllowedButton.Checked)
		{
			maxUsers = uint.MaxValue;
		}
		else
		{
			maxUsers = (uint)usersAllowedUpDown.Value;
		}
	}

	private void OnAccessBasedEnumerationChanged(object sender, EventArgs e)
	{
		if (accessBasedEnumerationCheckbox.Checked)
		{
			shareFlags |= 2048u;
		}
		else
		{
			shareFlags &= 4294965247u;
		}
	}

	private void OnCachingChanged(object sender, EventArgs e)
	{
		if (automaticCachingButton.Checked)
		{
			optimizedCachingCheckbox.Enabled = true;
		}
		else
		{
			CheckBox checkBox = optimizedCachingCheckbox;
			bool enabled = (optimizedCachingCheckbox.Checked = false);
			checkBox.Enabled = enabled;
		}
		uint num = 0u;
		num = ((!manualCachingButton.Checked) ? ((!automaticCachingButton.Checked) ? 48u : ((!optimizedCachingCheckbox.Checked) ? 16u : 32u)) : 0u);
		shareFlags = (shareFlags & 0xFFFFFFCFu) | num;
	}

	private void OnSubdirectoriesChanged(object sender, EventArgs e)
	{
		if (shareSubdirectoriesCheckBox.Checked)
		{
			hideSubdirectoriesCheckBox.Enabled = true;
		}
		else
		{
			CheckBox checkBox = hideSubdirectoriesCheckBox;
			bool enabled = (hideSubdirectoriesCheckBox.Checked = false);
			checkBox.Enabled = enabled;
		}
		shareSubDirs = shareSubdirectoriesCheckBox.Checked;
		hideSubDirShares = hideSubdirectoriesCheckBox.Checked;
	}

	private void OnOkButtonClick(object sender, EventArgs e)
	{
		((Form)this).DialogResult = DialogResult.OK;
	}

	private void OnCancelButtonClick(object sender, EventArgs e)
	{
		((Form)this).DialogResult = DialogResult.Cancel;
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
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FileShareAdvancedSettingsDialog));
		tabControl = new SnapinTabControl();
		userLimitsPage = new TabPage();
		accessBasedEnumerationCheckbox = new CheckBox();
		label1 = new Label();
		horizontalLine = new HorizontalLine();
		userLimitInstructionsLabel = new Label();
		maximumAllowedButton = new RadioButton();
		userLimitLabel = new Label();
		allowThisNumberButton = new RadioButton();
		usersAllowedUpDown = new NumericUpDown();
		cachingPage = new TabPage();
		noCachingButton = new RadioButton();
		automaticCachingButton = new RadioButton();
		manualCachingButton = new RadioButton();
		optimizedCachingCheckbox = new CheckBox();
		instructionsLabel = new Label();
		subdirectoriesPage = new TabPage();
		hideSubdirectoriesLabel = new Label();
		hideSubdirectoriesCheckBox = new CheckBox();
		shareSubdirectoriesCheckBox = new CheckBox();
		shareSubdirectoriesLabel = new Label();
		cancelButton = new Button();
		okButton = new Button();
		((Control)(object)tabControl).SuspendLayout();
		userLimitsPage.SuspendLayout();
		((ISupportInitialize)usersAllowedUpDown).BeginInit();
		cachingPage.SuspendLayout();
		subdirectoriesPage.SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tabControl, "tabControl");
		((Control)(object)tabControl).Controls.Add(userLimitsPage);
		((Control)(object)tabControl).Controls.Add(cachingPage);
		((Control)(object)tabControl).Controls.Add(subdirectoriesPage);
		((Control)(object)tabControl).Name = "tabControl";
		((TabControl)(object)tabControl).SelectedIndex = 0;
		userLimitsPage.Controls.Add(accessBasedEnumerationCheckbox);
		userLimitsPage.Controls.Add(label1);
		userLimitsPage.Controls.Add((Control)(object)horizontalLine);
		userLimitsPage.Controls.Add(userLimitInstructionsLabel);
		userLimitsPage.Controls.Add(maximumAllowedButton);
		userLimitsPage.Controls.Add(userLimitLabel);
		userLimitsPage.Controls.Add(allowThisNumberButton);
		userLimitsPage.Controls.Add(usersAllowedUpDown);
		componentResourceManager.ApplyResources(userLimitsPage, "userLimitsPage");
		userLimitsPage.Name = "userLimitsPage";
		componentResourceManager.ApplyResources(accessBasedEnumerationCheckbox, "accessBasedEnumerationCheckbox");
		accessBasedEnumerationCheckbox.Name = "accessBasedEnumerationCheckbox";
		accessBasedEnumerationCheckbox.UseVisualStyleBackColor = true;
		accessBasedEnumerationCheckbox.CheckedChanged += OnAccessBasedEnumerationChanged;
		componentResourceManager.ApplyResources(label1, "label1");
		label1.AutoEllipsis = true;
		label1.ForeColor = SystemColors.ControlText;
		label1.Name = "label1";
		componentResourceManager.ApplyResources(horizontalLine, "horizontalLine");
		((Control)(object)horizontalLine).Name = "horizontalLine";
		componentResourceManager.ApplyResources(userLimitInstructionsLabel, "userLimitInstructionsLabel");
		userLimitInstructionsLabel.AutoEllipsis = true;
		userLimitInstructionsLabel.ForeColor = SystemColors.ControlText;
		userLimitInstructionsLabel.Name = "userLimitInstructionsLabel";
		maximumAllowedButton.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(maximumAllowedButton, "maximumAllowedButton");
		maximumAllowedButton.Name = "maximumAllowedButton";
		maximumAllowedButton.CheckedChanged += OnUserLimitChanged;
		userLimitLabel.AutoEllipsis = true;
		userLimitLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(userLimitLabel, "userLimitLabel");
		userLimitLabel.Name = "userLimitLabel";
		allowThisNumberButton.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(allowThisNumberButton, "allowThisNumberButton");
		allowThisNumberButton.Name = "allowThisNumberButton";
		allowThisNumberButton.CheckedChanged += OnUserLimitChanged;
		componentResourceManager.ApplyResources(usersAllowedUpDown, "usersAllowedUpDown");
		usersAllowedUpDown.Maximum = new decimal(new int[4] { -1, 0, 0, 0 });
		usersAllowedUpDown.Name = "usersAllowedUpDown";
		usersAllowedUpDown.ValueChanged += OnUserLimitChanged;
		cachingPage.Controls.Add(noCachingButton);
		cachingPage.Controls.Add(automaticCachingButton);
		cachingPage.Controls.Add(manualCachingButton);
		cachingPage.Controls.Add(optimizedCachingCheckbox);
		cachingPage.Controls.Add(instructionsLabel);
		componentResourceManager.ApplyResources(cachingPage, "cachingPage");
		cachingPage.Name = "cachingPage";
		componentResourceManager.ApplyResources(noCachingButton, "noCachingButton");
		noCachingButton.Name = "noCachingButton";
		noCachingButton.CheckedChanged += OnCachingChanged;
		componentResourceManager.ApplyResources(automaticCachingButton, "automaticCachingButton");
		automaticCachingButton.Name = "automaticCachingButton";
		automaticCachingButton.CheckedChanged += OnCachingChanged;
		componentResourceManager.ApplyResources(manualCachingButton, "manualCachingButton");
		manualCachingButton.Name = "manualCachingButton";
		manualCachingButton.CheckedChanged += OnCachingChanged;
		componentResourceManager.ApplyResources(optimizedCachingCheckbox, "optimizedCachingCheckbox");
		optimizedCachingCheckbox.Name = "optimizedCachingCheckbox";
		optimizedCachingCheckbox.CheckedChanged += OnCachingChanged;
		componentResourceManager.ApplyResources(instructionsLabel, "instructionsLabel");
		instructionsLabel.Name = "instructionsLabel";
		subdirectoriesPage.Controls.Add(hideSubdirectoriesLabel);
		subdirectoriesPage.Controls.Add(hideSubdirectoriesCheckBox);
		subdirectoriesPage.Controls.Add(shareSubdirectoriesCheckBox);
		subdirectoriesPage.Controls.Add(shareSubdirectoriesLabel);
		componentResourceManager.ApplyResources(subdirectoriesPage, "subdirectoriesPage");
		subdirectoriesPage.Name = "subdirectoriesPage";
		componentResourceManager.ApplyResources(hideSubdirectoriesLabel, "hideSubdirectoriesLabel");
		hideSubdirectoriesLabel.Name = "hideSubdirectoriesLabel";
		componentResourceManager.ApplyResources(hideSubdirectoriesCheckBox, "hideSubdirectoriesCheckBox");
		hideSubdirectoriesCheckBox.Name = "hideSubdirectoriesCheckBox";
		hideSubdirectoriesCheckBox.CheckedChanged += OnSubdirectoriesChanged;
		componentResourceManager.ApplyResources(shareSubdirectoriesCheckBox, "shareSubdirectoriesCheckBox");
		shareSubdirectoriesCheckBox.Name = "shareSubdirectoriesCheckBox";
		shareSubdirectoriesCheckBox.CheckedChanged += OnSubdirectoriesChanged;
		componentResourceManager.ApplyResources(shareSubdirectoriesLabel, "shareSubdirectoriesLabel");
		shareSubdirectoriesLabel.Name = "shareSubdirectoriesLabel";
		componentResourceManager.ApplyResources(cancelButton, "cancelButton");
		cancelButton.DialogResult = DialogResult.Cancel;
		cancelButton.Name = "cancelButton";
		cancelButton.Click += OnCancelButtonClick;
		componentResourceManager.ApplyResources(okButton, "okButton");
		okButton.Name = "okButton";
		okButton.Click += OnOkButtonClick;
		((Form)this).AcceptButton = okButton;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Form)this).CancelButton = cancelButton;
		((Control)this).Controls.Add(okButton);
		((Control)this).Controls.Add(cancelButton);
		((Control)this).Controls.Add((Control)(object)tabControl);
		((Control)this).Name = "FileShareAdvancedSettingsDialog";
		((Control)(object)tabControl).ResumeLayout(performLayout: false);
		userLimitsPage.ResumeLayout(performLayout: false);
		((ISupportInitialize)usersAllowedUpDown).EndInit();
		cachingPage.ResumeLayout(performLayout: false);
		subdirectoriesPage.ResumeLayout(performLayout: false);
		subdirectoriesPage.PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
