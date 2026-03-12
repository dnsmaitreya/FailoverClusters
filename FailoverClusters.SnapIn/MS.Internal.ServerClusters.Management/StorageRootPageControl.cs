using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.ManagementConsole;
using MS.Internal.ServerClusters.Controls;

namespace MS.Internal.ServerClusters.Management;

internal class StorageRootPageControl : StartPageContainerControl
{
	private StorageRootContext context;

	private IContainer components;

	private TableLayoutPanel tableLayoutPanel;

	private TitleBarControl titleBarControl;

	private SummaryTitleControl summaryTitleControl;

	private CollapsiblePanel collapsiblePanelNavigate;

	private CollapsiblePanel collapsiblePanelPools;

	private CollapsiblePanel collapsiblePanelDisks;

	public StorageRootPageControl()
	{
		InitializeComponent();
		tableLayoutPanel.Name = "StorageRootPageControlTableLayoutPanel";
	}

	protected override void InitializeInternal(FormView formView)
	{
		base.InitializeInternal(formView);
		titleBarControl.Title = CommonResources.Storage_Text;
		context = (StorageRootContext)base.CluAdminScopeNode.Context;
		if (context.IsDisposed)
		{
			return;
		}
		summaryTitleControl.Title = Resources.StorageSumaryText;
		summaryTitleControl.Icon = new Icon2(Icons.GetIcon(context.ImageIndex)).Get(32, 32);
		ActionsPaneItemCollection actions = null;
		using (CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.RetrievingItem_Text, null))
		{
			cluadminWaitDialog.ShowDialog(notifyUser, delegate
			{
				actions = context.ActionsPaneItems;
			});
			if (cluadminWaitDialog.IsCanceled)
			{
				actions = new ActionsPaneItemCollection();
			}
		}
		LinksPanelControl contentControl = LinksPanelControl.CreateActionPanel(this, actions, ActionTypes.Extended, Resources.StoragePage_DisksPanel_Text);
		collapsiblePanelDisks.SetContentControl((Control)(object)contentControl);
		LinksPanelControl contentControl2 = LinksPanelControl.CreateActionPanel(this, actions, ActionTypes.Extended, Resources.StoragePage_PoolsPanel_Text);
		collapsiblePanelPools.SetContentControl((Control)(object)contentControl2);
		collapsiblePanelNavigate.SetContentControl((Control)(object)LinksPanelControl.CreateShortcutPanel(this, context));
	}

	protected sealed override object RefreshViewFetchData(UpdateReason updateReason)
	{
		return null;
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
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(StorageRootPageControl));
		tableLayoutPanel = new TableLayoutPanel();
		collapsiblePanelDisks = new CollapsiblePanel();
		collapsiblePanelPools = new CollapsiblePanel();
		collapsiblePanelNavigate = new CollapsiblePanel();
		titleBarControl = new TitleBarControl();
		summaryTitleControl = new SummaryTitleControl();
		tableLayoutPanel.SuspendLayout();
		((Control)(object)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel, "tableLayoutPanel");
		tableLayoutPanel.Controls.Add((Control)(object)collapsiblePanelDisks, 0, 2);
		tableLayoutPanel.Controls.Add((Control)(object)collapsiblePanelPools, 0, 3);
		tableLayoutPanel.Controls.Add((Control)(object)collapsiblePanelNavigate, 0, 4);
		tableLayoutPanel.Controls.Add((Control)(object)titleBarControl, 0, 0);
		tableLayoutPanel.Controls.Add((Control)(object)summaryTitleControl, 0, 1);
		tableLayoutPanel.Name = "tableLayoutPanel";
		((Control)(object)collapsiblePanelDisks).BackColor = SystemColors.ControlLightLight;
		collapsiblePanelDisks.Collapsed = false;
		componentResourceManager.ApplyResources(collapsiblePanelDisks, "collapsiblePanelDisks");
		collapsiblePanelDisks.EndColor = SystemColors.ControlDark;
		((Control)(object)collapsiblePanelDisks).Name = "collapsiblePanelDisks";
		collapsiblePanelDisks.StartColor = SystemColors.ControlLightLight;
		((Control)(object)collapsiblePanelPools).BackColor = SystemColors.ControlLightLight;
		collapsiblePanelPools.Collapsed = false;
		componentResourceManager.ApplyResources(collapsiblePanelPools, "collapsiblePanelPools");
		collapsiblePanelPools.EndColor = SystemColors.ControlDark;
		((Control)(object)collapsiblePanelPools).Name = "collapsiblePanelPools";
		collapsiblePanelPools.StartColor = SystemColors.ControlLightLight;
		((Control)(object)collapsiblePanelNavigate).BackColor = SystemColors.ControlLightLight;
		collapsiblePanelNavigate.Collapsed = false;
		componentResourceManager.ApplyResources(collapsiblePanelNavigate, "collapsiblePanelNavigate");
		collapsiblePanelNavigate.EndColor = SystemColors.ControlDark;
		((Control)(object)collapsiblePanelNavigate).Name = "collapsiblePanelNavigate";
		collapsiblePanelNavigate.StartColor = SystemColors.ControlLightLight;
		componentResourceManager.ApplyResources(titleBarControl, "titleBarControl");
		((Control)(object)titleBarControl).MinimumSize = new Size(20, 27);
		((Control)(object)titleBarControl).Name = "titleBarControl";
		componentResourceManager.ApplyResources(summaryTitleControl, "summaryTitleControl");
		summaryTitleControl.EnableLink = false;
		((Control)(object)summaryTitleControl).MinimumSize = new Size(100, 56);
		((Control)(object)summaryTitleControl).Name = "summaryTitleControl";
		((Control)(object)summaryTitleControl).TabStop = false;
		componentResourceManager.ApplyResources(this, "$this");
		((SnapinUserControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)(object)this).Controls.Add(tableLayoutPanel);
		((Control)(object)this).MinimumSize = new Size(0, 0);
		((Control)(object)this).Name = "StorageRootPageControl";
		tableLayoutPanel.ResumeLayout(performLayout: false);
		((Control)(object)this).ResumeLayout(performLayout: false);
	}
}
