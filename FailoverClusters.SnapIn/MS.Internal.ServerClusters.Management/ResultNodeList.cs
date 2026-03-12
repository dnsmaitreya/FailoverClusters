using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal class ResultNodeList : ClusterList
{
	private SnapInFormView formView;

	private static volatile ClusterList lastFocusedClusterList;

	private Point lastMouseUpPosition;

	private IContext selectedContext;

	public ResultNodeList()
	{
		((ClusterList)this).HideSelection = false;
		selectedContext = null;
	}

	public virtual void Initialize(FormView view, Guid id)
	{
		((ClusterList)this).SetInstanceId(id);
		formView = (SnapInFormView)view;
	}

	public override void Add(ClusterListItem item)
	{
		CheckItem(item);
		((ClusterList)this).Add(item);
	}

	public override void AddRange(ICollection<ClusterListItem> items)
	{
		foreach (ClusterListItem item in items)
		{
			CheckItem(item);
		}
		((ClusterList)this).AddRange(items);
	}

	private void CheckItem(ClusterListItem item)
	{
	}

	private void OnContextActionsUpdated(object sender, EventArgs e)
	{
		UIThreadHandlerV<object, EventArgs> val = OnContextActionsUpdated;
		if (UIHelper.ExecuteOnUIThread<object, EventArgs>((ISynchronizeInvoke)this, (Delegate)(object)val, sender, e))
		{
			return;
		}
		IContext context = selectedContext;
		if (context != null && (!((ClusterList)this).HideSelection || ((Control)(object)this).Focused))
		{
			if (context.IsResetActionsNeeded)
			{
				formView.SetSelectionData(context);
			}
			else
			{
				context.RefreshStateBasedActions();
			}
		}
	}

	public bool Remove(IContext context)
	{
		ClusterListItem val = FindListItem(context);
		if (val != null)
		{
			return ((ClusterList)this).Remove(val);
		}
		return false;
	}

	public void RefreshSelectedItem()
	{
		ClusterListItem selectedItem = ((ClusterList)this).SelectedItem;
		if (selectedItem == null)
		{
			return;
		}
		foreach (ClusterListItem item in ((ClusterList)this).Items)
		{
			if (item == selectedItem)
			{
				((ClusterList)this).Refresh(selectedItem);
				break;
			}
		}
	}

	private ClusterListItem FindListItem(IContext context)
	{
		foreach (ClusterListItem item in ((ClusterList)this).Items)
		{
			if (((ListViewItem)(object)item).Tag == context)
			{
				return item;
			}
		}
		return null;
	}

	protected override void OnItemRefreshed(ClusterListItemEventArgs e)
	{
		if (((ClusterList)this).MultiSelect && ((ClusterList)this).SelectedItems.Count > 1)
		{
			IContext context = (IContext)((Control)this).Tag;
			if (context.IsResetActionsNeeded)
			{
				formView.SetSelectionData(context);
			}
			else
			{
				context.RefreshStateBasedActions();
			}
		}
		else
		{
			IContext context2 = ((ListViewItem)(object)e.Item).Tag as IContext;
			if (context2.IsResetActionsNeeded || ((ClusterList)this).SelectedItem == null)
			{
				if (((ClusterList)this).SelectedItem != null && (!((ClusterList)this).HideSelection || ((Control)(object)this).Focused))
				{
					if (((ClusterList)this).SelectedItem != null)
					{
						formView.SetSelectionData((IContext)((ListViewItem)(object)((ClusterList)this).SelectedItem).Tag);
					}
					else
					{
						formView.SetSelectionData(null);
					}
				}
			}
			else
			{
				context2.RefreshStateBasedActions();
			}
		}
		((ClusterList)this).OnItemRefreshed(e);
	}

	protected override void OnSelectedItemChanged(EventArgs e)
	{
		((ClusterList)this).OnSelectedItemChanged(e);
		IContext context = selectedContext;
		if (context != null)
		{
			selectedContext = null;
			context.ActionsUpdated -= OnContextActionsUpdated;
		}
		if (((ClusterList)this).SelectedItem == null)
		{
			formView.SetSelectionData(null);
			return;
		}
		if (((ClusterList)this).MultiSelect && ((ClusterList)this).SelectedItems.Count > 1)
		{
			selectedContext = (IContext)((Control)this).Tag;
			context = selectedContext;
		}
		else
		{
			selectedContext = (IContext)((ListViewItem)(object)((ClusterList)this).SelectedItem).Tag;
			context = selectedContext;
			if (context != null)
			{
				context.ActionsUpdated += OnContextActionsUpdated;
			}
		}
		if (context != null)
		{
			formView.SetSelectionData(context);
		}
	}

	public void ResetLastListViewCache()
	{
		lastFocusedClusterList = (ClusterList)(object)this;
		((ClusterList)this).HideSelection = false;
	}

	protected override void OnLostFocus(EventArgs e)
	{
		((ClusterList)this).HideSelection = false;
		((Control)this).OnLostFocus(e);
	}

	protected override void OnGotFocus(EventArgs e)
	{
		if (lastFocusedClusterList != this)
		{
			if (lastFocusedClusterList != null)
			{
				lastFocusedClusterList.HideSelection = true;
			}
			lastFocusedClusterList = (ClusterList)(object)this;
			if (((ClusterList)this).SelectedItem == null)
			{
				formView.SetSelectionData(null);
			}
			else if (((ClusterList)this).MultiSelect && ((ClusterList)this).SelectedItems.Count > 1)
			{
				formView.SetSelectionData((IContext)((Control)this).Tag);
			}
			else
			{
				formView.SetSelectionData((IContext)((ListViewItem)(object)((ClusterList)this).SelectedItem).Tag);
			}
		}
	}

	protected override void OnKeyUp(KeyEventArgs e)
	{
		if (e.KeyCode == Keys.F5)
		{
			formView.RefreshView();
		}
		((Control)this).OnKeyUp(e);
	}

	protected override void OnItemActivate(EventArgs e)
	{
		((ClusterList)this).OnItemActivate(e);
		IContext selectionData = formView.GetSelectionData();
		if (selectionData == null)
		{
			return;
		}
		if (selectionData is GroupContext || selectionData is NodeContext || selectionData is NetworkContext)
		{
			CluAdminScopeNode clusterScopeNode = (CluAdminScopeNode)formView.ScopeNode;
			CluAdminScopeNode cluAdminScopeNode = null;
			cluAdminScopeNode = FindChildOnNodeContext(clusterScopeNode, selectionData.DisplayName);
			if (cluAdminScopeNode != null)
			{
				formView.SelectScopeNode(cluAdminScopeNode);
			}
			return;
		}
		if (selectionData is IHasPropertyPages)
		{
			formView.SelectionData.ShowPropertySheet(string.Format(CultureInfo.CurrentCulture, "{0} {1}", selectionData.DisplayName, Resources.Properties_Text));
			return;
		}
		ActionsPaneItemCollection actionsPaneItems = formView.SelectionData.ActionsPaneItems;
		if (actionsPaneItems == null)
		{
			return;
		}
		foreach (ActionsPaneItem item in actionsPaneItems)
		{
			if (item is ActionBase actionBase && !(actionBase.MnemonicDisplayName != StringExtensions.ReplaceAccelerator(CommandResources.ShowCriticalEvents)) && actionBase.Tag is ActionData actionData)
			{
				actionData.PerformAction(this);
			}
		}
	}

	private CluAdminScopeNode FindChildOnNodeContext(CluAdminScopeNode clusterScopeNode, string nodeToFind)
	{
		while (clusterScopeNode != null && clusterScopeNode.Context.GetType() != typeof(ClusterContext))
		{
			clusterScopeNode = (CluAdminScopeNode)clusterScopeNode.Parent;
		}
		if (clusterScopeNode != null && clusterScopeNode.Context != null)
		{
			_ = clusterScopeNode.Context.GetType() != typeof(ClusterContext);
		}
		return null;
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		((Control)this).OnMouseUp(e);
		if (e.Button == MouseButtons.Right)
		{
			lastMouseUpPosition = e.Location;
			if (formView.GetSelectionData() != null)
			{
				formView.ShowContextMenu(((Control)this).PointToScreen(lastMouseUpPosition), onResultItem: true);
			}
		}
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		if (((ClusterList)this).SelectedItem == null)
		{
			return;
		}
		if (e.KeyCode == Keys.Delete)
		{
			if (((ClusterList)this).SelectedItems.Count > 0)
			{
				Utilities.PerformDelete((((ClusterList)this).SelectedItems.Count == 1) ? (((ListViewItem)(object)((ClusterList)this).SelectedItem).Tag as IContext) : selectedContext, formView, ClusterAdministrator.NotifyUser, new CustomStatus());
			}
		}
		else if (e.KeyData == (Keys.A | Keys.Control))
		{
			((ClusterList)this).SelectAll();
		}
		else if (e.KeyData == Keys.Escape)
		{
			((ClusterList)this).SelectNone();
		}
		else if (e.KeyCode == Keys.Apps)
		{
			Point point = ((Control)this).PointToScreen(((ListViewItem)(object)((ClusterList)this).SelectedItem).Position);
			point.X += 16;
			point.Y += 8;
			formView.ShowContextMenu(point, onResultItem: true);
		}
	}
}
