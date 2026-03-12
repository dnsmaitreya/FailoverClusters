using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.FailoverClusters.UI.Controls;
using Microsoft.ManagementConsole;
using MS.Internal.ServerClusters.Controls;

namespace MS.Internal.ServerClusters.Management;

internal class DownClusterStartPageControl : StartPageContainerControl
{
	internal class SummaryData
	{
		internal string DisplayName { get; set; }

		internal string NodeStatus { get; set; }

		internal bool IsClusterUp { get; set; }

		internal SummaryData()
		{
			DisplayName = string.Empty;
			NodeStatus = string.Empty;
			IsClusterUp = false;
		}
	}

	internal class DownNodeListItem : ClusterListItem
	{
		private readonly DownNodeContext context;

		public static DownNodeListItem Create(DownNodeContext context, IDisposable childContext, ClusterListItemChildContext childContextOption)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			DownNodeListItem downNodeListItem = new DownNodeListItem(context, childContext, childContextOption);
			if ((((ClusterListItem)downNodeListItem).Options & 2) == 0)
			{
				((ClusterListItem)downNodeListItem).Initialize();
			}
			return downNodeListItem;
		}

		private DownNodeListItem(DownNodeContext context, IDisposable childContext, ClusterListItemChildContext childContextOption)
			: base(context.InstanceId, (string)null, (ClusterListItemOptions)2, childContext, childContextOption)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			this.context = context;
			((ListViewItem)this).Tag = context;
		}

		public override void Initialize()
		{
			context.ServiceStateChanged += OnPropertiesChanged;
			((ClusterListItem)this).Initialize();
		}

		protected override int GetImageIndexValue()
		{
			return context.ImageIndex;
		}

		protected override ReadOnlyCollection<string> GetPropertiesValue()
		{
			return ClusterListItem.CreateProperties(new string[2]
			{
				context.DisplayName,
				GetStateString(context.ServiceState)
			});
		}

		private void OnPropertiesChanged(object sender, EventArgs e)
		{
			((ClusterListItem)this).BeginRefresh();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (((ClusterListItem)this).Initialized)
				{
					context.ServiceStateChanged -= OnPropertiesChanged;
				}
				context.Dispose();
			}
			((ClusterListItem)this).Dispose(disposing);
		}

		public override void Refresh()
		{
			((ClusterListItem)this).Refresh();
			context.Refresh();
		}

		private static string GetStateString(WindowsServiceState state)
		{
			switch (state)
			{
			case WindowsServiceState.StartPending:
			case WindowsServiceState.StopPending:
			case WindowsServiceState.ContinuePending:
			case WindowsServiceState.PausePending:
				return Resources.ServiceState_Pending_Text;
			case WindowsServiceState.Paused:
				return Resources.ServiceState_Paused_Text;
			case WindowsServiceState.Running:
				return Resources.ServiceState_Running_Text;
			default:
				return Resources.ServiceState_Stopped_Text;
			}
		}
	}

	private BackgroundOperation<object, SummaryData> updateSummary;

	private DownClusterContext context;

	private bool lastSummaryUpdateOperationSucceeded;

	private ResultNodeList nodesList;

	private readonly IContainer components;

	private NamedValueLabel clusterName;

	private NamedValueLabel nodeStatus;

	private TableLayoutPanel layoutPanel;

	private TitleBarControl titleBar;

	private SummaryTitleControl summaryTitle;

	private CollapsiblePanel tasksPanel;

	private CollapsiblePanel nodesPanel;

	private CollapsiblePanel shortcutsPanel;

	private TableLayoutPanel tableLayoutPanel;

	private NamedValueLabel clusterStatus;

	public DownClusterStartPageControl()
	{
		InitializeComponent();
	}

	protected override void OnCreateControl()
	{
		base.OnCreateControl();
		if (!((Component)(object)this).DesignMode)
		{
			summaryTitle.Icon = WinFormsHelp.GetLargeIconBitmap(Icons.ClusterDown);
			summaryTitle.SubTitle = string.Empty;
			updateSummary = new BackgroundOperation<object, SummaryData>((BackgroundOperationFunction<object, SummaryData>)GetSummaryData);
			updateSummary.OperationCompleted += UpdateSummaryData;
		}
	}

	protected override void InitializeInternal(FormView formView)
	{
		base.InitializeInternal(formView);
		context = (DownClusterContext)base.CluAdminScopeNode.Context;
		((Control)(object)this).SuspendLayout();
		try
		{
			base.CluAdminScopeNode.Context.ExecuteUnderActionsLock(delegate(ActionsPaneItemCollection actions)
			{
				LinksPanelControl contentControl = LinksPanelControl.CreateActionPanel(this, actions, ActionTypes.All, Resources.DownClusterPage_ManagementPanel_Text);
				tasksPanel.SetContentControl((Control)(object)contentControl);
			});
			shortcutsPanel.SetContentControl((Control)(object)LinksPanelControl.CreateShortcutPanel(this, base.CluAdminScopeNode.Context));
			nodesList = new ResultNodeList();
			nodesList.Initialize(view, context.InstanceId);
			((Control)(object)nodesList).BackColor = ((Control)(object)this).BackColor;
			((ClusterList)nodesList).View = System.Windows.Forms.View.Details;
			((ClusterList)nodesList).ShowGroups = false;
			((Control)(object)nodesList).MinimumSize = new Size(0, 64);
			((ClusterList)nodesList).RequiredSizeChanged += delegate
			{
				((Control)(object)nodesList).Height = ((ClusterList)nodesList).RequiredSize.Height;
			};
			((ClusterList)nodesList).SetColumns(new string[2]
			{
				Resources.Node_Text,
				Resources.ServiceStatus_Text
			});
			((ClusterList)nodesList).VirtualMode = true;
			((ClusterList)nodesList).BorderStyle = BorderStyle.None;
			((ClusterList)nodesList).Scrollable = true;
			((Control)(object)nodesList).Font = SystemFonts.DefaultFont;
			nodesPanel.SetContentControl((Control)(object)nodesList);
			List<ClusterListItem> list = new List<ClusterListItem>();
			foreach (DownNodeListItem item in context.Nodes.Select((DownNodeContext nodeContext) => DownNodeListItem.Create(nodeContext, null, (ClusterListItemChildContext)1)))
			{
				((ClusterListItem)item).ExpandStatus = (ExpandStatus)0;
				list.Add((ClusterListItem)(object)item);
			}
			((ClusterList)nodesList).BeginUpdate();
			try
			{
				((ClusterList)nodesList).AddRange((ICollection<ClusterListItem>)list);
			}
			finally
			{
				((ClusterList)nodesList).EndUpdate();
			}
			context.ContextRefreshed += ContextContextRefreshed;
		}
		finally
		{
			((Control)(object)this).ResumeLayout();
		}
	}

	private void ContextContextRefreshed(object sender, EventArgs e)
	{
		StartSummaryUpdate(UpdateReason.Refresh);
	}

	protected override void OnShowInternal()
	{
		base.OnShowInternal();
		SubscribeToNotifications();
	}

	protected override void OnHideInternal()
	{
		base.OnHideInternal();
		updateSummary.CancelOperations();
		UnsubscribeToNotifications();
	}

	private void SubscribeToNotifications()
	{
		context.ClusterConnectionChanged += ContextClusterConnectionChanged;
		foreach (DownNodeContext node in context.Nodes)
		{
			node.ImageIndexChanged += OnDownClusterChanged;
		}
	}

	private void UnsubscribeToNotifications()
	{
		context.ClusterConnectionChanged -= ContextClusterConnectionChanged;
		foreach (DownNodeContext node in context.Nodes)
		{
			node.ImageIndexChanged -= OnDownClusterChanged;
		}
	}

	private void OnDownClusterChanged(object sender, EventArgs e)
	{
		StartSummaryUpdate(UpdateReason.Update);
	}

	protected override object RefreshViewFetchData(UpdateReason reason)
	{
		UIThreadHandler<object, UpdateReason> val = RefreshViewFetchData;
		object result = null;
		if (UIHelper.ExecuteOnUIThread<object, UpdateReason>(ref result, (ISynchronizeInvoke)this, (Delegate)(object)val, reason))
		{
			return result;
		}
		StartSummaryUpdate(reason);
		return null;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (components != null)
			{
				components.Dispose();
			}
			context.ContextRefreshed -= ContextContextRefreshed;
			((ClusterList)nodesList).Clear();
		}
		base.Dispose(disposing);
	}

	private void StartSummaryUpdate(UpdateReason reason)
	{
		UIThreadHandlerV<UpdateReason> val = StartSummaryUpdate;
		if (!UIHelper.ExecuteOnUIThread<UpdateReason>((ISynchronizeInvoke)this, (Delegate)(object)val, reason))
		{
			if (!lastSummaryUpdateOperationSucceeded)
			{
				ResetSummary();
			}
			base.CursorManager.BeginCursor(CursorType.DataLoad);
			if (!updateSummary.QueueOperation((object)null))
			{
				base.CursorManager.EndCursor();
			}
		}
	}

	private void ResetSummary()
	{
		ResetSummary(CommonResources.LoadingText);
	}

	private void ResetSummary(string message)
	{
		UIThreadHandlerV<string> val = ResetSummary;
		if (!UIHelper.ExecuteOnUIThread<string>((ISynchronizeInvoke)this, (Delegate)(object)val, message))
		{
			titleBar.Title = context.DisplayName;
			summaryTitle.EnableLink = false;
			summaryTitle.Icon = WinFormsHelp.GetLargeIconBitmap(Icons.GetIcon(Icons.ClusterIndex));
			summaryTitle.Title = Resources.Summary_Text;
			clusterName.DataValue = message;
			nodeStatus.DataValue = message;
		}
	}

	private SummaryData GetSummaryData(BackgroundOperationStatus backgroundStatus, object parameter)
	{
		SummaryData obj = new SummaryData
		{
			DisplayName = base.CluAdminScopeNode.DisplayName
		};
		int count = context.Nodes.Count;
		int num = context.Nodes.Count((DownNodeContext downNode) => downNode.ServiceState == WindowsServiceState.Running);
		obj.NodeStatus = string.Format(CultureInfo.CurrentCulture, Resources.DownCluster_Nodes_Text, num, count);
		obj.IsClusterUp = num != 0 && context.IsClusterUp;
		return obj;
	}

	private void UpdateSummaryData(object sender, BackgroundOperationCompletedEventArgs<object, SummaryData> e)
	{
		base.CursorManager.EndCursor();
		if (!base.IsViewVisible)
		{
			return;
		}
		lastSummaryUpdateOperationSucceeded = e.Success;
		if (e.Error != null)
		{
			ResetSummary(Resources.Unavailable_Text);
			ReportUpdateError(e.Error);
		}
		else if (e.Success)
		{
			SummaryData operationResult = e.OperationResult;
			summaryTitle.EnableLink = false;
			titleBar.Title = string.Format(CultureInfo.CurrentCulture, Resources.ClusterNodeStartPageTitleLabelFormat_Text, operationResult.DisplayName);
			summaryTitle.Title = string.Format(CultureInfo.CurrentCulture, Resources.ClusterNodeStartPageSummaryFormat_Text, operationResult.DisplayName);
			clusterName.DataValue = operationResult.DisplayName;
			nodeStatus.DataValue = operationResult.NodeStatus;
			if (operationResult.IsClusterUp)
			{
				clusterStatus.DataValue = Resources.Cluster_Up_Text;
				clusterStatus.HideIcon();
			}
			else
			{
				clusterStatus.DataValue = Resources.Cluster_Down_Text;
				clusterStatus.ShowIcon(Icons.Warning);
			}
		}
	}

	public override void RefreshSelectedResultNode(AsyncStatus status)
	{
		nodesList.RefreshSelectedItem();
	}

	private void InitializeComponent()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DownClusterStartPageControl));
		clusterName = new NamedValueLabel();
		nodeStatus = new NamedValueLabel();
		layoutPanel = new TableLayoutPanel();
		titleBar = new TitleBarControl();
		summaryTitle = new SummaryTitleControl();
		clusterStatus = new NamedValueLabel();
		tasksPanel = new CollapsiblePanel();
		nodesPanel = new CollapsiblePanel();
		shortcutsPanel = new CollapsiblePanel();
		tableLayoutPanel = new TableLayoutPanel();
		layoutPanel.SuspendLayout();
		tableLayoutPanel.SuspendLayout();
		((Control)(object)this).SuspendLayout();
		componentResourceManager.ApplyResources(clusterName, "clusterName");
		clusterName.EnableLink = false;
		((Control)(object)clusterName).MinimumSize = new Size(10, 22);
		((Control)(object)clusterName).Name = "clusterName";
		((Control)(object)clusterName).TabStop = false;
		clusterName.UseBoldFontForName = true;
		componentResourceManager.ApplyResources(nodeStatus, "nodeStatus");
		nodeStatus.EnableLink = false;
		((Control)(object)nodeStatus).MinimumSize = new Size(10, 22);
		((Control)(object)nodeStatus).Name = "nodeStatus";
		((Control)(object)nodeStatus).TabStop = false;
		nodeStatus.UseBoldFontForName = true;
		componentResourceManager.ApplyResources(layoutPanel, "layoutPanel");
		layoutPanel.Controls.Add((Control)(object)titleBar, 0, 0);
		layoutPanel.Controls.Add((Control)(object)summaryTitle, 0, 1);
		layoutPanel.Controls.Add((Control)(object)clusterName, 0, 2);
		layoutPanel.Controls.Add((Control)(object)nodeStatus, 1, 2);
		layoutPanel.Controls.Add((Control)(object)clusterStatus, 0, 3);
		layoutPanel.Name = "layoutPanel";
		componentResourceManager.ApplyResources(titleBar, "titleBar");
		layoutPanel.SetColumnSpan((Control)(object)titleBar, 2);
		((Control)(object)titleBar).MinimumSize = new Size(20, 27);
		((Control)(object)titleBar).Name = "titleBar";
		layoutPanel.SetColumnSpan((Control)(object)summaryTitle, 2);
		componentResourceManager.ApplyResources(summaryTitle, "summaryTitle");
		summaryTitle.EnableLink = false;
		((Control)(object)summaryTitle).MinimumSize = new Size(100, 56);
		((Control)(object)summaryTitle).Name = "summaryTitle";
		((Control)(object)summaryTitle).TabStop = false;
		componentResourceManager.ApplyResources(clusterStatus, "clusterStatus");
		clusterStatus.EnableLink = false;
		((Control)(object)clusterStatus).MinimumSize = new Size(10, 22);
		((Control)(object)clusterStatus).Name = "clusterStatus";
		((Control)(object)clusterStatus).TabStop = false;
		clusterStatus.UseBoldFontForName = true;
		componentResourceManager.ApplyResources(tasksPanel, "tasksPanel");
		((Control)(object)tasksPanel).BackColor = SystemColors.ControlLightLight;
		tasksPanel.Collapsed = false;
		tasksPanel.EndColor = SystemColors.ControlDark;
		((Control)(object)tasksPanel).Name = "tasksPanel";
		tasksPanel.StartColor = SystemColors.ControlLightLight;
		componentResourceManager.ApplyResources(nodesPanel, "nodesPanel");
		((Control)(object)nodesPanel).BackColor = SystemColors.ControlLightLight;
		nodesPanel.Collapsed = false;
		nodesPanel.EndColor = SystemColors.ControlDark;
		((Control)(object)nodesPanel).Name = "nodesPanel";
		nodesPanel.StartColor = SystemColors.ControlLightLight;
		componentResourceManager.ApplyResources(shortcutsPanel, "shortcutsPanel");
		((Control)(object)shortcutsPanel).BackColor = SystemColors.ControlLightLight;
		shortcutsPanel.Collapsed = false;
		shortcutsPanel.EndColor = SystemColors.ControlDark;
		((Control)(object)shortcutsPanel).Name = "shortcutsPanel";
		shortcutsPanel.StartColor = SystemColors.ControlLightLight;
		componentResourceManager.ApplyResources(tableLayoutPanel, "tableLayoutPanel");
		tableLayoutPanel.Controls.Add((Control)(object)tasksPanel, 0, 0);
		tableLayoutPanel.Controls.Add((Control)(object)nodesPanel, 0, 1);
		tableLayoutPanel.Controls.Add((Control)(object)shortcutsPanel, 0, 2);
		tableLayoutPanel.Name = "tableLayoutPanel";
		componentResourceManager.ApplyResources(this, "$this");
		((SnapinUserControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)(object)this).Controls.Add(tableLayoutPanel);
		((Control)(object)this).Controls.Add(layoutPanel);
		((Control)(object)this).Name = "DownClusterStartPageControl";
		layoutPanel.ResumeLayout(performLayout: false);
		tableLayoutPanel.ResumeLayout(performLayout: false);
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}

	private void ContextClusterConnectionChanged(object sender, EventArgs e)
	{
		StartSummaryUpdate(UpdateReason.Update);
	}
}
