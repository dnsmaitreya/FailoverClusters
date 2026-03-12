using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.FailoverClusters.UI.Controls;
using Microsoft.ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal class LinksPanelLinkControl : SnapinUserControl
{
	private IContainer components;

	private PictureBox pictureBox;

	private LinkLabel linkLabel;

	private string url;

	private IContext context;

	private Microsoft.ManagementConsole.View view;

	private string linkText;

	private LinksPanelLinkControl()
	{
		InitializeComponent();
	}

	private LinksPanelLinkControl(Icon icon, string linkText)
		: this()
	{
		this.linkText = linkText;
		WinFormsHelp.SetLinkLabelText(linkLabel, linkText);
		WinFormsHelp.SetPictureBoxImage(pictureBox, icon);
		pictureBox.Cursor = Cursors.Hand;
	}

	private LinksPanelLinkControl(Icon icon, string linkText, string url)
		: this(icon, linkText)
	{
		this.url = url;
		context = null;
		view = null;
	}

	private LinksPanelLinkControl(Icon icon, string linkText, Microsoft.ManagementConsole.View view, IContext context)
		: this(icon, linkText)
	{
		this.view = view;
		this.context = context;
		url = null;
	}

	public override int GetHashCode()
	{
		return linkText.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		return string.Compare(linkText, ((LinksPanelLinkControl)obj).linkText, StringComparison.CurrentCultureIgnoreCase) == 0;
	}

	public static UserControl CreateUrlLink(string linkText, string url)
	{
		return (UserControl)(object)new LinksPanelLinkControl(Icons.OnlineHelp, linkText, url);
	}

	internal static UserControl CreateShortcutLink(Microsoft.ManagementConsole.View view, IContext context, string shortcutText)
	{
		if (string.IsNullOrEmpty(shortcutText))
		{
			shortcutText = context.DisplayName;
		}
		return (UserControl)(object)new LinksPanelLinkControl(Icons.ShortcutArrow, shortcutText, view, context);
	}

	internal bool Compare(string text)
	{
		return string.Compare(text, linkText, StringComparison.OrdinalIgnoreCase) == 0;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		((SnapinUserControl)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(LinksPanelLinkControl));
		pictureBox = new PictureBox();
		linkLabel = new LinkLabel();
		((ISupportInitialize)pictureBox).BeginInit();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(pictureBox, "pictureBox");
		pictureBox.Name = "pictureBox";
		pictureBox.TabStop = false;
		pictureBox.Click += pictureBox_Click;
		componentResourceManager.ApplyResources(linkLabel, "linkLabel");
		linkLabel.AutoEllipsis = true;
		linkLabel.Name = "linkLabel";
		linkLabel.TabStop = true;
		linkLabel.LinkClicked += linkLabel_LinkClicked;
		componentResourceManager.ApplyResources(this, "$this");
		((SnapinUserControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)this).Controls.Add(linkLabel);
		((Control)this).Controls.Add(pictureBox);
		((Control)this).Name = "LinksPanelLinkControl";
		((ISupportInitialize)pictureBox).EndInit();
		((Control)this).ResumeLayout(performLayout: false);
	}

	private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		DoLink();
	}

	private void pictureBox_Click(object sender, EventArgs e)
	{
		DoLink();
	}

	private void DoLink()
	{
		if (context == null)
		{
			UIHelper.ApplicationActivate(Process.Start(url));
			return;
		}
		CluAdminScopeNode cluAdminScopeNode = (CluAdminScopeNode)view.ScopeNode;
		ScopeNode scopeNode = cluAdminScopeNode.FindChildWithExpand(context.DisplayName, cluAdminScopeNode.NotifyUser);
		view.SelectScopeNode(scopeNode);
	}
}
