using System;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.FileServer.Management.Plugin;
using Microsoft.FileServer.Management.ServerManagerProxy;

namespace Microsoft.FailoverClusters.Framework;

public static class ServerManagerProxy
{
	private static readonly object ProxyLock = new object();

	private static ProxyLauncher proxyLauncher;

	private static void InstantiateProxy()
	{
		lock (ProxyLock)
		{
			if (proxyLauncher == null)
			{
				proxyLauncher = new ProxyLauncher();
			}
		}
	}

	public static void StartStorageWizardAsync(StorageWizardType wizardType, Cluster cluster, Action onStartWizard, Action onEndWizard)
	{
		Exceptions.ThrowIfNull(cluster, "cluster");
		onStartWizard.SafeCall();
		Worker.Start(delegate
		{
			string connectedTo = cluster.ConnectedTo;
			string networkName = WmiHelper.GetNodeFullyQualifiedDomainName(connectedTo);
			if (string.IsNullOrEmpty(networkName))
			{
				networkName = connectedTo;
			}
			UIHelper.ExecuteOnDispatcher(delegate
			{
				try
				{
					StartStorageWizard(wizardType, networkName);
				}
				catch (ClusterDefaultException ex)
				{
					ClusterDialogException.ShowTaskDialog(ex);
				}
				finally
				{
					onEndWizard.SafeCall();
				}
			}, OperationType.Sync);
		});
	}

	public static void StartShareWizard(string networkName)
	{
		InstantiateProxy();
		try
		{
			proxyLauncher.StartShareWizard(networkName);
		}
		catch (FSACException ex)
		{
			ClusterLog.LogException(ex, "FSACException occurred when FileServer property page executed.");
			throw new ClusterDefaultException(ExceptionResources.DefaultNewShareWizardExceptionMessage, ex);
		}
	}

	public static void OpenPropertyPage(PropertyPageType propertyPageType, string networkName, string shareName)
	{
		InstantiateProxy();
		try
		{
			proxyLauncher.OpenPropertyPage(propertyPageType, networkName, shareName);
		}
		catch (FSACException ex)
		{
			ClusterLog.LogException(ex, "FSACException occurred when FileServer property page executed.");
			throw new ClusterDefaultException(ExceptionResources.DefaultFileSharePropertyPageExceptionMessage, ex);
		}
	}

	private static void StartStorageWizard(StorageWizardType wizardType, string networkName)
	{
		InstantiateProxy();
		try
		{
			proxyLauncher.StartStorageWizard(wizardType, networkName);
		}
		catch (FSACException ex)
		{
			string message = string.Empty;
			switch (wizardType)
			{
			case StorageWizardType.PoolWizard:
				message = ExceptionResources.DefaultNewPoolWizardExceptionMessage;
				break;
			case StorageWizardType.VirtualDiskWizard:
				message = ExceptionResources.DefaultNewVirtualDiskWizardExceptionMessage;
				break;
			}
			ClusterLog.LogException(ex, "FSACException occurred when StorageWizard of type {0} executed.".FormatCurrentCulture(wizardType));
			throw new ClusterDefaultException(message, ex);
		}
	}
}
