using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using MS.Internal.ServerClusters;
using MS.Internal.ServerClusters.Management;

namespace FailoverClusters.WinForms;

internal class SnapinPropertyPageControlBase : SnapinUserControl, ISnapInPropertyPage
{
	private INotifyUser notifyUser;

	private readonly string title;

	private readonly Guid helpTopic;

	private bool dirty;

	private bool initialized;

	private IntPtr hwnd;

	private readonly CursorManager cursorManager;

	private LinkLabel linkLabelError;

	private Label messageLabel;

	private const int ControlSpacing = 5;

	public string Title => title;

	public Guid HelpTopic => helpTopic;

	public bool IsDirty
	{
		get
		{
			return dirty;
		}
		set
		{
			if (dirty != value)
			{
				dirty = value;
				OnDirtyChanged();
			}
		}
	}

	public INotifyUser NotifyUser => notifyUser;

	public bool Initialized => initialized;

	protected IntPtr HWND => hwnd;

	public event EventHandler DirtyChanged;

	public event EventHandler ApplyCompleted;

	protected SnapinPropertyPageControlBase(string title)
		: this(title, Guid.Empty)
	{
	}

	protected SnapinPropertyPageControlBase(string title, Guid helpTopic)
		: this()
	{
		this.title = title;
		this.helpTopic = helpTopic;
		dirty = false;
		initialized = false;
		cursorManager = new CursorManager((Control)(object)this);
	}

	public SnapinPropertyPageControlBase()
	{
		InitializeComponent();
	}

	protected override void OnCreateControl()
	{
		((UserControl)this).OnCreateControl();
		hwnd = ((Control)this).Handle;
	}

	protected void OnDirtyChanged()
	{
		this.DirtyChanged?.Invoke(this, EventArgs.Empty);
	}

	protected void OnApplyCompleted()
	{
		this.ApplyCompleted?.Invoke(this, EventArgs.Empty);
	}

	protected override void Dispose(bool disposing)
	{
		hwnd = IntPtr.Zero;
		((SnapinUserControl)this).Dispose(disposing);
	}

	public void Initialize(INotifyUser notifyUserConsole)
	{
		notifyUser = notifyUserConsole;
		InitializePage();
		initialized = true;
		LoadProperties(null);
	}

	public bool ApplyChanges()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!ValidateProperties())
			{
				return false;
			}
			cursorManager.BeginCursor(CursorType.DataLoad);
			try
			{
				if (!SaveProperties())
				{
					return false;
				}
				CompleteSaveProperties();
				OnApplyCompleted();
			}
			finally
			{
				cursorManager.EndCursor();
			}
			return true;
		}
		catch (ClusterInputValidationException ex)
		{
			ClusterDialogException.ShowTaskDialog(ex, ((Control)this).Handle);
			return false;
		}
		catch (Exception ex2)
		{
			ClusterLog.LogException(ex2, "Error saving properties");
			ClusterDialogException.ShowTaskDialogAsync(ex2, HWND);
			return false;
		}
	}

	protected void ShowMessage(string text)
	{
		InternalShowMessage(text, null);
	}

	protected void ShowMessage(string text, Exception exception)
	{
		if (exception == null)
		{
			throw new ArgumentNullException("exception");
		}
		InternalShowMessage(text, exception);
	}

	private void InternalShowMessage(string text, Exception exception)
	{
		messageLabel.Size = ((Control)this).Size;
		messageLabel.Text = text;
		messageLabel.BringToFront();
		if (exception != null)
		{
			messageLabel.Height = ((Control)this).Size.Height - linkLabelError.Height - 5;
			linkLabelError.Text = Resources.PropertyPageErrorLink_Text;
			linkLabelError.Location = new Point(messageLabel.Location.X, messageLabel.Location.Y + messageLabel.Height + 5);
			linkLabelError.Tag = exception;
			linkLabelError.BringToFront();
		}
		ChangeControls(visible: false);
		((Control)this).Visible = true;
		((Control)(object)this).Refresh();
	}

	protected void HideMessage()
	{
		messageLabel.Text = string.Empty;
		messageLabel.Size = new Size(0, 0);
		messageLabel.SendToBack();
		linkLabelError.Text = string.Empty;
		linkLabelError.Size = new Size(0, 0);
		linkLabelError.SendToBack();
		ChangeControls(visible: true);
		((Control)this).Visible = true;
	}

	private void ChangeControls(bool visible)
	{
		foreach (Control control in ((Control)this).Controls)
		{
			if (control != messageLabel)
			{
				if (control == linkLabelError)
				{
					control.Visible = !visible;
				}
				else
				{
					control.Visible = visible;
				}
			}
			else
			{
				control.Enabled = false;
				control.Visible = true;
			}
		}
	}

	protected virtual object LoadProperties(object context)
	{
		throw new NotImplementedException();
	}

	protected virtual void InitializePage()
	{
		throw new NotImplementedException();
	}

	protected virtual bool ValidateProperties()
	{
		throw new NotImplementedException();
	}

	protected virtual bool SaveProperties()
	{
		throw new NotImplementedException();
	}

	public virtual void Cancel()
	{
	}

	protected virtual void CompleteSaveProperties()
	{
		throw new NotImplementedException();
	}

	private void InitializeComponent()
	{
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SnapinPropertyPageControlBase));
		messageLabel = new Label();
		linkLabelError = new LinkLabel();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(messageLabel, "messageLabel");
		messageLabel.Name = "messageLabel";
		linkLabelError.AutoEllipsis = true;
		componentResourceManager.ApplyResources(linkLabelError, "linkLabelError");
		linkLabelError.Name = "linkLabelError";
		((Control)this).Controls.Add(messageLabel);
		((Control)this).Controls.Add(linkLabelError);
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Name = "SnapinPropertyPageControlBase";
		((Control)this).ResumeLayout(performLayout: false);
		((Control)this).PerformLayout();
	}

	public static void BeginUpdateControl(Control control, Action update)
	{
		if (control.InvokeRequired)
		{
			control.BeginInvoke(update);
		}
		else
		{
			update();
		}
	}

	public static void UpdateControl(Control control, Action update)
	{
		try
		{
			if (control.InvokeRequired)
			{
				control.Invoke(update);
			}
			else
			{
				update();
			}
		}
		catch (ObjectDisposedException)
		{
		}
	}
}

