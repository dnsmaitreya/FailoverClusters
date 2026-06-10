using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ManagementConsole;

namespace KDDSL.ServerClusters.Management;

internal class StandardStartPageControl : StartPageContainerControl
{
	protected SnapinPanel summaryPanel;

	protected TitleBarControl titleBarControl;

	protected SummaryTitleControl summaryTitleControl;

	private IContainer components;

	private bool lastSummaryUpdateOperationSucceeded;

	private BackgroundOperation<object, object> summaryUpdateOperation;

	private BackgroundOperation<object, object> contentUpdateOperation;

	public StandardStartPageControl()
	{
		InitializeComponent();
		lastSummaryUpdateOperationSucceeded = false;
	}

	protected override void OnCreateControl()
	{
		base.OnCreateControl();
		if (!UIHelper.DesignMode)
		{
			summaryUpdateOperation = new BackgroundOperation<object, object>((BackgroundOperationFunction<object, object>)SummaryDataFetch);
			summaryUpdateOperation.OperationCompleted += SummaryDataFetchCompleted;
			summaryUpdateOperation.MaximumRetriesOnError = ClusterAdministrator.MaxBackgroundRetries;
			contentUpdateOperation = new BackgroundOperation<object, object>((BackgroundOperationFunction<object, object>)ContentDataFetch);
			contentUpdateOperation.OperationCompleted += ContentDataFetchCompleted;
			contentUpdateOperation.MaximumRetriesOnError = ClusterAdministrator.MaxBackgroundRetries;
		}
	}

	protected override void OnHideInternal()
	{
		base.OnHideInternal();
		summaryUpdateOperation.CancelOperations();
		contentUpdateOperation.CancelOperations();
	}

	public override void RefreshSelectedResultNode(AsyncStatus status)
	{
		throw new NotImplementedException();
	}

	protected sealed override object RefreshViewFetchData(UpdateReason reason)
	{
		UIThreadHandler<object, UpdateReason> val = RefreshViewFetchData;
		object result = null;
		if (UIHelper.ExecuteOnUIThread<object, UpdateReason>(ref result, (ISynchronizeInvoke)this, (Delegate)(object)val, reason))
		{
			return result;
		}
		UpdateSummary(reason);
		UpdateContent(reason);
		return null;
	}

	protected void UpdateSummary(UpdateReason reason)
	{
		UIThreadHandlerV<UpdateReason> val = UpdateSummary;
		if (!UIHelper.ExecuteOnUIThread<UpdateReason>((ISynchronizeInvoke)this, (Delegate)(object)val, reason))
		{
			if (reason == UpdateReason.Update || !lastSummaryUpdateOperationSucceeded)
			{
				ClearSummary();
			}
			base.CursorManager.BeginCursor(CursorType.DataLoad);
			if (!summaryUpdateOperation.QueueOperation((object)reason))
			{
				base.CursorManager.EndCursor();
			}
		}
	}

	protected void UpdateContent(UpdateReason updateReason)
	{
		UIThreadHandlerV<UpdateReason> val = UpdateContent;
		if (!UIHelper.ExecuteOnUIThread<UpdateReason>((ISynchronizeInvoke)this, (Delegate)(object)val, updateReason))
		{
			BeginningContentDataFetch(updateReason);
			if (!contentUpdateOperation.QueueOperation((object)updateReason))
			{
				base.CursorManager.EndCursor();
			}
		}
	}

	protected virtual void ClearSummary()
	{
	}

	protected virtual object SummaryDataFetch(BackgroundOperationStatus backgroundStatus, object parameter)
	{
		throw new NotImplementedException();
	}

	protected virtual void SummaryDataFetchCompleted(object sender, BackgroundOperationCompletedEventArgs<object, object> e)
	{
		UIThreadHandlerV<object, BackgroundOperationCompletedEventArgs<object, object>> val = SummaryDataFetchCompleted;
		if (!UIHelper.ExecuteOnUIThread<object, BackgroundOperationCompletedEventArgs<object, object>>((ISynchronizeInvoke)this, (Delegate)(object)val, sender, e))
		{
			base.CursorManager.EndCursor();
			lastSummaryUpdateOperationSucceeded = e.Success;
		}
	}

	protected virtual void BeginningContentDataFetch(UpdateReason updateReason)
	{
		UIThreadHandlerV<UpdateReason> val = BeginningContentDataFetch;
		if (!UIHelper.ExecuteOnUIThread<UpdateReason>((ISynchronizeInvoke)this, (Delegate)(object)val, updateReason))
		{
			base.CursorManager.BeginCursor(CursorType.DataLoad);
		}
	}

	protected virtual object ContentDataFetch(BackgroundOperationStatus backgroundStatus, object parameter)
	{
		throw new NotImplementedException();
	}

	protected virtual void ContentDataFetchCompleted(object sender, BackgroundOperationCompletedEventArgs<object, object> e)
	{
		UIThreadHandlerV<object, BackgroundOperationCompletedEventArgs<object, object>> val = ContentDataFetchCompleted;
		if (!UIHelper.ExecuteOnUIThread<object, BackgroundOperationCompletedEventArgs<object, object>>((ISynchronizeInvoke)this, (Delegate)(object)val, sender, e))
		{
			base.CursorManager.EndCursor();
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
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(StandardStartPageControl));
		summaryPanel = new SnapinPanel();
		summaryTitleControl = new SummaryTitleControl();
		titleBarControl = new TitleBarControl();
		((Control)(object)summaryPanel).SuspendLayout();
		((Control)(object)this).SuspendLayout();
		componentResourceManager.ApplyResources(summaryPanel, "summaryPanel");
		((Control)(object)summaryPanel).Controls.Add((Control)(object)summaryTitleControl);
		((Control)(object)summaryPanel).Controls.Add((Control)(object)titleBarControl);
		((Control)(object)summaryPanel).Name = "summaryPanel";
		componentResourceManager.ApplyResources(summaryTitleControl, "summaryTitleControl");
		summaryTitleControl.EnableLink = false;
		((Control)(object)summaryTitleControl).MinimumSize = new Size(100, 56);
		((Control)(object)summaryTitleControl).Name = "summaryTitleControl";
		((Control)(object)summaryTitleControl).TabStop = false;
		componentResourceManager.ApplyResources(titleBarControl, "titleBarControl");
		((Control)(object)titleBarControl).MinimumSize = new Size(20, 27);
		((Control)(object)titleBarControl).Name = "titleBarControl";
		((Control)(object)this).Controls.Add((Control)(object)summaryPanel);
		((Control)(object)this).Name = "StandardStartPageControl";
		componentResourceManager.ApplyResources(this, "$this");
		((Control)(object)summaryPanel).ResumeLayout(performLayout: false);
		((Control)(object)this).ResumeLayout(performLayout: false);
	}
}

