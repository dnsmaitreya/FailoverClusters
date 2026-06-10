using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace MS.Internal.ServerClusters.Management;

internal class EventLogDialog : SnapinForm
{
	private Label statusLabel;

	private Button closeButton;

	private Button cancelButton;

	private ClusterEventsControl clusterEventsControl;

	private INotifyUser notifyUser;

	private IContainer components;

	public EventLogDialog()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		InitializeComponent();
		if (!UIHelper.DesignMode)
		{
			notifyUser = (INotifyUser)new MessageBoxNotifyUser((IWin32Window)this);
			clusterEventsControl.Initialize(notifyUser);
			clusterEventsControl.EnableFindHotKey(enabled: false);
			clusterEventsControl.SortStart += OnSortStart;
			clusterEventsControl.SortCompleted += OnSortCompleted;
			clusterEventsControl.QueryCompleted += OnQueryCompleted;
		}
		statusLabel.Text = string.Empty;
		cancelButton.Visible = false;
	}

	public void SetInstanceId(Guid guid)
	{
		clusterEventsControl.SetInstanceId(guid);
	}

	public void ExecuteQuery(EventLogFilter filter, INotifyUser showDialogNotifyUser)
	{
		PrepareForAsyncOperation(Resources.LoadingQueryResults_Text);
		clusterEventsControl.QueryAsync(filter);
		showDialogNotifyUser.ShowDialog((Form)(object)this);
	}

	private void Show()
	{
	}

	private void Show(IWin32Window w)
	{
	}

	private void ShowDialog()
	{
	}

	private void ShowDialog(IWin32Window w)
	{
	}

	private void OnSortStart(object sender, EventArgs e)
	{
		try
		{
			PrepareForAsyncOperation(Resources.SortingQueryResults_Text);
		}
		catch (Exception ex)
		{
			notifyUser.ShowError(ex, Resources.CannotSortQueryResults_Text);
		}
	}

	private void OnSortCompleted(object sender, AsyncCompletedEventArgs e)
	{
		try
		{
			if (!e.Cancelled && e.Error != null)
			{
				notifyUser.ShowError(e.Error, Resources.CannotSortQueryResults_Text);
			}
			AsyncOperationCompleted();
		}
		catch (Exception ex)
		{
			notifyUser.ShowError(ex, Resources.CannotSortQueryResults_Text);
		}
	}

	private void OnQueryCompleted(object sender, AsyncCompletedEventArgs e)
	{
		try
		{
			if (!e.Cancelled && e.Error != null)
			{
				notifyUser.ShowError(e.Error, Resources.CannotExecuteQuery_Text);
			}
			AsyncOperationCompleted();
		}
		catch (Exception ex)
		{
			notifyUser.ShowError(ex, Resources.CannotExecuteQuery_Text);
		}
	}

	private void PrepareForAsyncOperation(string message)
	{
		statusLabel.Text = message;
		statusLabel.TextAlign = ContentAlignment.TopRight;
		cancelButton.Visible = true;
		cancelButton.Enabled = true;
	}

	private void AsyncOperationCompleted()
	{
		statusLabel.Text = string.Format(CultureInfo.CurrentCulture, Resources.NumberOfEvents_Text, clusterEventsControl.EventCount);
		statusLabel.TextAlign = ContentAlignment.TopLeft;
		cancelButton.Visible = false;
		clusterEventsControl.EnableFindHotKey(clusterEventsControl.EventCount != 0);
		((Control)(object)clusterEventsControl).Focus();
	}

	private void CancelButtonClick(object sender, EventArgs e)
	{
		try
		{
			statusLabel.Text = Resources.StoppingOperation_Text;
			cancelButton.Enabled = false;
			clusterEventsControl.Cancel();
		}
		catch (Exception ex)
		{
			notifyUser.ShowError(ex, Resources.CannotCancelQuery_Text);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		((Form)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(EventLogDialog));
		statusLabel = new Label();
		closeButton = new Button();
		cancelButton = new Button();
		clusterEventsControl = new ClusterEventsControl();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(statusLabel, "statusLabel");
		statusLabel.Name = "statusLabel";
		componentResourceManager.ApplyResources(closeButton, "closeButton");
		closeButton.DialogResult = DialogResult.Cancel;
		closeButton.Name = "closeButton";
		componentResourceManager.ApplyResources(cancelButton, "cancelButton");
		cancelButton.Name = "cancelButton";
		cancelButton.Click += CancelButtonClick;
		componentResourceManager.ApplyResources(clusterEventsControl, "clusterEventsControl");
		((Control)(object)clusterEventsControl).Name = "clusterEventsControl";
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Form)this).CancelButton = closeButton;
		((Control)this).Controls.Add((Control)(object)clusterEventsControl);
		((Control)this).Controls.Add(closeButton);
		((Control)this).Controls.Add(cancelButton);
		((Control)this).Controls.Add(statusLabel);
		((Control)this).Name = "EventLogDialog";
		((Control)this).ResumeLayout(performLayout: false);
	}
}
