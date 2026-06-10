using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FailoverClusters.UI.Common;

namespace MS.Internal.ServerClusters.Management;

internal class DriveLetterDialog : SnapinForm
{
	private static string[] driveLetters = new string[26]
	{
		"A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
		"K", "L", "M", "N", "O", "P", "Q", "R", "S", "T",
		"U", "V", "W", "X", "Y", "Z"
	};

	private int currentDriveLetterIndex;

	private IContainer components;

	private Button cancelButton;

	private Button okButton;

	private Label selectLabel;

	private ComboBox driveLetterComboBox;

	private TableLayoutPanel mainTableLayoutPanel;

	private Panel newDriveLetterPanel;

	private Label diskAssignedToRoleWarningText;

	private Panel warningPanel;

	private PictureBox warningIcon;

	public uint DriveLetterMask
	{
		get
		{
			if (driveLetterComboBox.SelectedIndex == 0)
			{
				return 0u;
			}
			for (int i = 0; i < driveLetters.Length; i++)
			{
				if (driveLetters[i] == SelectedDriveLetter)
				{
					return (uint)(1 << i);
				}
			}
			return 0u;
		}
	}

	public string SelectedDriveLetter => (string)driveLetterComboBox.SelectedItem;

	private uint CurrentDriveLetterMask { get; set; }

	private uint AvailableDriveLettersMask { get; set; }

	private ClusterGroup ClusterGroup { get; set; }

	public DriveLetterDialog()
	{
		InitializeComponent();
	}

	public DriveLetterDialog(ClusterGroup clusterGroup, uint currentDriveLetterMask, uint availableDriveLettersMask)
		: this()
	{
		ClusterGroup = clusterGroup;
		CurrentDriveLetterMask = currentDriveLetterMask;
		AvailableDriveLettersMask = availableDriveLettersMask;
	}

	protected override void OnCreateControl()
	{
		((Form)this).OnCreateControl();
		uint num = 1u;
		driveLetterComboBox.Items.Add(Resources.None_Text);
		for (int i = 0; i < driveLetters.Length; i++)
		{
			if ((AvailableDriveLettersMask & num) != 0 || CurrentDriveLetterMask == num)
			{
				int selectedIndex = driveLetterComboBox.Items.Add(driveLetters[i]);
				if (CurrentDriveLetterMask == num)
				{
					driveLetterComboBox.SelectedIndex = selectedIndex;
				}
			}
			num <<= 1;
		}
		if (driveLetterComboBox.SelectedIndex == -1)
		{
			driveLetterComboBox.SelectedIndex = 0;
		}
		currentDriveLetterIndex = driveLetterComboBox.SelectedIndex;
		okButton.Enabled = false;
		diskAssignedToRoleWarningText.Text = Resources.ChangeDriveLetter_Text;
		warningIcon.Image = InvariantResources.Warning.ToBitmap();
		if (ClusterGroup != null)
		{
			GroupType groupType = GroupType.Unknown;
			using (CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.RetrievingItem_Text, string.Empty))
			{
				cluadminWaitDialog.ShowDialog(ClusterAdministrator.NotifyUser, delegate
				{
					groupType = ClusterGroup.GroupType;
				});
				if (cluadminWaitDialog.IsCanceled)
				{
					return;
				}
			}
			if (groupType != GroupType.AvailableStorage)
			{
				mainTableLayoutPanel.RowStyles[1].SizeType = SizeType.AutoSize;
			}
		}
		else
		{
			mainTableLayoutPanel.RowStyles[1].SizeType = SizeType.Absolute;
			mainTableLayoutPanel.RowStyles[1].Height = 0f;
		}
	}

	private void DriveLetterComboBoxSelectedIndexChanged(object sender, EventArgs e)
	{
		okButton.Enabled = driveLetterComboBox.SelectedIndex != currentDriveLetterIndex;
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DriveLetterDialog));
		cancelButton = new Button();
		okButton = new Button();
		selectLabel = new Label();
		driveLetterComboBox = new ComboBox();
		mainTableLayoutPanel = new TableLayoutPanel();
		newDriveLetterPanel = new Panel();
		warningPanel = new Panel();
		warningIcon = new PictureBox();
		diskAssignedToRoleWarningText = new Label();
		mainTableLayoutPanel.SuspendLayout();
		newDriveLetterPanel.SuspendLayout();
		warningPanel.SuspendLayout();
		((ISupportInitialize)warningIcon).BeginInit();
		((Control)this).SuspendLayout();
		cancelButton.DialogResult = DialogResult.Cancel;
		componentResourceManager.ApplyResources(cancelButton, "cancelButton");
		cancelButton.Name = "cancelButton";
		okButton.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(okButton, "okButton");
		okButton.Name = "okButton";
		componentResourceManager.ApplyResources(selectLabel, "selectLabel");
		selectLabel.ForeColor = SystemColors.ControlText;
		selectLabel.Name = "selectLabel";
		driveLetterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
		driveLetterComboBox.FormattingEnabled = true;
		componentResourceManager.ApplyResources(driveLetterComboBox, "driveLetterComboBox");
		driveLetterComboBox.Name = "driveLetterComboBox";
		driveLetterComboBox.SelectedIndexChanged += DriveLetterComboBoxSelectedIndexChanged;
		componentResourceManager.ApplyResources(mainTableLayoutPanel, "mainTableLayoutPanel");
		mainTableLayoutPanel.Controls.Add(newDriveLetterPanel, 1, 2);
		mainTableLayoutPanel.Controls.Add(warningPanel, 1, 1);
		mainTableLayoutPanel.Name = "mainTableLayoutPanel";
		componentResourceManager.ApplyResources(newDriveLetterPanel, "newDriveLetterPanel");
		newDriveLetterPanel.Controls.Add(cancelButton);
		newDriveLetterPanel.Controls.Add(okButton);
		newDriveLetterPanel.Controls.Add(driveLetterComboBox);
		newDriveLetterPanel.Controls.Add(selectLabel);
		newDriveLetterPanel.Name = "newDriveLetterPanel";
		componentResourceManager.ApplyResources(warningPanel, "warningPanel");
		warningPanel.Controls.Add(warningIcon);
		warningPanel.Controls.Add(diskAssignedToRoleWarningText);
		warningPanel.Name = "warningPanel";
		componentResourceManager.ApplyResources(warningIcon, "warningIcon");
		warningIcon.Name = "warningIcon";
		warningIcon.TabStop = false;
		componentResourceManager.ApplyResources(diskAssignedToRoleWarningText, "diskAssignedToRoleWarningText");
		diskAssignedToRoleWarningText.Name = "diskAssignedToRoleWarningText";
		((Form)this).AcceptButton = okButton;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Form)this).CancelButton = cancelButton;
		((Control)this).Controls.Add(mainTableLayoutPanel);
		((Form)this).FormBorderStyle = FormBorderStyle.FixedDialog;
		((Control)this).Name = "DriveLetterDialog";
		mainTableLayoutPanel.ResumeLayout(performLayout: false);
		mainTableLayoutPanel.PerformLayout();
		newDriveLetterPanel.ResumeLayout(performLayout: false);
		newDriveLetterPanel.PerformLayout();
		warningPanel.ResumeLayout(performLayout: false);
		((ISupportInitialize)warningIcon).EndInit();
		((Control)this).ResumeLayout(performLayout: false);
		((Control)this).PerformLayout();
	}
}

