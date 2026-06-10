using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using FailoverClusters.SnapIn;
using FailoverClusters.UI.Common;
using ManagementConsole;
using MS.Internal.ServerClusters;

namespace FailoverClusters.ClusterSnapIn;

public class WpfViewHostControl<TWpfPageControl, TWpfViewAdapter> : System.Windows.Forms.UserControl, IFormViewControl where TWpfPageControl : System.Windows.Controls.UserControl, new() where TWpfViewAdapter : FormView, IWpfViewAdapter, new()
{
	private TWpfPageControl wpfPageControl;

	private TWpfViewAdapter wpfViewAdapter;

	private bool initializationDone;

	private PointF dpiFactor = new PointF(1f, 1f);

	private float DefaultDPIResolution = 96f;

	private IContainer components;

	private System.Windows.Forms.Label label;

	private Cursor oldCursor;

	public WpfViewHostControl()
	{
		InitializeComponent();
	}

	protected override void OnCreateControl()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		using (Graphics graphics = Graphics.FromHwnd(base.Handle))
		{
			dpiFactor = new PointF(DefaultDPIResolution / graphics.DpiX, DefaultDPIResolution / graphics.DpiY);
		}
		DoubleBuffered = true;
		Dock = DockStyle.Fill;
		initializationDone = false;
		UIThreadHandlerV loadElementHostandWpf = (UIThreadHandlerV)delegate
		{
			wpfPageControl = new TWpfPageControl();
			TabFriendlyElementHost current = TabFriendlyElementHost.Current;
			current.Dock = DockStyle.Fill;
			current.Location = new System.Drawing.Point(0, 0);
			current.Name = "sharedElementHost";
			current.Size = new System.Drawing.Size(756, 433);
			current.TabIndex = 0;
			RemoveEvent(current.Child as FrameworkElement);
			current.Child = null;
			current.ClearReflectParent();
			current.Child = wpfPageControl;
			base.SizeChanged += delegate
			{
				wpfPageControl.Width = (float)base.Width * dpiFactor.X;
				wpfPageControl.Height = (float)base.Height * dpiFactor.Y;
			};
			wpfPageControl.Width = (float)base.Width * dpiFactor.X;
			wpfPageControl.Height = (float)base.Height * dpiFactor.Y;
			InitializationDone();
			SuspendLayout();
			base.Controls.Remove(label);
			base.Controls.Add(current);
			ResumeLayout(performLayout: true);
			Cursor = oldCursor;
		};
		base.OnCreateControl();
		ThreadPool.QueueUserWorkItem(delegate
		{
			Thread.Sleep(100);
			BeginInvoke((Delegate)(object)loadElementHostandWpf);
		});
	}

	public void Initialize(FormView view)
	{
		Exceptions.ThrowIfNull((object)view, "view");
		wpfViewAdapter = view as TWpfViewAdapter;
		if (wpfViewAdapter == null)
		{
			throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "FormView must be assignable to ViewType {0}", typeof(TWpfViewAdapter)));
		}
		wpfViewAdapter.Show += delegate
		{
			if (initializationDone)
			{
				RemoveEvent(TabFriendlyElementHost.Current.Child as FrameworkElement);
				TabFriendlyElementHost.Current.Child = null;
				TabFriendlyElementHost.Current.ClearReflectParent();
				TabFriendlyElementHost.Current.Child = wpfPageControl;
				base.Controls.Add(TabFriendlyElementHost.Current);
			}
		};
		wpfViewAdapter.Hide += delegate
		{
			base.Controls.Remove(TabFriendlyElementHost.Current);
			RemoveEvent(TabFriendlyElementHost.Current.Child as FrameworkElement);
			TabFriendlyElementHost.Current.Child = null;
			TabFriendlyElementHost.Current.ClearReflectParent();
		};
	}

	private static void RemoveEvent(FrameworkElement fe)
	{
		if (fe != null)
		{
			SizeChangedEventHandler value = (SizeChangedEventHandler)Delegate.CreateDelegate(typeof(SizeChangedEventHandler), TabFriendlyElementHost.Current, "childFrameworkElement_SizeChanged");
			fe.SizeChanged -= value;
		}
	}

	private void InitializationDone()
	{
		wpfPageControl.DataContext = wpfViewAdapter.SetupViewModel((ViewModelData)wpfViewAdapter.ViewDescriptionTag);
		initializationDone = true;
	}

	private void InitializeComponent()
	{
		this.oldCursor = this.Cursor;
		this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
		this.label = new System.Windows.Forms.Label();
		this.label.Dock = System.Windows.Forms.DockStyle.Fill;
		this.label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.label.Text = CommonResources.LoadingText;
		base.SuspendLayout();
		base.Controls.Add(this.label);
		this.DoubleBuffered = true;
		base.Name = "WpfHostControl";
		base.Size = new System.Drawing.Size(756, 433);
		base.ResumeLayout(false);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}
}

