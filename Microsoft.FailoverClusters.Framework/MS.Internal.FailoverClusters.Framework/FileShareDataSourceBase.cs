using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.FailoverClusters.UI.Common.Reactive.Linq;
using Microsoft.FailoverClusters.UI.Common.Services;
using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Options;

namespace MS.Internal.FailoverClusters.Framework;

internal abstract class FileShareDataSourceBase
{
	protected const string ShareNamePropertyName = "Name";

	protected const string SharePathPropertyName = "Path";

	protected const string TargetInstancePropertyName = "TargetInstance";

	protected const string IpcShareName = "IPC$";

	protected const string WqlDialect = "WQL";

	private const string MaxLatencyParameter = "__MI_SUBSCRIPTIONDELIVERYOPTIONS_SET_MAXIMUM_LATENCY";

	private TimeSpan MaxLatencyTime = TimeSpan.FromSeconds(10.0);

	public FileShareProtocol SupportedProtocol { get; private set; }

	protected FileShareDataSourceBase(FileShareProtocol protocol)
	{
		SupportedProtocol = protocol;
	}

	public virtual void DeleteShare(FileShare fileShare)
	{
		if (fileShare.Deleted)
		{
			return;
		}
		using CimSession cimSession = ServiceContainer.Container.Resolve<ICimUtilities>(Array.Empty<object>()).GetCimSession(fileShare.ConnectionFqdn);
		CimInstance cimInstance = GetCimInstance(cimSession, fileShare.ServerName, fileShare.Name, fileShare.Path);
		try
		{
			if (cimInstance != null)
			{
				cimSession.DeleteInstance(cimInstance);
			}
		}
		catch (CimException exception)
		{
			ClusterLog.LogException(exception, "Failed to delete file share");
		}
	}

	public virtual void DeleteShares(IEnumerable<FileShare> shares)
	{
		foreach (FileShare share in shares)
		{
			DeleteShare(share);
		}
	}

	public FileShare GetShare(string serverName, string netName)
	{
		FileShare fileShare = new FileShare();
		PopulateShare(fileShare, serverName, netName);
		return fileShare;
	}

	public virtual bool PopulateShare(FileShare fileShare, string serverName, string netName)
	{
		if (fileShare.Protocol != FileShareProtocol.Unknown && fileShare.Protocol != SupportedProtocol)
		{
			return false;
		}
		using CimSession session = ServiceContainer.Container.Resolve<ICimUtilities>(Array.Empty<object>()).GetCimSession(serverName);
		CimInstance cimInstance = GetCimInstance(session, serverName, netName, string.Empty);
		if (cimInstance != null)
		{
			fileShare.CopyFrom(TransformCimInstance(cimInstance, ShareEventType.None, serverName));
			return true;
		}
		return false;
	}

	public abstract void SetShare(FileShare fileShare);

	public abstract IObservable<IFileShareDataItem> GetSubscriptionObservable(string nodeFqdn, string serverName);

	public abstract IObservable<IFileShareDataItem> GetSharesObservable(string nodeFqdn, string scopeName);

	protected static CimInstance GetCimInstance(CimSession session, string wmiClass, string wmiNamespace, IEnumerable<CimProperty> searchProperties)
	{
		if (ServiceContainer.Container.Resolve<ICimUtilities>(Array.Empty<object>()).CanConnectToCim(session, wmiNamespace, wmiClass) == CimObservableErrorType.None)
		{
			CimInstance instance = new CimInstance(wmiClass);
			searchProperties.ForEach(delegate(CimProperty property)
			{
				instance.CimInstanceProperties.Add(property);
			});
			try
			{
				return session.GetInstance(wmiNamespace, instance);
			}
			catch (CimException exception)
			{
				ClusterLog.LogException(exception, "Error getting CIM instances");
				return null;
			}
		}
		return null;
	}

	protected static IEnumerable<CimInstance> SearchCimInstances(CimSession session, string wmiClass, string wmiNamespace, string query)
	{
		if (ServiceContainer.Container.Resolve<ICimUtilities>(Array.Empty<object>()).CanConnectToCim(session, wmiNamespace, wmiClass) == CimObservableErrorType.None)
		{
			try
			{
				return session.QueryInstances(wmiNamespace, "WQL", query);
			}
			catch (CimException exception)
			{
				ClusterLog.LogException(exception, "Error getting CIM instances");
				return Enumerable.Empty<CimInstance>();
			}
		}
		return Enumerable.Empty<CimInstance>();
	}

	protected IObservable<IFileShareDataItem> GetShareEventObservable(string nodeFqdn, string wmiNamespace, string wmiClass, string watchQuery, string sharePropertyName, string eventTypePropertyName, Func<CimInstance, ShareEventType, string, FileShare> transformOperation)
	{
		CimSession cimSession = ServiceContainer.Container.Resolve<ICimUtilities>(Array.Empty<object>()).GetCimSession(nodeFqdn);
		if (ServiceContainer.Container.Resolve<ICimUtilities>(Array.Empty<object>()).CanConnectToCim(cimSession, wmiNamespace, wmiClass) == CimObservableErrorType.None)
		{
			CimSubscriptionDeliveryOptions cimSubscriptionDeliveryOptions = new CimSubscriptionDeliveryOptions(CimSubscriptionDeliveryType.Pull);
			cimSubscriptionDeliveryOptions.SetInterval("__MI_SUBSCRIPTIONDELIVERYOPTIONS_SET_MAXIMUM_LATENCY", MaxLatencyTime, 0u);
			return (from result in cimSession.SubscribeAsync(wmiNamespace, "WQL", watchQuery, cimSubscriptionDeliveryOptions).OfType<CimSubscriptionResult>()
				select transformOperation((CimInstance)result.Instance.CimInstanceProperties[sharePropertyName].Value, (ShareEventType)((uint)result.Instance.CimInstanceProperties[eventTypePropertyName].Value + 1), nodeFqdn)).Catch(delegate(CimException ex)
			{
				ClusterLog.LogException(ex, "Subscribing for event failed for query '{0}", watchQuery);
				return Observable.Return((IFileShareDataItem)new FileShareErrorItem(CimObservableErrorType.ExceptionObserved, SupportedProtocol, wmiClass, nodeFqdn, ex));
			});
		}
		ClusterLog.LogInfo("Could not connect to CIM for {0}", nodeFqdn);
		return Observable.Return(new FileShareErrorItem(CimObservableErrorType.ConnectionFailure, SupportedProtocol, wmiClass, nodeFqdn));
	}

	protected abstract CimInstance GetCimInstance(CimSession session, string serverName, string shareName, string sharePath);

	protected abstract FileShare TransformCimInstance(CimInstance instance, ShareEventType eventType, string connectionFqdn);
}
