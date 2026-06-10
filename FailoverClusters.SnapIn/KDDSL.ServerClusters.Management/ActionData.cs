using System;
using System.ComponentModel;
using FailoverClusters.Framework;
using ManagementConsole;

namespace KDDSL.ServerClusters.Management;

internal class ActionData : IDisposable
{
	private ActionBase mmcAction;

	private SnapinActionEventHandler actionHandler;

	private bool isDisposed;

	private readonly object disposeLock = new object();

	internal object Tag { get; private set; }

	public SnapinActionEventHandler EventHandler => actionHandler;

	internal ActionData(ActionBase action, SnapinActionEventHandler eventHandler, object tag)
	{
		mmcAction = action;
		actionHandler = eventHandler;
		Tag = tag;
		SubscribeToTriggered();
	}

	public void Dispose()
	{
		if (!isDisposed)
		{
			UnsubscribeToTriggered();
			lock (disposeLock)
			{
				isDisposed = true;
				mmcAction = null;
				actionHandler = null;
				Tag = null;
			}
			GC.SuppressFinalize(this);
		}
	}

	internal static ActionData GetActionData(ActionBase action)
	{
		return (ActionData)action.Tag;
	}

	internal static object GetActionTag(ActionBase action)
	{
		return GetActionData(action).Tag;
	}

	private void SubscribeToTriggered()
	{
		if (mmcAction is ManagementConsole.Action action)
		{
			action.Triggered += AsyncActionHandler;
		}
		else
		{
			((SyncAction)mmcAction).Triggered += SyncActionHandler;
		}
	}

	private void UnsubscribeToTriggered()
	{
		if (mmcAction != null)
		{
			if (mmcAction is ManagementConsole.Action action)
			{
				action.Triggered -= AsyncActionHandler;
			}
			else
			{
				((SyncAction)mmcAction).Triggered -= SyncActionHandler;
			}
		}
	}

	private void AsyncActionHandler(object sender, ActionEventArgs e)
	{
		SnapinActionEventArgs e2 = new SnapinActionEventArgs(mmcAction, e.Status);
		PerformAction(sender, e2);
	}

	private void SyncActionHandler(object sender, SyncActionEventArgs e)
	{
		SnapinActionEventArgs e2 = new SnapinActionEventArgs(mmcAction, e.Status);
		PerformAction(sender, e2);
	}

	internal void PerformAction(object sender)
	{
		CustomStatus status = new CustomStatus();
		SnapinActionEventArgs e = new SnapinActionEventArgs(mmcAction, status);
		PerformAction(sender, e);
	}

	private void PerformAction(object sender, SnapinActionEventArgs e)
	{
		SnapinActionEventHandler snapinActionEventHandler;
		lock (disposeLock)
		{
			if (mmcAction == null || isDisposed)
			{
				return;
			}
			e.Status.Title = mmcAction.DisplayName;
			snapinActionEventHandler = actionHandler;
			if (snapinActionEventHandler == null)
			{
				return;
			}
		}
		try
		{
			snapinActionEventHandler(sender, e);
		}
		catch (OperationCanceledException)
		{
		}
		catch (ClusterFailedCleanupEvictNodeException ex2)
		{
			ProcessError(sender, ex2, e.Status.Title);
		}
		catch (ClusterBaseException ex3)
		{
			ProcessError(sender, ex3, e.Status.Title);
		}
		catch (ClusterException ex4)
		{
			ProcessError(sender, ex4, e.Status.Title);
		}
		catch (ApplicationException ex5)
		{
			ProcessError(sender, ex5, e.Status.Title);
		}
		catch (Win32Exception ex6)
		{
			ProcessError(sender, ex6, e.Status.Title);
		}
	}

	private static void ProcessError(object sender, Exception ex, string displayName)
	{
		ExceptionHelp.LogException(ex, "An error occurred executing the action '{0}'.", displayName);
		INotifyUser notifyUserFromSender = GetNotifyUserFromSender(sender);
		if (notifyUserFromSender != null)
		{
			notifyUserFromSender.ShowError(ex, Resources.ActionExecution_Error_Text, new object[1] { displayName });
		}
	}

	public static INotifyUser GetNotifyUserFromSender(object sender)
	{
		if (!(sender is CluAdminScopeNode cluAdminScopeNode))
		{
			return ClusterAdministrator.NotifyUser;
		}
		return cluAdminScopeNode.NotifyUser;
	}
}

