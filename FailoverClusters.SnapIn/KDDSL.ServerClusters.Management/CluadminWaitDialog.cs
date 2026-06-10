using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using ManagementConsole.Advanced;

namespace KDDSL.ServerClusters.Management;

internal class CluadminWaitDialog : CommonDialog
{
	private enum DialogState
	{
		NotShown,
		Shown,
		Closed
	}

	private delegate void SetBool(bool value);

	private delegate void SetTextDelegate(string text);

	private delegate void UpdateProgressDelegate(int workProcessed, int totalWork, string statusText);

	private delegate void CompleteDelegate();

	private delegate O InternalBackgroundOperation<I, O>(BackgroundWaitDialogOperation<I, O> backgroundOperation, I data);

	private class ExternalWindowHandle : IWin32Window
	{
		private IntPtr hwnd;

		public IntPtr Handle => hwnd;

		public ExternalWindowHandle(IntPtr hwnd)
		{
			this.hwnd = hwnd;
		}
	}

	private WaitDialog waitDialog = new WaitDialog();

	private ExternalWindowHandle owner;

	private UpdateMonitor updateMonitor = new UpdateMonitor();

	private ManualResetEvent cancelEvent = new ManualResetEvent(initialState: false);

	private System.Windows.Forms.Timer cancelTimer = new System.Windows.Forms.Timer();

	private TimeSpan cancelTime = new TimeSpan(0, 1, 0);

	private bool autoCancelEnabled = true;

	private bool autoCompleteDialog = true;

	private bool confirmCancel;

	private DialogState dialogState;

	private string initialStatusText;

	public bool CanCancel
	{
		get
		{
			return waitDialog.CanCancel;
		}
		set
		{
			InternalSetCanCancel(value);
		}
	}

	public TimeSpan DisplayDelay
	{
		get
		{
			return waitDialog.DisplayDelay;
		}
		set
		{
			waitDialog.DisplayDelay = value;
		}
	}

	public TimeSpan MinimumDisplayTime
	{
		get
		{
			return waitDialog.MinimumDisplayTime;
		}
		set
		{
			waitDialog.MinimumDisplayTime = value;
		}
	}

	public string Name
	{
		get
		{
			return waitDialog.Name;
		}
		set
		{
			ThrowIfCanceled();
			waitDialog.Name = value;
		}
	}

	public string StatusText
	{
		get
		{
			return waitDialog.StatusText;
		}
		set
		{
			ThrowIfCanceled();
			updateMonitor.ResetMonitor();
			SetStatusTextInternal(value);
		}
	}

	public string Title
	{
		get
		{
			return waitDialog.Title;
		}
		set
		{
			ThrowIfCanceled();
			waitDialog.Title = value;
		}
	}

	public int TotalWork
	{
		get
		{
			return waitDialog.TotalWork;
		}
		set
		{
			ThrowIfCanceled();
			waitDialog.TotalWork = value;
		}
	}

	public int WorkProcessed
	{
		get
		{
			return waitDialog.WorkProcessed;
		}
		set
		{
			ThrowIfCanceled();
			updateMonitor.ResetMonitor();
			waitDialog.WorkProcessed = value;
		}
	}

	public bool ConfirmCancel
	{
		get
		{
			return confirmCancel;
		}
		set
		{
			confirmCancel = value;
		}
	}

	public bool AutoCompleteDialog
	{
		get
		{
			return autoCompleteDialog;
		}
		set
		{
			autoCompleteDialog = value;
		}
	}

	public bool AutoCancelEnabled
	{
		get
		{
			return autoCancelEnabled;
		}
		set
		{
			autoCancelEnabled = false;
		}
	}

	public TimeSpan CancelTime
	{
		get
		{
			return cancelTime;
		}
		set
		{
			cancelTime = value;
		}
	}

	public bool IsCanceled => cancelEvent.WaitOne(0, exitContext: false);

	public TimeSpan CylonTime
	{
		get
		{
			return updateMonitor.DelayTime;
		}
		set
		{
			updateMonitor.DelayTime = value;
		}
	}

	public event EventHandler Cancel;

