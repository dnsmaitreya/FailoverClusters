using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MS.Internal.ServerClusters.Management;

internal class SeparatorComboBox : ComboBox
{
	internal class SeparatorItem
	{
		private object data;

		internal SeparatorItem(object data)
		{
			this.data = data;
		}

		public override string ToString()
		{
			string text = null;
			if (data != null)
			{
				return data.ToString();
			}
			return base.ToString();
		}

		public override bool Equals(object obj)
		{
			bool result = false;
			if (obj != null)
			{
				if (data is string strA)
				{
					if (obj is string strB)
					{
						result = string.Compare(strA, strB, StringComparison.CurrentCultureIgnoreCase) == 0;
					}
				}
				else
				{
					result = data.Equals(obj);
				}
			}
			return result;
		}

		public override int GetHashCode()
		{
			return data.GetHashCode();
		}
	}

	private const int SeparatorHeight = 3;

	private const int VerticalItemPadding = 4;

	private const int WidthPadding = 30;

	private int initialDropDownWidth;

	private string myText;

	private IContainer components;

	public SeparatorComboBox()
	{
		InitializeComponent();
		initialDropDownWidth = base.DropDownWidth;
		base.DrawMode = DrawMode.OwnerDrawVariable;
		base.HandleCreated += OnHandleCreated;
	}

	private void OnHandleCreated(object sender, EventArgs e)
	{
		UpdateDropDownWidth();
	}

	protected override void OnMeasureItem(MeasureItemEventArgs e)
	{
		object obj = base.Items[e.Index];
		Size size = TextRenderer.MeasureText(e.Graphics, obj.ToString(), Font);
		e.ItemHeight = size.Height + 4;
		e.ItemWidth = size.Width;
		if (obj is SeparatorItem)
		{
			e.ItemHeight += 3;
		}
		base.OnMeasureItem(e);
	}

	protected override void OnDrawItem(DrawItemEventArgs e)
	{
		object obj = base.Items[e.Index];
		bool num = obj is SeparatorItem;
		e.DrawBackground();
		Rectangle bounds = e.Bounds;
		if (num)
		{
			bounds.Height -= 3;
		}
		bounds.Inflate(0, -2);
		TextRenderer.DrawText(e.Graphics, obj.ToString(), Font, bounds, e.ForeColor, e.BackColor, TextFormatFlags.WordEllipsis);
		if (num)
		{
			using Pen pen = new Pen(SystemColors.Window);
			e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 3, e.Bounds.Right, e.Bounds.Bottom - 3);
			e.Graphics.DrawLine(SystemPens.ControlText, e.Bounds.Left, e.Bounds.Bottom - 2, e.Bounds.Right, e.Bounds.Bottom - 2);
			e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
		}
		base.OnDrawItem(e);
	}

	private void UpdateDropDownWidth()
	{
		Graphics graphics = CreateGraphics();
		float num = 0f;
		foreach (object item in base.Items)
		{
			num = Math.Max(num, graphics.MeasureString(item.ToString(), Font).Width);
		}
		num += 30f;
		int num2 = (int)decimal.Round((decimal)num, 0);
		if (num2 > Screen.GetWorkingArea(this).Width)
		{
			num2 = Screen.GetWorkingArea(this).Width;
		}
		if (num2 > initialDropDownWidth)
		{
			base.DropDownWidth = num2;
		}
		graphics.Dispose();
	}

	private void OnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			myText = Text;
		}
	}

	private void OnDropDownClosed(object sender, EventArgs e)
	{
		if (myText != null && Text.Length == 0)
		{
			Text = myText;
			myText = null;
		}
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
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(MS.Internal.ServerClusters.Management.SeparatorComboBox));
		base.SuspendLayout();
		base.DropDownClosed += new System.EventHandler(OnDropDownClosed);
		base.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(OnPreviewKeyDown);
		componentResourceManager.ApplyResources(this, "$this");
		base.Name = "SeparatorComboBox";
		base.ResumeLayout(false);
	}
}
