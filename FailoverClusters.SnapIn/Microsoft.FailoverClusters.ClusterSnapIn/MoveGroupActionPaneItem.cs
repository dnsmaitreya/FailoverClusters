using System;
using System.Globalization;
using System.Windows.Input;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UIFramework;
using Microsoft.ManagementConsole;
using MS.Internal.ServerClusters;
using MS.Internal.ServerClusters.Management;

namespace Microsoft.FailoverClusters.ClusterSnapIn;

public class MoveGroupActionPaneItem : MmcActionPaneItem
{
	private readonly Group group;

	private readonly ICommand parentCommand;

	private readonly bool moveToBest;

	public MoveGroupActionPaneItem(Group group, ICommand command, ICommand parentCommand, bool moveToBest)
		: base(command)
	{
		this.group = group;
		this.parentCommand = parentCommand;
		this.moveToBest = moveToBest;
	}

	protected override void OnActionTriggered(object sender, ActionEventArgs e)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		if (!(base.Action is Microsoft.ManagementConsole.Action action))
		{
			throw new InvalidOperationException("Action must be a MgmtConsoleAction");
		}
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		Microsoft.FailoverClusters.Framework.Node node = null;
		if (!moveToBest)
		{
			UIDialogProxyCommand val = new UIDialogProxyCommand(base.Command as ClusterCommand, parentCommand, false);
			((UIProxyCommand)val).Execute();
			if (val.OutputObject == null)
			{
				return;
			}
			node = val.OutputObject as Microsoft.FailoverClusters.Framework.Node;
			if (node == null)
			{
				throw new InvalidOperationException("Dialog chooser did not pick object of the correct type");
			}
		}
		SnapinActionEventArgs snapinActionEventArgs = new SnapinActionEventArgs(action, e.Status);
		string arg = ((group is CoreClusterGroup) ? string.Empty : group.DisplayName);
		string initialStatus = ((node == null) ? string.Format(CultureInfo.CurrentCulture, Resources.MovingGroup_BestNode_Text, arg) : string.Format(CultureInfo.CurrentCulture, Resources.MovingGroup_Text, arg, node.Name));
		using CluadminWaitDialog cluadminWaitDialog = snapinActionEventArgs.CreateWaitDialog(initialStatus);
		cluadminWaitDialog.ShowDialog(notifyUserFromSender, delegate
		{
			group.RedirectAsyncOutput(delegate
			{
				ExecuteCommand(node);
			}, delegate(OperationResult result)
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				if (result.Error != null)
				{
					ClusterDialogException.ShowTaskDialog(result.Error);
				}
			});
		});
	}
}
