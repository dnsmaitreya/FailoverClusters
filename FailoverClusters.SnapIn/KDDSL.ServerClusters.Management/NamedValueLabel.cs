using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using FailoverClusters.UI.Controls;

namespace KDDSL.ServerClusters.Management;

internal class NamedValueLabel : SnapinUserControl
{
	private IContainer components;

	private Label nameLabel;

	private PictureBox pictureBox;

	private LinkLabel valueLabel;

	private string name;

	private TableLayoutPanel tableLayoutPanel;

	private bool enableLink;

	private string nameFormat;

	private static Color? defaultLinkColor;

	[Browsable(true)]
	[Localizable(true)]
	public string DataName
	{
		get
		{
			UIThreadHandler<string> val = () => DataName;
			string empty = string.Empty;
			if (UIHelper.ExecuteOnUIThread<string>(ref empty, (ISynchronizeInvoke)this, (Delegate)(object)val, Array.Empty<object>()))
			{
				return empty;
			}
			return name;
		}
		set
		{
			UIThreadHandlerV<string> val2 = delegate
			{
				DataName = value;
			};
			if (!UIHelper.ExecuteOnUIThread<string>((ISynchronizeInvoke)this, (Delegate)(object)val2, value))
			{
				name = value;
				SetNameLabel();
			}
		}
	}

	[Browsable(true)]
	[Localizable(true)]
	public string DataValue
	{
		get
		{
			UIThreadHandler<string> val = () => DataValue;
			string empty = string.Empty;
			if (UIHelper.ExecuteOnUIThread<string>(ref empty, (ISynchronizeInvoke)this, (Delegate)(object)val, Array.Empty<object>()))
			{
				return empty;
			}
			return valueLabel.Text;
		}
		set
		{
			UIThreadHandlerV<string> val2 = delegate
			{
				DataValue = value;
			};
			if (!UIHelper.ExecuteOnUIThread<string>((ISynchronizeInvoke)this, (Delegate)(object)val2, value))
			{
				valueLabel.Text = value;
				SetLink();
			}
		}
	}

	[Browsable(true)]
	public bool UseBoldFontForName
	{
		get
		{
			UIThreadHandler<bool> val = () => UseBoldFontForName;
			bool result = false;
			if (UIHelper.ExecuteOnUIThread<bool>(ref result, (ISynchronizeInvoke)this, (Delegate)(object)val, Array.Empty<object>()))
			{
				return result;
			}
			return nameLabel.Font.Bold;
		}
		set
		{
			UIThreadHandlerV<bool> val2 = delegate
			{
				UseBoldFontForName = value;
			};
			if (!UIHelper.ExecuteOnUIThread<bool>((ISynchronizeInvoke)this, (Delegate)(object)val2, value) && value != nameLabel.Font.Bold)
			{
				nameLabel.Font = new Font(nameLabel.Font, value ? FontStyle.Bold : FontStyle.Regular);
			}
		}
	}

	[Browsable(true)]
	public bool EnableLink
	{
		get
		{
			UIThreadHandler<bool> val = () => EnableLink;
			bool result = false;
			if (UIHelper.ExecuteOnUIThread<bool>(ref result, (ISynchronizeInvoke)this, (Delegate)(object)val, Array.Empty<object>()))
			{
				return result;
			}
			return enableLink;
		}
		set
		{
			UIThreadHandlerV<bool> val2 = delegate
			{
				EnableLink = value;
			};
			if (!UIHelper.ExecuteOnUIThread<bool>((ISynchronizeInvoke)this, (Delegate)(object)val2, value) && value != enableLink)
			{
				enableLink = value;
				SetLink();
			}
		}
	}

	public event EventHandler LinkClicked;

	public NamedValueLabel()
	{
		((Control)this).SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
		InitializeComponent();
		valueLabel.Links.Clear();
		((Control)this).TabStop = false;
		nameFormat = Resources.NameLabelFormat_Text;
	}

	public int GetRequiredWidth()
	{
		UIThreadHandler<int> val = GetRequiredWidth;
		int result = 0;
		if (UIHelper.ExecuteOnUIThread<int>(ref result, (ISynchronizeInvoke)this, (Delegate)(object)val, Array.Empty<object>()))
		{
			return result;
		}
		return nameLabel.Width + pictureBox.Width + TextRenderer.MeasureText(valueLabel.Text, valueLabel.Font).Width + ((Control)this).Padding.Right + ((Control)this).Padding.Left;
	}

