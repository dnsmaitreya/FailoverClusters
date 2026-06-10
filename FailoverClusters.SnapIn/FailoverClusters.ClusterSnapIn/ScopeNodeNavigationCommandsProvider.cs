using System;
using System.Windows.Input;
using FailoverClusters.UIFramework;
using ManagementConsole;
using KDDSL.ServerClusters.Management;

namespace FailoverClusters.ClusterSnapIn;

internal class ScopeNodeNavigationCommandsProvider : IScopeNodeNavigationCommandsProvider
{
	private class SelectScopeNodeCommand : ICommand
	{
		private View View { get; set; }

		private string ClusterName { get; set; }

		private string ChildNodeName { get; set; }

		private bool CanExecuteValue { get; set; }

		public event EventHandler CanExecuteChanged;

		public SelectScopeNodeCommand(View view, string clusterName, string childNodeName, bool canExecute)
		{
			View = view;
			ClusterName = clusterName;
			ChildNodeName = childNodeName;
			CanExecuteValue = canExecute;
		}

		public void FireCanExecuteChanged()
		{
			if (this.CanExecuteChanged != null)
			{
				this.CanExecuteChanged(this, new EventArgs());
			}
		}

		public bool CanExecute(object parameter)
		{
			return CanExecuteValue;
		}

		public void Execute(object parameter)
		{
			if (ClusterAdministrator.Instance.RootNode == null)
			{
				return;
			}
			CluAdminScopeNode cluAdminScopeNode = ClusterAdministrator.Instance.RootNode.FindChildWithExpand(ClusterName, ClusterAdministrator.NotifyUser);
			if (cluAdminScopeNode == null)
			{
				return;
			}
			if (!string.IsNullOrEmpty(ChildNodeName))
			{
				CluAdminScopeNode cluAdminScopeNode2 = cluAdminScopeNode.FindChildWithExpand(ChildNodeName, ClusterAdministrator.NotifyUser);
				if (cluAdminScopeNode2 != null)
				{
					View.SelectScopeNode(cluAdminScopeNode2);
				}
			}
			else
			{
				View.SelectScopeNode(cluAdminScopeNode);
			}
		}
	}

	private View View { get; set; }

	public ScopeNodeNavigationCommandsProvider(View view)
	{
		View = view;
	}

	public ICommand GetCommand(string clusterName, string childNodeName, bool canExecute)
	{
		return new SelectScopeNodeCommand(View, clusterName, childNodeName, canExecute);
	}
}

