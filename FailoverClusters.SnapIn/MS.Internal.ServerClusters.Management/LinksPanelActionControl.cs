using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FailoverClusters.UI.Controls;
using ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal class LinksPanelActionControl : SnapinUserControl
{
	private IContainer components;

	private PictureBox actionPictureBox;

	private LinkLabel actionLinkLabel;

	private ActionBase action;

	private ManagementConsole.View view;

	private LinksPanelActionControl()
	{
		InitializeComponent();
	}

	private LinksPanelActionControl(ManagementConsole.View view, ActionBase action)
		: this()
	{
		this.action = action;
		if (this.action != null)
		{
			this.view = view;
			WinFormsHelp.SetLinkLabelText(actionLinkLabel, this.action.DisplayName);
		}
		else
		{
			actionLinkLabel.Visible = false;
			actionPictureBox.Visible = false;
		}
		SetupPictureBox(actionPictureBox, Icons.GreenArrow);
		UpdateHeight();
	}

	private void SetupPictureBox(PictureBox pictureBox, Icon icon)
	{
		WinFormsHelp.SetPictureBoxImage(pictureBox, icon);
		pictureBox.Cursor = Cursors.Hand;
	}

	public static UserControl CreateActionLink(ManagementConsole.View view, ActionBase action)
	{
		ActionData.GetActionData(action);
		return (UserControl)(object)new LinksPanelActionControl(view, action);
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(LinksPanelActionControl));
		actionPictureBox = new PictureBox();
		actionLinkLabel = new LinkLabel();
		((ISupportInitialize)actionPictureBox).BeginInit();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(actionPictureBox, "actionPictureBox");
		actionPictureBox.Name = "actionPictureBox";
		actionPictureBox.TabStop = false;
		actionPictureBox.Click += actionPictureBox_Click;
		actionLinkLabel.AutoEllipsis = true;
		componentResourceManager.ApplyResources(actionLinkLabel, "actionLinkLabel");
		actionLinkLabel.Name = "actionLinkLabel";
		actionLinkLabel.TabStop = true;
		actionLinkLabel.LinkClicked += actionLinkLabel_LinkClicked;
		componentResourceManager.ApplyResources(this, "$this");
		((SnapinUserControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)this).Controls.Add(actionLinkLabel);
		((Control)this).Controls.Add(actionPictureBox);
		((Control)this).Name = "LinksPanelActionControl";
		((Control)this).SizeChanged += LinksPanelActionControl_SizeChanged;
		((ISupportInitialize)actionPictureBox).EndInit();
		((Control)this).ResumeLayout(performLayout: false);
	}

	private void actionLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		PerformAction();
	}

	private void actionPictureBox_Click(object sender, EventArgs e)
	{
		PerformAction();
	}

	private void PerformAction()
	{
		ActionData.GetActionData(action).PerformAction(view);
	}

	private void LinksPanelActionControl_SizeChanged(object sender, EventArgs e)
	{
		UpdateHeight();
	}

	private void UpdateHeight()
	{
		UpdateLabelHeight(actionLinkLabel);
		((Control)this).Height = actionLinkLabel.Height;
	}

	private void UpdateLabelHeight(Label label)
	{
		label.Height = TextRenderer.MeasureText(label.Text, label.Font, new Size(label.Width, int.MaxValue), TextFormatFlags.WordBreak).Height;
	}
}