	private CluadminWaitDialog(string initialStatusText)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		updateMonitor.MessageUpdated += updateMonitor_MessageUpdated;
		waitDialog.Cancel += OnCancel;
		CanCancel = false;
		cancelTimer.Tick += cancelTimer_Tick;
		this.initialStatusText = initialStatusText;
	}

	public static CluadminWaitDialog Create(string title, string initialStatus)
	{
		return new CluadminWaitDialog(initialStatus)
		{
			Title = title
		};
	}

	public static CluadminWaitDialog Create(string title, string initialStatusFormat, params object[] args)
	{
		string initialStatus = string.Format(CultureInfo.CurrentCulture, initialStatusFormat, args);
		return Create(title, initialStatus);
	}

	private void InternalSetCanCancel(bool canCancel)
	{
		if (SynchronizeInvoke.InvokeRequired)
		{
			object[] array = new object[1] { canCancel };
			SynchronizeInvoke.BeginInvoke((Delegate)new SetBool(InternalSetCanCancel), array);
		}
		else if (dialogState != DialogState.Closed)
		{
			waitDialog.CanCancel = canCancel;
		}
	}

	private void SetStatusTextInternal(string text)
	{
		if (SynchronizeInvoke.InvokeRequired)
		{
			object[] array = new object[1] { text };
			SynchronizeInvoke.BeginInvoke((Delegate)new SetTextDelegate(SetStatusTextInternal), array);
		}
		else if (dialogState != DialogState.Closed)
		{
			waitDialog.StatusText = text;
		}
	}

	public void SetStatusText(string format, params object[] args)
	{
		string statusText = string.Format(CultureInfo.CurrentCulture, format, args);
		StatusText = statusText;
	}

	public void ThrowIfCanceled()
	{
		if (IsCanceled)
		{
			throw new OperationCanceledException();
		}
	}

	public void UpdateProgress(int workProcessed, int totalWork, string statusText)
	{
		ThrowIfCanceled();
		if (SynchronizeInvoke.InvokeRequired)
		{
			object[] array = new object[3] { workProcessed, totalWork, statusText };
			SynchronizeInvoke.BeginInvoke((Delegate)new UpdateProgressDelegate(UpdateProgress), array);
		}
		else if (dialogState != DialogState.Closed)
		{
			updateMonitor.ResetMonitor();
			waitDialog.UpdateProgress(workProcessed, totalWork, statusText);
		}
	}

	public void CompleteDialog()
	{
		if (SynchronizeInvoke.InvokeRequired)
		{
			SynchronizeInvoke.BeginInvoke((Delegate)new CompleteDelegate(CompleteDialog), (object[])null);
			return;
		}
		dialogState = DialogState.Closed;
		updateMonitor.Dispose();
		waitDialog.CompleteDialog();
	}

	public override void Reset()
	{
	}

	public void ShowDialog(INotifyUser notifyUser, SimpleBackgroundWaitDialogOperation backgroundOperation)
	{
		ShowDialog(notifyUser, SimpleBackgroundOperationWrapper, backgroundOperation);
	}

	private object SimpleBackgroundOperationWrapper(CluadminWaitDialog cluadminWaitDialog, SimpleBackgroundWaitDialogOperation backgroundOperation)
	{
		backgroundOperation(cluadminWaitDialog);
		return null;
	}

	public O ShowDialog<I, O>(INotifyUser notifyUser, BackgroundWaitDialogOperation<I, O> backgroundOperation, I data)
	{
		InternalBackgroundOperation<I, O> internalBackgroundOperation = BackgroundOperation;
		IAsyncResult asyncResult = internalBackgroundOperation.BeginInvoke(backgroundOperation, data, null, null);
		TimeSpan timeSpan = new TimeSpan(Math.Min(new TimeSpan(0, 0, 2).Ticks, waitDialog.DisplayDelay.Ticks));
		DateTime now = DateTime.Now;
		while (!asyncResult.IsCompleted && DateTime.Now - now < timeSpan)
		{
			Thread.Sleep(10);
			Application.DoEvents();
		}
		if (asyncResult.IsCompleted)
		{
			return internalBackgroundOperation.EndInvoke(asyncResult);
		}
		waitDialog.DisplayDelay -= timeSpan;
		try
		{
			notifyUser.ShowDialog((CommonDialog)this);
		}
		catch (InvalidOperationException)
		{
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Error to display wait dialog");
			throw;
		}
		timeSpan = TimeSpan.FromMinutes(2.0);
		now = DateTime.Now;
		while (!asyncResult.IsCompleted && !IsCanceled)
		{
			Application.DoEvents();
			if (DateTime.Now - now >= timeSpan)
			{
				return default(O);
			}
			Thread.Sleep(10);
		}
		if (!IsCanceled)
		{
			return internalBackgroundOperation.EndInvoke(asyncResult);
		}
		return default(O);
	}

	private O BackgroundOperation<I, O>(BackgroundWaitDialogOperation<I, O> backgroundOperation, I data)
	{
		O result = default(O);
		try
		{
			if (!string.IsNullOrEmpty(initialStatusText))
			{
				StatusText = initialStatusText;
			}
			result = backgroundOperation(this, data);
			return result;
		}
		catch (ClusterObjectDeletedException)
		{
		}
		finally
		{
			if (AutoCompleteDialog)
			{
				CompleteDialog();
			}
		}
		return result;
	}

	protected override bool RunDialog(IntPtr hwndOwner)
	{
		owner = new ExternalWindowHandle(hwndOwner);
		try
		{
			dialogState = DialogState.Shown;
			updateMonitor.ResetMonitor();
			if (autoCancelEnabled)
			{
				cancelTimer.Interval = (int)(cancelTime.TotalMilliseconds + DisplayDelay.TotalMilliseconds);
				cancelTimer.Start();
			}
			waitDialog.ShowDialog(owner);
			return true;
		}
		finally
		{
			owner = null;
		}
	}

	protected void OnCancel(object sender, EventArgs e)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!IsCanceled && (!confirmCancel || ((NotifyUser)new MessageBoxNotifyUser((IWin32Window)owner)).ShowYesNoQuestion(MessageBoxDefaultButton.Button1, Resources.ConfirmCancel_Text) == DialogResult.Yes))
		{
			StatusText = Resources.StoppingOperation_Text;
			CanCancel = false;
			cancelEvent.Set();
			if (AutoCompleteDialog)
			{
				CompleteDialog();
			}
			this.Cancel?.Invoke(this, e);
		}
	}

	private void updateMonitor_MessageUpdated(object sender, EventArgs e)
	{
		if (!IsCanceled)
		{
			SetStatusTextInternal(updateMonitor.Message);
		}
	}

	private void cancelTimer_Tick(object sender, EventArgs e)
	{
		CanCancel = true;
		cancelTimer.Stop();
	}
}

