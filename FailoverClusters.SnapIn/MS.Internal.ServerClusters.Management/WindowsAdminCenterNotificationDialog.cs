using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace MS.Internal.ServerClusters.Management;

public class WindowsAdminCenterNotificationDialog : Form
{
	private static readonly string settingsValueName = "{80DF3188-A4CB-4A33-8E7E-DFEEF9D944E3}";

	private IContainer components;

	private CheckBox doNotShowThisAgain;

	private Label greetinglabel;

	private LinkLabel linkLabel;

	private Label announcementLabel;

	public WindowsAdminCenterNotificationDialog()
	{
		InitializeComponent();
	}

	internal static bool ShouldShow()
	{
		return !UserSettings.GetBoolValue(settingsValueName);
	}

	private void OnDoNotShowAgainCheckedChanged(object sender, EventArgs e)
	{
		UserSettings.SetBoolValue(settingsValueName, doNotShowThisAgain.Checked);
	}

	private void OnCloseButtonClicked(object sender, EventArgs e)
	{
		Close();
	}

	private void OnLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		UIHelper.ApplicationActivate(Process.Start(((LinkLabel)sender).Tag.ToString()));
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (keyData == Keys.Escape)
		{
			Close();
			return true;
		}
		return base.ProcessCmdKey(ref msg, keyData);
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
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(MS.Internal.ServerClusters.Management.WindowsAdminCenterNotificationDialog));
		this.doNotShowThisAgain = new System.Windows.Forms.CheckBox();
		this.greetinglabel = new System.Windows.Forms.Label();
		this.linkLabel = new System.Windows.Forms.LinkLabel();
		this.announcementLabel = new System.Windows.Forms.Label();
		base.SuspendLayout();
		componentResourceManager.ApplyResources(this.doNotShowThisAgain, "doNotShowThisAgain");
		this.doNotShowThisAgain.Name = "doNotShowThisAgain";
		this.doNotShowThisAgain.UseVisualStyleBackColor = true;
		this.doNotShowThisAgain.CheckedChanged += new System.EventHandler(OnDoNotShowAgainCheckedChanged);
		componentResourceManager.ApplyResources(this.greetinglabel, "greetinglabel");
		this.greetinglabel.Name = "greetinglabel";
		componentResourceManager.ApplyResources(this.linkLabel, "linkLabel");
		this.linkLabel.Name = "linkLabel";
		this.linkLabel.TabStop = true;
		this.linkLabel.Tag = "https://go.com/fwlink/?linkid=872972";
		this.linkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(OnLinkClicked);
		componentResourceManager.ApplyResources(this.announcementLabel, "announcementLabel");
		this.announcementLabel.Name = "announcementLabel";
		componentResourceManager.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.SystemColors.Window;
		base.Controls.Add(this.announcementLabel);
		base.Controls.Add(this.linkLabel);
		base.Controls.Add(this.greetinglabel);
		base.Controls.Add(this.doNotShowThisAgain);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "WindowsAdminCenterNotificationDialog";
		base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
		base.ResumeLayout(false);
	}
}