	public void ShowIcon(Icon icon)
	{
		UIThreadHandlerV<Icon> val = ShowIcon;
		if (!UIHelper.ExecuteOnUIThread<Icon>((ISynchronizeInvoke)this, (Delegate)(object)val, icon))
		{
			WinFormsHelp.SetPictureBoxImage(pictureBox, icon);
			pictureBox.Visible = true;
		}
	}

	public void HideIcon()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		UIThreadHandlerV val = new UIThreadHandlerV(HideIcon);
		if (!UIHelper.ExecuteOnUIThread((ISynchronizeInvoke)this, (Delegate)(object)val, Array.Empty<object>()))
		{
			pictureBox.Visible = false;
		}
	}

	private void SetLink()
	{
		if (enableLink)
		{
			if (valueLabel.Links.Count == 0 || valueLabel.Links[0].Length != valueLabel.Text.Length)
			{
				valueLabel.Links.Clear();
				valueLabel.Links.Add(0, valueLabel.Text.Length);
				pictureBox.Cursor = Cursors.Hand;
			}
		}
		else
		{
			valueLabel.Links.Clear();
			pictureBox.Cursor = Cursors.Default;
		}
		((Control)this).TabStop = enableLink;
	}

	private void OnLinkClicked()
	{
		if (enableLink)
		{
			this.LinkClicked?.Invoke(this, EventArgs.Empty);
		}
	}

	private void ValueLabelClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		OnLinkClicked();
	}

	private void SetNameLabel()
	{
		if (name != null && name.Length > 0)
		{
			nameLabel.Text = string.Format(CultureInfo.CurrentCulture, nameFormat, name);
		}
		else
		{
			nameLabel.Text = string.Empty;
		}
	}

	private void NamedValueLabel_SizeChanged(object sender, EventArgs e)
	{
		if (!UIHelper.DesignMode)
		{
			((Control)(object)this).MinimumSize = new Size(valueLabel.Location.X + 4, ((Control)(object)this).MinimumSize.Height);
		}
	}

	private void pictureBox_Click(object sender, EventArgs e)
	{
		OnLinkClicked();
	}

	private static Color GetDefaultLinkColor()
	{
		if (!defaultLinkColor.HasValue)
		{
			defaultLinkColor = new LinkLabel().LinkColor;
		}
		return defaultLinkColor.Value;
	}

	public void RestoreDefaultColors()
	{
		if (!UIHelper.DesignMode)
		{
			valueLabel.LinkColor = GetDefaultLinkColor();
		}
	}

	protected override void OnCreateControl()
	{
		((UserControl)this).OnCreateControl();
		if (!UIHelper.DesignMode)
		{
			valueLabel.LinkColor = (SystemInformation.HighContrast ? SystemColors.ControlText : Color.White);
			nameLabel.Font = new Font(ClusterAdministrator.DefaultSystemFontName, ClusterAdministrator.DefaultSystemFontSize, FontStyle.Bold);
			valueLabel.Font = new Font(ClusterAdministrator.DefaultSystemFontName, ClusterAdministrator.DefaultSystemFontSize);
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(NamedValueLabel));
		nameLabel = new Label();
		pictureBox = new PictureBox();
		valueLabel = new LinkLabel();
		tableLayoutPanel = new TableLayoutPanel();
		((ISupportInitialize)pictureBox).BeginInit();
		tableLayoutPanel.SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(nameLabel, "nameLabel");
		nameLabel.Name = "nameLabel";
		componentResourceManager.ApplyResources(pictureBox, "pictureBox");
		pictureBox.Name = "pictureBox";
		pictureBox.TabStop = false;
		pictureBox.Click += pictureBox_Click;
		valueLabel.AutoEllipsis = true;
		componentResourceManager.ApplyResources(valueLabel, "valueLabel");
		valueLabel.LinkBehavior = LinkBehavior.HoverUnderline;
		valueLabel.Name = "valueLabel";
		valueLabel.LinkClicked += ValueLabelClicked;
		componentResourceManager.ApplyResources(tableLayoutPanel, "tableLayoutPanel");
		tableLayoutPanel.Controls.Add(nameLabel, 0, 0);
		tableLayoutPanel.Controls.Add(valueLabel, 2, 0);
		tableLayoutPanel.Controls.Add(pictureBox, 1, 0);
		tableLayoutPanel.Name = "tableLayoutPanel";
		((Control)this).Controls.Add(tableLayoutPanel);
		((Control)(object)this).MinimumSize = new Size(10, 18);
		((Control)this).Name = "NamedValueLabel";
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).SizeChanged += NamedValueLabel_SizeChanged;
		((ISupportInitialize)pictureBox).EndInit();
		tableLayoutPanel.ResumeLayout(performLayout: false);
		tableLayoutPanel.PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}

