using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal static class Utilities
{
	internal class FormViewDescriptionTag
	{
		private readonly Type pageControl;

		private readonly object tag;

		public Type PageControl => pageControl;

		public object Tag => tag;

		public FormViewDescriptionTag(Type pageControl, object tag)
		{
			this.pageControl = pageControl;
			this.tag = tag;
		}
	}

	public static ActionsPaneItem DisposeActions(ActionsPaneItem action)
	{
		if (action == null)
		{
			return null;
		}
		if (action.Tag is ActionData actionData)
		{
			actionData.Dispose();
		}
		return null;
	}

	public static ActionsPaneItemCollection DisposeActions(ActionsPaneItemCollection paneAction)
	{
		if (paneAction == null)
		{
			return null;
		}
		foreach (ActionsPaneItem item in new List<ActionsPaneItem>(paneAction.ToArray()))
		{
			if (item is ActionGroup)
			{
				DisposeActions((ActionGroup)item);
			}
			DisposeActions(item);
			paneAction.Remove(item);
		}
		return null;
	}

	public static ActionsPaneItem[] DisposeActions(ActionsPaneItem[] paneAction)
	{
		if (paneAction == null)
		{
			return null;
		}
		foreach (ActionsPaneItem item in new List<ActionsPaneItem>(paneAction))
		{
			if (item is ActionGroup)
			{
				DisposeActions((ActionGroup)item);
			}
			else
			{
				DisposeActions(item);
			}
		}
		Array.Resize(ref paneAction, 0);
		return null;
	}

	public static ActionGroup DisposeActions(ActionGroup actionGroup)
	{
		if (actionGroup == null)
		{
			return null;
		}
		foreach (ActionsPaneItem item in new List<ActionsPaneItem>(actionGroup.Items.ToArray()))
		{
			if (item is ActionGroup)
			{
				DisposeActions((ActionGroup)item);
			}
			DisposeActions(item);
			actionGroup.Items.Remove(item);
		}
		return null;
	}

	public static ActionsPaneItem CloneActions(ActionsPaneItem action)
	{
		if (action == null)
		{
			return null;
		}
		if (action is ActionSeparator)
		{
			return new ActionSeparator();
		}
		if (action is SyncAction)
		{
			SyncAction syncAction = (SyncAction)action;
			SyncAction syncAction2 = new SyncAction(syncAction.DisplayName, syncAction.Description, syncAction.ImageIndex)
			{
				Bulleted = syncAction.Bulleted,
				Checked = syncAction.Checked,
				Enabled = syncAction.Enabled,
				LanguageIndependentName = syncAction.LanguageIndependentName,
				MnemonicDisplayName = syncAction.MnemonicDisplayName
			};
			ActionData actionData = syncAction.Tag as ActionData;
			syncAction2.Tag = ((actionData != null) ? new ActionData(syncAction2, actionData.EventHandler, actionData.Tag) : syncAction.Tag);
			return syncAction2;
		}
		return action;
	}

	public static FormViewDescription CreateFormViewDescription(string displayName, Type pageControlType)
	{
		return CreateFormViewDescription(displayName, pageControlType, null);
	}

	public static FormViewDescription CreateFormViewDescription(string displayName, Type pageControlType, object tag)
	{
		return new FormViewDescription
		{
			DisplayName = displayName,
			Tag = new FormViewDescriptionTag(pageControlType, tag),
			ControlType = pageControlType,
			ViewType = typeof(SnapInFormView)
		};
	}

	public static Type GetPageControlTypeFromTag(object tag)
	{
		return ((FormViewDescriptionTag)tag).PageControl;
	}

	public static object GetTagFromTag(object tag)
	{
		return ((FormViewDescriptionTag)tag).Tag;
	}

	public static bool IsDomainUser()
	{
		if (string.Compare(Environment.UserDomainName, Environment.MachineName, StringComparison.OrdinalIgnoreCase) == 0)
		{
			return false;
		}
		return true;
	}

	public static bool IsVcoCleanupOn(Cluster cluster)
	{
		if ((uint)cluster.GetResourceType(WellKnownResourceType.NetName).GetPrivateProperties(PropertyCollectionSet.ReadWrite)["DeleteVcoOnResCleanup"].Value == 0)
		{
			return false;
		}
		return true;
	}

	public static void NodeStateSubscription(ClusterNode item, EventHandler notificationMethod, bool subscribe)
	{
		if (subscribe)
		{
			item.StateChanged += notificationMethod;
		}
		else
		{
			item.StateChanged -= notificationMethod;
		}
	}

	private static void VerifyUserIsAdminOnNodes<T>(IEnumerable<T> items, string clusterName, Func<T, string> getNodeName)
	{
		ConcurrentBag<string> nodeNames = new ConcurrentBag<string>();
		ConcurrentBag<string> nonAdminNodes = new ConcurrentBag<string>();
		ConcurrentBag<Tuple<Exception, string>> exceptions = new ConcurrentBag<Tuple<Exception, string>>();
		Parallel.ForEach(items, delegate(T item)
		{
			string text = getNodeName(item);
			if (string.IsNullOrWhiteSpace(text))
			{
				return;
			}
			try
			{
				using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(WmiHelper.GetClusterWmiConnection(text), new ObjectQuery(Extensions.FormatInvariantCulture("Select {0} From {1}", new object[2]
				{
					Extensions.FormatInvariantCulture("{0},{1}", new object[2] { "Fqdn", "HasSystemAccess" }),
					"MSCluster_ClusterUtilities"
				})));
				using ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
				foreach (ManagementBaseObject item4 in managementObjectCollection)
				{
					using (item4)
					{
						if (item4 != null && item4["Fqdn"] != null)
						{
							nodeNames.Add(item4["Fqdn"].ToString());
						}
						else
						{
							nodeNames.Add(text);
						}
						if (item4 != null && item4["HasSystemAccess"] != null && !(bool)item4["HasSystemAccess"])
						{
							nonAdminNodes.Add(text);
						}
					}
				}
			}
			catch (COMException item2)
			{
				exceptions.Add(new Tuple<Exception, string>(item2, text));
			}
			catch (ManagementException item3)
			{
				exceptions.Add(new Tuple<Exception, string>(item3, text));
			}
		});
		if (exceptions.Count > 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Tuple<Exception, string> item5 in exceptions)
			{
				stringBuilder.Append(item5.Item2);
				stringBuilder.Append("\n");
			}
			throw new ClusterWmiProviderException(stringBuilder);
		}
		if (nonAdminNodes.Count <= 0)
		{
			return;
		}
		StringBuilder stringBuilder2 = new StringBuilder();
		foreach (string item6 in nonAdminNodes)
		{
			FormatHelp.AddToList(stringBuilder2, item6);
		}
		throw new ClusterAccessDeniedException(clusterName, Extensions.FormatCurrentCulture(Resources.NotAdminOnNodes_Text, (object)stringBuilder2.ToString()));
	}

	public static void VerifyUserIsAdminOnNodes(Cluster cluster)
	{
		ClusterNodeCollection nodes = cluster.GetNodes();
		if (nodes != null)
		{
			VerifyUserIsAdminOnNodes(nodes, cluster.Name, (ClusterNode node) => (node.State == NodeState.Up && NetworkHelper.CanPing(node.Name)) ? node.Name : null);
		}
	}

	internal static void VerifyUserIsAdminOnNodes(IEnumerable<string> nodeNames, string clusterName)
	{
		VerifyUserIsAdminOnNodes(nodeNames, clusterName, (string nodeName) => (!NetworkHelper.CanPing(nodeName)) ? null : nodeName);
	}

	public static void PerformDelete(IContext context, object sender, INotifyUser notifyUser, Status status)
	{
		try
		{
			if (context is IDeleteable deleteable && deleteable.IsDeletable)
			{
				deleteable.Delete(sender, status);
			}
		}
		catch (Exception ex)
		{
			notifyUser.ShowError(ex, string.Format(CultureInfo.CurrentCulture, Resources.CannotDelete_Text, context.DisplayName));
		}
	}

	public static INotifyUser GetClusterNotifyUser(IContext context, INotifyUser baseNotifyUser)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		if (context is IClusterSpecific clusterSpecific)
		{
			return (INotifyUser)new ClusterFilteredNotifyUser(clusterSpecific.Cluster, baseNotifyUser);
		}
		return baseNotifyUser;
	}

	public static bool StartInBackground<T>(Action<T> callback, T parameter)
	{
		return Worker.Start(delegate(T param, string stackTrace)
		{
			callback(param);
		}, parameter, delegate(Exception onError)
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			if (!(onError is ApplicationException) && !(onError is ClusterBaseException) && !(onError is Win32Exception))
			{
				throw onError;
			}
			ITaskDialogResource dialogResource = onError as ITaskDialogResource;
			if (dialogResource != null)
			{
				Dispatcher defaultDispatcher = Global.DefaultDispatcher;
				if (defaultDispatcher != null)
				{
					Global.EnqueueInvoke(defaultDispatcher, (Delegate)(System.Action)delegate
					{
						using TaskDialog taskDialog = new TaskDialog(dialogResource);
						taskDialog.ShowDialog(Global.DefaultWindow);
					});
				}
			}
			else
			{
				ClusterDialogException.ShowTaskDialog(onError);
			}
		});
	}

	public static bool StartInBackground(System.Action callback)
	{
		return StartInBackground<object>(delegate
		{
			callback();
		}, null);
	}
}

