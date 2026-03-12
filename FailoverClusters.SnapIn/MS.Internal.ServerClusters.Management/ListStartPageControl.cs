using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal class ListStartPageControl : StartPageContainerControl
{
	private TitleBarControl titleBarControl;

	private SnapinPanel dataPanel;

	private SummaryTitleControl dataTitleControl;

	private SnapinPanel dataTitlePanel;

	private Label dataTitle;

	private readonly IContainer components;

	private ResultNodeList listView;

	private readonly BackgroundOperation<object, object> dataPanelUpdateOperation;

	[Browsable(true)]
	protected string Title
	{
		get
		{
			return titleBarControl.Title;
		}
		set
		{
			titleBarControl.Title = value;
		}
	}

	protected string DataTitle
	{
		get
		{
			return dataTitle.Text;
		}
		set
		{
			dataTitle.Text = value;
			dataTitle.Refresh();
		}
	}

	protected string DataSummaryTitle
	{
		get
		{
			return dataTitleControl.Title;
		}
		set
		{
			dataTitleControl.Title = value;
			dataTitleControl.EnableLink = dataTitleControl.Title.Length != 0;
		}
	}

	protected Image DataSummaryIcon
	{
		get
		{
			return dataTitleControl.Icon;
		}
		set
		{
			dataTitleControl.Icon = value;
		}
	}

	protected ResultNodeList ListView => listView;

	protected SnapinPanel DataPanel => dataPanel;

	protected string ListViewUpdateErrorMessage { get; set; }

	protected string ListViewEmptyMessage { get; set; }

	protected ClusterListItem SelectedItem => ((ClusterList)listView).SelectedItem;

	protected string DefaultSummaryTitle
	{
		get
		{
			if (SelectedItem != null)
			{
				return ((ListViewItem)(object)SelectedItem).Text;
			}
			return string.Empty;
		}
	}

	public ListStartPageControl()
	{
		InitializeComponent();
		ListViewUpdateErrorMessage = Resources.List_FailedLoadingItems_Text;
		if (!UIHelper.DesignMode)
		{
			dataPanelUpdateOperation = new BackgroundOperation<object, object>((BackgroundOperationFunction<object, object>)DataPanelFetchData);
			dataPanelUpdateOperation.OperationCompleted += DataPanelFetchDataCompeted;
			dataPanelUpdateOperation.MaximumRetriesOnError = ClusterAdministrator.MaxBackgroundRetries;
		}
		dataTitleControl.Clicked += DataTitleControlClicked;
	}

	protected virtual void DataTitleControlClicked(object sender, EventArgs e)
	{
		UIThreadHandlerV<object, EventArgs> val = DataTitleControlClicked;
		if (UIHelper.ExecuteOnUIThread<object, EventArgs>((ISynchronizeInvoke)this, (Delegate)(object)val, sender, e))
		{
			return;
		}
		SummaryTitleControl summaryTitleControl = (SummaryTitleControl)sender;
		if (!string.IsNullOrEmpty(summaryTitleControl.Title))
		{
			CluAdminScopeNode cluAdminScopeNode = base.CluAdminScopeNode.FindChildWithExpand(summaryTitleControl.Title, base.NotifyUser);
			if (cluAdminScopeNode != null)
			{
				base.View.SelectScopeNode(cluAdminScopeNode);
			}
		}
	}

	protected override void StartingRefreshViewFetchData(UpdateReason updateReason)
	{
		UIThreadHandlerV<UpdateReason> val = StartingRefreshViewFetchData;
		if (!UIHelper.ExecuteOnUIThread<UpdateReason>((ISynchronizeInvoke)this, (Delegate)(object)val, updateReason))
		{
			((ClusterList)listView).EmptyText = CommonResources.LoadingText;
			if (updateReason == UpdateReason.Refresh)
			{
				((ClusterList)listView).Clear();
			}
		}
	}

	protected override object RefreshViewFetchData(UpdateReason reason)
	{
		return null;
	}

	protected override void RefreshViewFetchDataCompleted(object sender, BackgroundOperationCompletedEventArgs<UpdateReason, object> e)
	{
		base.RefreshViewFetchDataCompleted(sender, e);
		if (!base.IsViewVisible)
		{
			if (!(e.OperationUnsafeResult is ICollection<ClusterListItem> collection))
			{
				return;
			}
			{
				foreach (ClusterListItem item in collection)
				{
					item.Dispose();
				}
				return;
			}
		}
		if (e.Error != null)
		{
			((ClusterList)listView).EmptyText = ListViewUpdateErrorMessage;
			ReportUpdateError(e.Error);
		}
		else if (e.Success)
		{
			StartPageContainerControl.UpdateListView((ICollection<ClusterListItem>)e.OperationResult, listView, ListViewEmptyMessage);
		}
	}

	public override void OnRemoveResultNode(IContext context)
	{
		ListView.Remove(context);
	}

	protected IContext GetContext(ClusterListItem item)
	{
		if (item == null)
		{
			return null;
		}
		return (IContext)((ListViewItem)(object)item).Tag;
	}

	public override void RefreshSelectedResultNode(AsyncStatus status)
	{
		UIThreadHandlerV<AsyncStatus> val = RefreshSelectedResultNode;
		if (!UIHelper.ExecuteOnUIThread<AsyncStatus>((ISynchronizeInvoke)this, (Delegate)(object)val, status))
		{
			listView.RefreshSelectedItem();
			RefreshDataPanel(UpdateReason.Refresh);
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
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ListStartPageControl));
		titleBarControl = new TitleBarControl();
		dataPanel = new SnapinPanel();
		dataTitleControl = new SummaryTitleControl();
		dataTitlePanel = new SnapinPanel();
		dataTitle = new Label();
		listView = new ResultNodeList();
		((Control)(object)dataPanel).SuspendLayout();
		((Control)(object)dataTitlePanel).SuspendLayout();
		((Control)(object)this).SuspendLayout();
		componentResourceManager.ApplyResources(titleBarControl, "titleBarControl");
		((Control)(object)titleBarControl).MinimumSize = new Size(20, 27);
		((Control)(object)titleBarControl).Name = "titleBarControl";
		componentResourceManager.ApplyResources(dataPanel, "dataPanel");
		((Control)(object)dataPanel).Controls.Add((Control)(object)dataTitleControl);
		((Control)(object)dataPanel).Controls.Add((Control)(object)dataTitlePanel);
		((Control)(object)dataPanel).Name = "dataPanel";
		componentResourceManager.ApplyResources(dataTitleControl, "dataTitleControl");
		dataTitleControl.EnableLink = false;
		dataTitleControl.Icon = null;
		((Control)(object)dataTitleControl).MinimumSize = new Size(100, 56);
		((Control)(object)dataTitleControl).Name = "dataTitleControl";
		((Control)(object)dataTitleControl).TabStop = false;
		componentResourceManager.ApplyResources(dataTitlePanel, "dataTitlePanel");
		((Control)(object)dataTitlePanel).Controls.Add(dataTitle);
		((Control)(object)dataTitlePanel).ForeColor = SystemColors.ControlText;
		((Control)(object)dataTitlePanel).Name = "dataTitlePanel";
		componentResourceManager.ApplyResources(dataTitle, "dataTitle");
		dataTitle.AutoEllipsis = true;
		dataTitle.Name = "dataTitle";
		componentResourceManager.ApplyResources(listView, "listView");
		((ClusterList)listView).BorderStyle = BorderStyle.Fixed3D;
		((ClusterList)listView).CheckBoxes = false;
		((ClusterList)listView).HideSelection = false;
		((Control)(object)listView).MinimumSize = new Size(0, 100);
		((Control)(object)listView).Name = "listView";
		((ClusterList)listView).Scrollable = true;
		((ClusterList)listView).ShowGroups = true;
		((ClusterList)listView).SingleCheckedItem = false;
		((ClusterList)listView).View = System.Windows.Forms.View.Details;
		((ClusterList)listView).VirtualMode = false;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)(object)this).Controls.Add((Control)(object)listView);
		((Control)(object)this).Controls.Add((Control)(object)dataPanel);
		((Control)(object)this).Controls.Add((Control)(object)titleBarControl);
		((Control)(object)this).MinimumSize = new Size(0, 0);
		((Control)(object)this).Name = "ListStartPageControl";
		((Control)(object)dataPanel).ResumeLayout(performLayout: false);
		((Control)(object)dataTitlePanel).ResumeLayout(performLayout: false);
		((Control)(object)this).ResumeLayout(performLayout: false);
	}

	private void ListViewSelectedItemChanged(object sender, EventArgs e)
	{
		UIThreadHandlerV<object, EventArgs> val = ListViewSelectedItemChanged;
		if (!UIHelper.ExecuteOnUIThread<object, EventArgs>((ISynchronizeInvoke)this, (Delegate)(object)val, sender, e))
		{
			OnSelectedItemChanged();
			RefreshDataPanel(UpdateReason.Refresh);
		}
	}

	protected virtual void OnSelectedItemChanged()
	{
		throw new NotImplementedException();
	}

	protected void RefreshDataPanel(UpdateReason reason)
	{
		UIThreadHandlerV<UpdateReason> val = RefreshDataPanel;
		if (!UIHelper.ExecuteOnUIThread<UpdateReason>((ISynchronizeInvoke)this, (Delegate)(object)val, reason))
		{
			base.CursorManager.BeginCursor(CursorType.DataLoad);
			if (reason == UpdateReason.Refresh)
			{
				DataPanelClear();
			}
			dataPanelUpdateOperation.CancelOperations();
			if (!dataPanelUpdateOperation.QueueOperation((object)null))
			{
				base.CursorManager.EndCursor();
			}
		}
	}

	protected virtual object DataPanelFetchData(BackgroundOperationStatus backgroundStatus, object parameter)
	{
		throw new NotImplementedException();
	}

	protected virtual void DataPanelFetchDataCompeted(object sender, BackgroundOperationCompletedEventArgs<object, object> e)
	{
		UIThreadHandlerV<object, BackgroundOperationCompletedEventArgs<object, object>> val = DataPanelFetchDataCompeted;
		if (!UIHelper.ExecuteOnUIThread<object, BackgroundOperationCompletedEventArgs<object, object>>((ISynchronizeInvoke)this, (Delegate)(object)val, sender, e))
		{
			base.CursorManager.EndCursor();
		}
	}

	protected virtual void DataPanelClear()
	{
	}

	protected override void OnHideInternal()
	{
		base.OnHideInternal();
		dataPanelUpdateOperation.CancelOperations();
		((ClusterList)listView).EmptyText = string.Empty;
		((ClusterList)listView).Clear();
	}

	protected override void InitializeInternal(FormView formView)
	{
		base.InitializeInternal(formView);
		((ClusterList)ListView).View = System.Windows.Forms.View.Details;
		((ClusterList)ListView).ShowGroups = false;
		UpdateActions();
		((ClusterList)listView).SelectedItemChanged += ListViewSelectedItemChanged;
	}

	private void OnLargeIconView(object sender, SnapinActionEventArgs e)
	{
		((ClusterList)ListView).View = System.Windows.Forms.View.LargeIcon;
		UpdateActions();
	}

	private void OnDetailsView(object sender, SnapinActionEventArgs e)
	{
		((ClusterList)ListView).View = System.Windows.Forms.View.Details;
		UpdateActions();
	}

	private void UpdateActions()
	{
		ActionBase actionBase = ActionFactory.CreateAction(Resources.LargeIconViewAction_Text, Resources.LargeIconViewActionDescription_Text, 0, OnLargeIconView);
		ActionBase actionBase2 = ActionFactory.CreateAction(Resources.DetailsViewAction_Text, Resources.DetailsViewActionDescription_Text, 0, OnDetailsView);
		actionBase.Enabled = ((ClusterList)ListView).View != System.Windows.Forms.View.LargeIcon;
		actionBase2.Enabled = !actionBase.Enabled;
		base.View.ModeActionsPaneItems.Clear();
		base.View.ModeActionsPaneItems.AddRange(new ActionsPaneItem[2] { actionBase, actionBase2 });
	}
}
