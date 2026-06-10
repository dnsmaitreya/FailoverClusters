using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace MS.Internal.ServerClusters.Management;

internal static class ClusterConnectionFactory
{
	private class ClusterConnectSettings
	{
		public readonly ConnectedClusterData ConnectionData;

		public readonly ConnectionType Type;

		public ClusterConnectSettings(ConnectedClusterData connectionData, ConnectionType type)
		{
			ConnectionData = connectionData;
			Type = type;
		}
	}

	private class UiUpdate
	{
		private readonly CluadminWaitDialog waitDialog;

		public UiUpdate(CluadminWaitDialog waitDialog)
		{
			this.waitDialog = waitDialog;
		}

		public UiUpdate()
		{
			waitDialog = null;
		}

		public void SetStatusText(string msg)
		{
			if (waitDialog != null)
			{
				waitDialog.StatusText = msg;
			}
		}
	}

	private static readonly HashSet<ClusterContext> ConnectedClusterContextCache = new HashSet<ClusterContext>();

	public static IContext ConnectToCluster(ConnectedClusterData connectionData, ConnectionType setting)
	{
		ClusterConnectSettings connectSettings = new ClusterConnectSettings(connectionData, setting);
		return AttemptClusterConnect(new UiUpdate(), connectSettings);
	}

	public static IContext ConnectToCluster(ConnectedClusterData connectionData, INotifyUser notifyUser, ConnectionType setting)
	{
		string initialMessage = string.Format(CultureInfo.CurrentCulture, Resources.ConnectingToCluster_Text, connectionData.ClusterName);
		return ConnectToCluster(new ClusterConnectSettings(connectionData, setting), notifyUser, initialMessage);
	}

	public static void AddClusterContextToCache(ClusterContext context)
	{
		lock (ConnectedClusterContextCache)
		{
			if (!ConnectedClusterContextCache.Contains(context))
			{
				ConnectedClusterContextCache.Add(context);
			}
		}
	}

	public static void RemoveClusterContextFromCache(ClusterContext context)
	{
		lock (ConnectedClusterContextCache)
		{
			if (ConnectedClusterContextCache.Contains(context))
			{
				ConnectedClusterContextCache.Remove(context);
			}
		}
	}

	public static ClusterContext GetClusterContextFromCache(Guid id)
	{
		lock (ConnectedClusterContextCache)
		{
			return ConnectedClusterContextCache.FirstOrDefault((ClusterContext ctx) => ctx.Cluster.Id == id);
		}
	}

