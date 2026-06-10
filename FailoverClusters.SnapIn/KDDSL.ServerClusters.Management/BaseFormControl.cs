using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using ManagementConsole;

namespace KDDSL.ServerClusters.Management;

internal class BaseFormControl : SnapinUserControl
{
	protected INotifyUser notifyUser;

	protected CluAdminScopeNode scopeNode;

	protected SnapInFormView view;

	protected CursorManager cursorManager;

	protected BackgroundOperation<UpdateReason, object> refreshOperation;

	private IContainer components;

	protected CursorManager CursorManager => cursorManager;

	[Browsable(false)]
	public INotifyUser NotifyUser => notifyUser;

	public CluAdminScopeNode CluAdminScopeNode => scopeNode;

	public SnapInFormView View => view;

	public BaseFormControl()
	{
		InitializeComponent();
	}

	protected override void OnCreateControl()
	{
		((UserControl)this).OnCreateControl();
		cursorManager = new CursorManager((Control)(object)this);
		if (!UIHelper.DesignMode)
		{
			refreshOperation = new BackgroundOperation<UpdateReason, object>((BackgroundOperationFunction<UpdateReason, object>)RefreshViewFetchDataWrapper);
			refreshOperation.OperationCompleted += RefreshViewFetchDataCompleted;
			refreshOperation.MaximumRetriesOnError = ClusterAdministrator.MaxBackgroundRetries;
		}
	}

	public void ShowWaitCursor()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		UIThreadHandlerV val = new UIThreadHandlerV(ShowWaitCursor);
		if (!UIHelper.ExecuteOnUIThread((ISynchronizeInvoke)this, (Delegate)(object)val, Array.Empty<object>()))
		{
			cursorManager.BeginCursor(CursorType.DataLoad);
		}
	}

	public void RestoreCursor()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		UIThreadHandlerV val = new UIThreadHandlerV(RestoreCursor);
		if (!UIHelper.ExecuteOnUIThread((ISynchronizeInvoke)this, (Delegate)(object)val, Array.Empty<object>()))
		{
			cursorManager.EndCursor();
		}
	}

	public virtual void OnRefreshView(UpdateReason reason)
	{
		UIThreadHandlerV<UpdateReason> val = OnRefreshView;
		if (!UIHelper.ExecuteOnUIThread<UpdateReason>((ISynchronizeInvoke)this, (Delegate)(object)val, reason))
		{
			cursorManager.BeginCursor(CursorType.DataLoad);
			if (!refreshOperation.QueueOperation(reason))
			{
				cursorManager.EndCursor();
			}
		}
	}

	private object RefreshViewFetchDataWrapper(BackgroundOperationStatus backgroundStatus, UpdateReason updateReason)
	{
		StartingRefreshViewFetchData(updateReason);
		int num = 1;
		while (true)
		{
			try
			{
				return RefreshViewFetchData(updateReason);
			}
			catch (Exception ex)
			{
				if (!ExceptionHelp.IsFirstExceptionFound<ClusterObjectDeletedException>(ex))
				{
					throw;
				}
				ExceptionHelp.LogException(ex, "Deleted object encountered while refreshing a start page");
				if (num == ClusterAdministrator.MaxDeletedObjectRetries)
				{
					throw ExceptionHelp.Build<ApplicationException>(ex, new string[1] { Resources.RefreshViewDataFailed_Text });
				}
				Thread.Sleep(ClusterAdministrator.DeletedObjectSleepTime);
			}
			num++;
		}
	}

	protected virtual void StartingRefreshViewFetchData(UpdateReason updateReason)
	{
	}

	protected virtual object RefreshViewFetchData(UpdateReason updateReason)
	{
		throw new NotImplementedException();
	}

	protected virtual void RefreshViewFetchDataCompleted(object sender, BackgroundOperationCompletedEventArgs<UpdateReason, object> e)
	{
		cursorManager.EndCursor();
	}

	protected void ReportUpdateError(Exception e)
	{
		ExceptionHelp.LogException(e, "Error updating start page");
		ExceptionHelp.LogException(e, "Failed to update the scope node page");
	}

	public virtual void OnRemoveResultNode(IContext context)
	{
	}

	public virtual void RefreshSelectedResultNode(AsyncStatus status)
	{
		throw new NotImplementedException();
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(BaseFormControl));
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(this, "$this");
		((Control)(object)this).BackColor = SystemColors.Control;
		((Control)(object)this).MinimumSize = new Size(500, 500);
		((Control)this).Name = "BaseFormControl";
		((Control)this).ResumeLayout(performLayout: false);
	}
}

