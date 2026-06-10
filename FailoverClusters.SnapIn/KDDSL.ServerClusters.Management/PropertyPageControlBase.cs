using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace KDDSL.ServerClusters.Management;

internal class PropertyPageControlBase : SnapinUserControl, ISnapInPropertyPage
{
	private INotifyUser notifyUser;

	private readonly string title;

	private readonly Guid helpTopic;

	private bool dirty;

	private bool initialized;

	private readonly CursorManager cursorManager;

	private BackgroundOperation<object, object> initializeOperation;

	private bool initializeOperationCompleted;

	private LinkLabel linkLabelError;

	private Label messageLabel;

	private const int ControlSpacing = 5;

	public string Title => title;

	public bool InitializeOperationCompleted => initializeOperationCompleted;

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

	public INotifyUser NotifyUser
	{
		get
		{
			_ = ((Component)this).DesignMode;
			return notifyUser;
		}
	}

	public bool Initialized => initialized;

	public event EventHandler DirtyChanged;

	public event EventHandler ApplyCompleted;

	public PropertyPageControlBase()
	{
		InitializeComponent();
	}

	protected PropertyPageControlBase(string title)
		: this(title, Guid.Empty)
	{
	}

	protected PropertyPageControlBase(string title, Guid helpTopic)
		: this()
	{
		notifyUser = null;
		this.title = title;
		this.helpTopic = helpTopic;
		dirty = false;
		initialized = false;
		initializeOperationCompleted = false;
		cursorManager = new CursorManager((Control)(object)this);
	}

	protected override void OnCreateControl()
	{
		((UserControl)this).OnCreateControl();
		if (!UIHelper.DesignMode)
		{
			initializeOperation = new BackgroundOperation<object, object>((BackgroundOperationFunction<object, object>)InitializeOperation);
			initializeOperation.OperationCompleted += OnInitializeOperationCompleted;
		}
	}

	protected void OnDirtyChanged()
	{
		this.DirtyChanged?.Invoke(this, EventArgs.Empty);
	}

	protected void OnApplyCompleted()
	{
		this.ApplyCompleted?.Invoke(this, EventArgs.Empty);
	}

	public void Initialize(INotifyUser notifyUserConsole)
	{
		_ = ((Component)this).DesignMode;
		notifyUser = notifyUserConsole;
		initialized = true;
		Reload();
	}

	public void Reload()
	{
		((Control)this).Visible = false;
		ShowMessage(Resources.LoadingProperties_Text);
		cursorManager.BeginCursor(CursorType.DataLoad);
		if (!initializeOperation.QueueOperation((object)null))
		{
			cursorManager.EndCursor();
		}
	}

	private object InitializeOperation(BackgroundOperationStatus backgroundStatus, object parameter)
	{
		LoadProperties();
		return null;
	}

	private void OnInitializeOperationCompleted(object sender, BackgroundOperationCompletedEventArgs<object, object> e)
	{
		try
		{
			cursorManager.EndCursor();
			if (e.Cancelled)
			{
				HideMessage();
			}
			else if (e.Error != null)
			{
				ShowMessage(Resources.CannotLoadProperties_Text);
				ExceptionHelp.LogException(e.Error, Resources.UnexpectedError_Text);
			}
			else if (e.Success)
			{
				HideMessage();
				InitializePage();
			}
		}
		catch (Exception ex)
		{
			ShowMessage(Resources.CannotLoadProperties_Text, ex);
			ExceptionHelp.LogException(ex, Resources.UnexpectedError_Text);
		}
		finally
		{
			IsDirty = false;
			initializeOperationCompleted = true;
		}
	}

	internal void Cancel()
	{
		initializeOperation.CancelOperations();
	}

	public bool ApplyChanges()
	{
		try
		{
			if (!ValidateProperties())
			{
				return false;
			}
			CluadminWaitDialog waitDialog = CluadminWaitDialog.Create(Resources.SavingProperties_Text, Resources.SavingProperties_Text);
			using (waitDialog)
			{
				waitDialog.StatusText = Resources.SavingProperties_Text;
				waitDialog.ShowDialog(NotifyUser, delegate
				{
					SaveProperties(waitDialog);
				});
				if (waitDialog.IsCanceled)
				{
					return false;
				}
			}
			CompleteSaveProperties();
			OnApplyCompleted();
		}
		catch (ClusterInputValidationException ex)
		{
			notifyUser.ShowError((Exception)ex);
			return false;
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Error saving properties");
			throw;
		}
		return true;
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

	protected virtual void LoadProperties()
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

	protected virtual void SaveProperties(CluadminWaitDialog waitDialog)
	{
		throw new NotImplementedException();
	}

	protected virtual void CompleteSaveProperties()
	{
		throw new NotImplementedException();
	}

	private void InitializeComponent()
	{
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(PropertyPageControlBase));
		messageLabel = new Label();
		linkLabelError = new LinkLabel();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(messageLabel, "messageLabel");
		messageLabel.Name = "messageLabel";
		linkLabelError.AutoEllipsis = true;
		componentResourceManager.ApplyResources(linkLabelError, "linkLabelError");
		linkLabelError.Name = "linkLabelError";
		linkLabelError.Click += linkLabelErrorClick;
		((Control)this).Controls.Add(messageLabel);
		((Control)this).Controls.Add(linkLabelError);
		((Control)this).Name = "PropertyPageControlBase";
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).ResumeLayout(performLayout: false);
		((Control)this).PerformLayout();
	}

	private void linkLabelErrorClick(object sender, EventArgs e)
	{
		if (linkLabelError.Tag is Exception ex)
		{
			notifyUser.ShowError(ex);
		}
	}
}