	private static IContext ConnectToCluster(ClusterConnectSettings connectSettings, INotifyUser notifyUser, string initialMessage)
	{
		CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.ConnectingToClusterTitle_Text, initialMessage);
		cluadminWaitDialog.DisplayDelay = TimeSpan.Zero;
		cluadminWaitDialog.CancelTime = new TimeSpan(0, 0, 5);
		using (cluadminWaitDialog)
		{
			return cluadminWaitDialog.ShowDialog(notifyUser, AttemptClusterConnect, connectSettings);
		}
	}

	private static IContext AttemptClusterConnect(CluadminWaitDialog waitDialog, ClusterConnectSettings connectSettings)
	{
		return AttemptClusterConnect(new UiUpdate(waitDialog), connectSettings);
	}

	private static IContext AttemptClusterConnect(UiUpdate uiUpdate, ClusterConnectSettings connectSettings)
	{
		IContext context = null;
		Exception ex = null;
		connectSettings.ConnectionData.Connected = false;
		if (connectSettings.ConnectionData.ClusterName != null)
		{
			uiUpdate.SetStatusText(string.Format(CultureInfo.CurrentCulture, Resources.ConnectingToCluster_Text, connectSettings.ConnectionData.ClusterName));
			try
			{
				context = ContextFactory.CreateClusterContext(connectSettings.ConnectionData.ClusterName);
				connectSettings.ConnectionData.Connected = true;
			}
			catch (ClusterWmiProviderException caughtException)
			{
				ExceptionHelp.LogException(caughtException, "Error creating cluster context for '{0}'", connectSettings.ConnectionData.ClusterName);
				throw;
			}
			catch (ClusterAccessDeniedException caughtException2)
			{
				ExceptionHelp.LogException(caughtException2, "Error creating cluster context for '{0}'", connectSettings.ConnectionData.ClusterName);
				throw;
			}
			catch (ClusterReadOnlyAccessException caughtException3)
			{
				ExceptionHelp.LogException(caughtException3, "Error creating cluster context for '{0}'", connectSettings.ConnectionData.ClusterName);
				throw;
			}
			catch (ClusterRpcConnectionException ex2)
			{
				ExceptionHelp.LogException(ex2, "Error creating cluster context for '{0}'", connectSettings.ConnectionData.ClusterName);
				ex = ex2;
			}
			catch (ClusterEndpointNotRegisteredException ex3)
			{
				ExceptionHelp.LogException(ex3, "Error creating cluster context for '{0}'", connectSettings.ConnectionData.ClusterName);
				ex = ex3;
			}
			catch (ClusterStatusNotReadyException ex4)
			{
				ExceptionHelp.LogException(ex4, "Error creating cluster context for '{0}'", connectSettings.ConnectionData.ClusterName);
				ex = ex4;
			}
			catch (ClusterSharingPausedException ex5)
			{
				ExceptionHelp.LogException(ex5, "Error creating cluster context for '{0}'", connectSettings.ConnectionData.ClusterName);
				ex = ex5;
			}
			catch (ClusterException ex6)
			{
				Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(ex6);
				if (firstException == null)
				{
					throw;
				}
				int num = firstException.NativeErrorCode;
				if (num > 0)
				{
					num = (num & 0xFFFF) | 0x70000 | int.MinValue;
				}
				switch (num)
				{
				case -2147024891:
					ExceptionHelp.LogException(ex6, "Error creating cluster context for '{0}'", connectSettings.ConnectionData.ClusterName);
					throw;
				case -2147024826:
				case -2147023174:
				case -2147023170:
				case -2147023169:
				case -2147023143:
				case -2147023071:
					ExceptionHelp.LogException(ex6, "Error creating cluster context for '{0}'", connectSettings.ConnectionData.ClusterName);
					ex = ex6;
					break;
				default:
					throw;
				}
			}
		}
		if (context == null)
		{
			context = ConnectToNodes(uiUpdate, connectSettings, ex);
			if (context == null && ex != null)
			{
				throw ex;
			}
		}
		return context;
	}

	private static IContext ConnectToNodes(UiUpdate uiUpdate, ClusterConnectSettings connectSettings, Exception exception)
	{
		if (exception != null)
		{
			ExceptionHelp.LogException(exception, "Error creating cluster context for '{0}'", connectSettings.ConnectionData.ClusterName);
		}
		IContext context = OpenClusterByNodes(uiUpdate, connectSettings);
		if (context != null)
		{
			connectSettings.ConnectionData.Connected = true;
		}
		else
		{
			context = ClusterAdministrator.CreateDownClusterContext(connectSettings.ConnectionData.ClusterName);
			if (context != null)
			{
				return context;
			}
			if (exception == null)
			{
				throw ExceptionHelp.Build<ApplicationException>(new string[1] { Resources.NoNodesUp_Text });
			}
		}
		return context;
	}

	private static IContext OpenClusterByNodes(UiUpdate uiUpdate, ClusterConnectSettings connectSettings)
	{
		string text = connectSettings.ConnectionData.ClusterName;
		foreach (string nodeName in connectSettings.ConnectionData.NodeNames)
		{
			text = nodeName;
			uiUpdate.SetStatusText(string.Format(CultureInfo.CurrentCulture, Resources.ConnectingToCluster_Text, text));
			try
			{
				return ContextFactory.CreateClusterContext(text);
			}
			catch (ClusterAccessDeniedException)
			{
				return null;
			}
			catch (ClusterRpcConnectionException caughtException)
			{
				ExceptionHelp.LogException(caughtException, "ContextFactory.CreateClusterContext() failed for : '{0}'", nodeName);
			}
			catch (ClusterEndpointNotRegisteredException caughtException2)
			{
				ExceptionHelp.LogException(caughtException2, "ContextFactory.CreateClusterContext() failed for : '{0}'", nodeName);
			}
			catch (ClusterStatusNotReadyException caughtException3)
			{
				ExceptionHelp.LogException(caughtException3, "ContextFactory.CreateClusterContext() failed for : '{0}'", nodeName);
			}
			catch (ClusterSharingPausedException caughtException4)
			{
				ExceptionHelp.LogException(caughtException4, "ContextFactory.CreateClusterContext() failed for : '{0}'", nodeName);
			}
			catch (ApplicationException caughtException5)
			{
				ExceptionHelp.LogException(caughtException5, "ContextFactory.CreateClusterContext() failed for : '{0}'", nodeName);
			}
		}
		if (connectSettings.Type == ConnectionType.AnyConnection)
		{
			foreach (string nodeName2 in connectSettings.ConnectionData.NodeNames)
			{
				try
				{
					if (NetworkHelper.CanPing(nodeName2))
					{
						return new DownClusterContext(new ClusterDatabase(nodeName2));
					}
				}
				catch (ApplicationException caughtException6)
				{
					ExceptionHelp.LogException(caughtException6, "Failed to create cluster database for : '{0}'", text);
				}
			}
		}
		return null;
	}
}

