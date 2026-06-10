using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FailoverClusters.UI.Common;

namespace MS.Internal.ServerClusters.Management;

internal class CriticalEventsAlert : SnapinUserControl
{
	private SnapInFormView hostView;

	private ClusterEventsMonitor clusterEventsMonitor;

	private INotifyUser notifyUser;

	private IContainer components;

	private NamedValueLabel criticalEventsLabel;

	public event EventHandler SummaryUpdated;

	public CriticalEventsAlert()
	{
		((Control)this).SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
		InitializeComponent();
		((Control)this).TabStop = false;
	}

	public void Initialize(ClusterEventsMonitor eventsMonitor, INotifyUser notifyUserConsole)
	{
		Initialize(visible: false, eventsMonitor, notifyUserConsole);
	}

	public void Initialize(bool visible, ClusterEventsMonitor eventsMonitor, INotifyUser notifyUserConsole)
	{
		UIThreadHandlerV<bool, ClusterEventsMonitor, INotifyUser> val = Initialize;
		if (UIHelper.ExecuteOnUIThread<bool, ClusterEventsMonitor, INotifyUser>((ISynchronizeInvoke)this, (Delegate)(object)val, visible, eventsMonitor, notifyUserConsole))
		{
			return;
		}
		clusterEventsMonitor = eventsMonitor;
		notifyUser = notifyUserConsole;
		hostView = null;
		for (Control parent = ((Control)this).Parent; parent != null; parent = parent.Parent)
		{
			if (parent is StartPageContainerControl startPageContainerControl)
			{
				hostView = startPageContainerControl.View;
				break;
			}
		}
		if (hostView == null)
		{
			throw new InvalidOperationException("This control must be hosted within a BaseFormControl");
		}
		clusterEventsMonitor.ClusterEventsSummaryUpdated += ClusterEventsSummaryUpdated;
		UpdateSummary();
		((Control)this).TabStop = ((Control)this).Visible;
		((Control)this).Visible = visible || clusterEventsMonitor.ClusterEventsCount > 0;
	}

	public void RestoreDefaultColors()
	{
		if (!UIHelper.DesignMode)
		{
			criticalEventsLabel.RestoreDefaultColors();
		}
	}

	internal void Display()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		UIThreadHandlerV val = new UIThreadHandlerV(Display);
		if (!UIHelper.ExecuteOnUIThread((ISynchronizeInvoke)this, (Delegate)(object)val, Array.Empty<object>()))
		{
			((Control)this).TabStop = ((Control)this).Visible;
			((Control)this).Visible = clusterEventsMonitor.ClusterEventsCount > 0;
		}
	}

	private void UpdateSummary()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		UIThreadHandlerV val = new UIThreadHandlerV(UpdateSummary);
		if (!UIHelper.ExecuteOnUIThread((ISynchronizeInvoke)this, (Delegate)(object)val, Array.Empty<object>()))
		{
			if (clusterEventsMonitor == null)
			{
				throw new InvalidOperationException("The control has not been initialized");
			}
			criticalEventsLabel.DataValue = clusterEventsMonitor.ClusterEventsSummary;
			if (clusterEventsMonitor.ClusterEventsCount == 0)
			{
				criticalEventsLabel.HideIcon();
				criticalEventsLabel.EnableLink = false;
			}
			else
			{
				criticalEventsLabel.ShowIcon(Icons.Warning);
				criticalEventsLabel.EnableLink = true;
			}
			OnSummaryUpdated(new EventArgs());
		}
	}

	private void OnSummaryUpdated(EventArgs e)
	{
		this.SummaryUpdated?.Invoke(this, e);
	}

	public int GetRequiredWidth()
	{
		UIThreadHandler<int> val = GetRequiredWidth;
		int result = 0;
		if (UIHelper.ExecuteOnUIThread<int>(ref result, (ISynchronizeInvoke)this, (Delegate)(object)val, Array.Empty<object>()))
		{
			return result;
		}
		return criticalEventsLabel.GetRequiredWidth();
	}

	private void ClusterEventsSummaryUpdated(object sender, EventArgs e)
	{
		if (((Control)this).IsDisposed)
		{
			return;
		}
		try
		{
			UIThreadHandlerV<object, EventArgs> val = ClusterEventsSummaryUpdated;
			if (!UIHelper.ExecuteOnUIThread<object, EventArgs>((ISynchronizeInvoke)this, (Delegate)(object)val, sender, e))
			{
				UpdateSummary();
				((Control)this).Visible = true;
			}
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Error updating critical events");
		}
	}

	private void OnCriticalEventsLinkClicked(object sender, EventArgs e)
	{
		UIThreadHandlerV<object, EventArgs> val = OnCriticalEventsLinkClicked;
		if (UIHelper.ExecuteOnUIThread<object, EventArgs>((ISynchronizeInvoke)this, (Delegate)(object)val, sender, e))
		{
			return;
		}
		CluAdminScopeNode scopeNode = null;
		for (CluAdminScopeNode cluAdminScopeNode = (CluAdminScopeNode)hostView.ScopeNode; cluAdminScopeNode != null; cluAdminScopeNode = (CluAdminScopeNode)cluAdminScopeNode.Parent)
		{
			if (cluAdminScopeNode.Context is ClusterContext || cluAdminScopeNode.Context is DownClusterContext)
			{
				scopeNode = cluAdminScopeNode.FindChildWithExpand(CommonResources.ClusterEvents_Text, notifyUser);
				break;
			}
		}
		clusterEventsMonitor.SetDefaultQuery();
		hostView.SelectScopeNode(scopeNode);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (clusterEventsMonitor != null)
			{
				clusterEventsMonitor.ClusterEventsSummaryUpdated -= ClusterEventsSummaryUpdated;
			}
			if (components != null)
			{
				components.Dispose();
			}
		}
		((SnapinUserControl)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(CriticalEventsAlert));
		criticalEventsLabel = new NamedValueLabel();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(criticalEventsLabel, "criticalEventsLabel");
		criticalEventsLabel.EnableLink = false;
		((Control)(object)criticalEventsLabel).MinimumSize = new Size(10, 16);
		((Control)(object)criticalEventsLabel).Name = "criticalEventsLabel";
		((Control)(object)criticalEventsLabel).TabStop = false;
		criticalEventsLabel.UseBoldFontForName = true;
		criticalEventsLabel.LinkClicked += OnCriticalEventsLinkClicked;
		((Control)this).Controls.Add((Control)(object)criticalEventsLabel);
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Name = "CriticalEventsAlert";
		((Control)this).ResumeLayout(performLayout: false);
	}
}

