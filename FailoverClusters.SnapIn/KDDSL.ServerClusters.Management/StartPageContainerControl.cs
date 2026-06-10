using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FailoverClusters.UI.Common;
using ManagementConsole;

namespace KDDSL.ServerClusters.Management;

internal class StartPageContainerControl : BaseFormControl, IFormViewControl
{
	private bool viewVisible;

	private bool refreshOnShow;

	private IContainer components;

	protected bool RefreshOnShow
	{
		get
		{
			return refreshOnShow;
		}
		set
		{
			refreshOnShow = value;
		}
	}

	protected bool IsViewVisible => viewVisible;

	public StartPageContainerControl()
	{
		InitializeComponent();
		refreshOnShow = true;
	}

	protected virtual void InitializeInternal(FormView formView)
	{
	}

	protected virtual void OnHideInternal()
	{
	}

	protected virtual void OnShowInternal()
	{
	}

	public void Initialize(FormView formView)
	{
		try
		{
			view = (SnapInFormView)formView;
			scopeNode = (CluAdminScopeNode)view.ScopeNode;
			notifyUser = base.CluAdminScopeNode.NotifyUser;
			((Control)(object)this).Dock = DockStyle.Fill;
			InitializeInternal(view);
		}
		catch (ObjectDisposedException ex)
		{
			ClusterLog.LogException((Exception)ex, "The view or the context was disposed whilst initializing the view.");
		}
		catch (Exception ex2)
		{
			if (notifyUser != null)
			{
				notifyUser.ShowError(ex2, Resources.StartPage_Error_Text);
			}
		}
	}

	public void OnShow()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		UIThreadHandlerV val = new UIThreadHandlerV(OnShow);
		if (UIHelper.ExecuteOnUIThread((ISynchronizeInvoke)this, (Delegate)(object)val, Array.Empty<object>()))
		{
			return;
		}
		try
		{
			viewVisible = true;
			if (refreshOnShow)
			{
				OnRefreshView(UpdateReason.Refresh);
			}
			Global.DefaultWindow = view.Control;
			OnShowInternal();
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "An error occurred displaying the start page.");
			if (notifyUser != null)
			{
				notifyUser.ShowError(ex);
			}
		}
	}

	public void OnHide()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		UIThreadHandlerV val = new UIThreadHandlerV(OnHide);
		if (UIHelper.ExecuteOnUIThread((ISynchronizeInvoke)this, (Delegate)(object)val, Array.Empty<object>()))
		{
			return;
		}
		try
		{
			viewVisible = false;
			if (refreshOnShow)
			{
				refreshOperation.CancelOperations();
			}
			OnHideInternal();
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "An error occurred hiding the start page.");
			if (notifyUser != null)
			{
				notifyUser.ShowError(ex);
			}
		}
	}

	protected static void UpdateListView(ICollection<ClusterListItem> newList, ResultNodeList resultNodeList, string emptyMessage)
	{
		Guid id = Guid.Empty;
		if (((ClusterList)resultNodeList).SelectedItem != null)
		{
			id = ((ClusterList)resultNodeList).SelectedItem.Id;
		}
		((ClusterList)resultNodeList).EmptyText = emptyMessage;
		((ClusterList)resultNodeList).BeginUpdate();
		try
		{
			((ClusterList)resultNodeList).Clear();
			((ClusterList)resultNodeList).AddRange(newList);
			if (id != Guid.Empty)
			{
				using (IEnumerator<ClusterListItem> enumerator = ((ClusterList)resultNodeList).Items.Where((ClusterListItem item) => item.Id == id).GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						((ListViewItem)(object)enumerator.Current).Selected = true;
					}
					return;
				}
			}
			((ClusterList)resultNodeList).SelectFirstItem();
			resultNodeList.ResetLastListViewCache();
		}
		finally
		{
			((ClusterList)resultNodeList).EndUpdate();
		}
	}

	private void InitializeComponent()
	{
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(StartPageContainerControl));
		((Control)(object)this).SuspendLayout();
		componentResourceManager.ApplyResources(this, "$this");
		((Control)(object)this).MinimumSize = new Size(500, 100);
		((Control)(object)this).Name = "StartPageContainerControl";
		((Control)(object)this).ResumeLayout(performLayout: false);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}
}

