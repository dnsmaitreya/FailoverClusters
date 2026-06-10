using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace KDDSL.ServerClusters.Management;

internal class SummaryTitleControl : SnapinUserControl
{
	private PictureBox icon;

	private LinkLabel titleLinkLabel;

	private IContainer components;

	private Label subTitleLabel;

	private bool enableLink;

	[Browsable(true)]
	public bool EnableLink
	{
		get
		{
			return enableLink;
		}
		set
		{
			enableLink = value;
			((Control)this).TabStop = value;
			if (enableLink)
			{
				titleLinkLabel.LinkArea = new LinkArea(0, titleLinkLabel.Text.Length);
				titleLinkLabel.LinkBehavior = LinkBehavior.HoverUnderline;
				icon.Cursor = Cursors.Hand;
			}
			else
			{
				titleLinkLabel.LinkArea = new LinkArea(0, 0);
				titleLinkLabel.LinkBehavior = LinkBehavior.NeverUnderline;
				icon.Cursor = Cursors.Default;
			}
		}
	}

	[Browsable(true)]
	[Localizable(true)]
	public string Title
	{
		get
		{
			return titleLinkLabel.Text;
		}
		set
		{
			titleLinkLabel.Text = value;
			if (enableLink)
			{
				titleLinkLabel.LinkArea = new LinkArea(0, titleLinkLabel.Text.Length);
			}
		}
	}

	[Browsable(true)]
	[Localizable(true)]
	public string SubTitle
	{
		get
		{
			return subTitleLabel.Text;
		}
		set
		{
			subTitleLabel.Text = value;
			subTitleLabel.Visible = !string.IsNullOrEmpty(subTitleLabel.Text);
		}
	}

	[Browsable(true)]
	[Localizable(true)]
	public Image Icon
	{
		get
		{
			return icon.Image;
		}
		set
		{
			icon.Image = value;
		}
	}

	internal event EventHandler Clicked;

	public SummaryTitleControl()
	{
		InitializeComponent();
		EnableLink = false;
	}

	private void LinkClicked(object sender, EventArgs e)
	{
		this.Clicked?.Invoke(this, EventArgs.Empty);
	}

	private void TitleLinkLabel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Space)
		{
			LinkClicked(sender, e);
		}
	}

	protected override void OnCreateControl()
	{
		((UserControl)this).OnCreateControl();
		if (!UIHelper.DesignMode)
		{
			titleLinkLabel.ForeColor = (SystemInformation.HighContrast ? SystemColors.ControlDark : ColorTranslator.FromHtml("#414141"));
		}
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SummaryTitleControl));
		icon = new PictureBox();
		titleLinkLabel = new LinkLabel();
		subTitleLabel = new Label();
		((ISupportInitialize)icon).BeginInit();
		((Control)this).SuspendLayout();
		icon.Cursor = Cursors.Default;
		componentResourceManager.ApplyResources(icon, "icon");
		icon.Name = "icon";
		icon.TabStop = false;
		icon.Click += LinkClicked;
		componentResourceManager.ApplyResources(titleLinkLabel, "titleLinkLabel");
		titleLinkLabel.AutoEllipsis = true;
		titleLinkLabel.Name = "titleLinkLabel";
		titleLinkLabel.TabStop = true;
		titleLinkLabel.UseMnemonic = false;
		titleLinkLabel.Click += LinkClicked;
		titleLinkLabel.PreviewKeyDown += TitleLinkLabel_PreviewKeyDown;
		componentResourceManager.ApplyResources(subTitleLabel, "subTitleLabel");
		subTitleLabel.Name = "subTitleLabel";
		((Control)this).Controls.Add(titleLinkLabel);
		((Control)this).Controls.Add(subTitleLabel);
		((Control)this).Controls.Add(icon);
		((Control)(object)this).MinimumSize = new Size(100, 56);
		((Control)this).Name = "SummaryTitleControl";
		componentResourceManager.ApplyResources(this, "$this");
		((ISupportInitialize)icon).EndInit();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
