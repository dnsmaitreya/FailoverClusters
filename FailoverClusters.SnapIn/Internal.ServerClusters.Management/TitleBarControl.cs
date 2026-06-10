using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MS.Internal.ServerClusters.Management;

internal class TitleBarControl : SnapinUserControl
{
	internal class LabelEx : Label
	{
		public LabelEx()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			if (!base.DesignMode)
			{
				Font = new Font(ClusterAdministrator.DefaultSystemFontName, ClusterAdministrator.DefaultSystemFontSize + 1f, FontStyle.Bold);
			}
		}
	}

	internal class TableLayoutPanelEx : TableLayoutPanel
	{
		public TableLayoutPanelEx()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
		}
	}

	private LabelEx titleLabel;

	private LabelEx subTitleLabel;

	private IContainer components;

	private TableLayoutPanelEx tableLayoutPanel;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Color BackColor
	{
		get
		{
			return ((Control)this).BackColor;
		}
		set
		{
			((Control)this).BackColor = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Color ForeColor
	{
		get
		{
			return ((Control)this).ForeColor;
		}
		set
		{
			((Control)this).ForeColor = value;
		}
	}

	[Browsable(true)]
	[Localizable(true)]
	public string Title
	{
		get
		{
			return titleLabel.Text;
		}
		set
		{
			titleLabel.Text = value;
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
		}
	}

	public TitleBarControl()
	{
		((Control)this).SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
		InitializeComponent();
	}

	protected override void OnCreateControl()
	{
		((UserControl)this).OnCreateControl();
		if (!UIHelper.DesignMode)
		{
			BackColor = (SystemInformation.HighContrast ? SystemColors.ControlDark : ColorTranslator.FromHtml("#414141"));
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(TitleBarControl));
		tableLayoutPanel = new TableLayoutPanelEx();
		titleLabel = new LabelEx();
		subTitleLabel = new LabelEx();
		tableLayoutPanel.SuspendLayout();
		((Control)this).SuspendLayout();
		tableLayoutPanel.BackColor = Color.Transparent;
		componentResourceManager.ApplyResources(tableLayoutPanel, "tableLayoutPanel");
		tableLayoutPanel.Controls.Add(titleLabel, 0, 0);
		tableLayoutPanel.Controls.Add(subTitleLabel, 1, 0);
		tableLayoutPanel.Name = "tableLayoutPanel";
		titleLabel.AutoEllipsis = true;
		componentResourceManager.ApplyResources(titleLabel, "titleLabel");
		titleLabel.ForeColor = SystemColors.ControlLightLight;
		titleLabel.Name = "titleLabel";
		subTitleLabel.AutoEllipsis = true;
		componentResourceManager.ApplyResources(subTitleLabel, "subTitleLabel");
		subTitleLabel.ForeColor = SystemColors.ControlLightLight;
		subTitleLabel.MinimumSize = new Size(10, 0);
		subTitleLabel.Name = "subTitleLabel";
		BackColor = SystemColors.ControlDark;
		((Control)this).Controls.Add(tableLayoutPanel);
		((Control)(object)this).MinimumSize = new Size(64, 27);
		((Control)this).Name = "TitleBarControl";
		componentResourceManager.ApplyResources(this, "$this");
		tableLayoutPanel.ResumeLayout(performLayout: false);
		tableLayoutPanel.PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
