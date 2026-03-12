using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.FailoverClusters.UI.Common.Reactive.Linq;
using Microsoft.FailoverClusters.UI.Common.Services;
using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Generic;
using Microsoft.Management.Infrastructure.Options;

namespace MS.Internal.FailoverClusters.Framework;

internal class NfsAdapter : FileShareDataSourceBase
{
	private const string NfsShareSubscriptionQueryFormat = "SELECT * FROM MSFT_NfsShareEvent WHERE Share.NetworkName='{0}'";

	private const string EnumerateSharesSelect = "select Name, Path, NetworkName, IsClustered from msft_NfsShare where networkname='{0}'";

	private const string RawNfsNamespace = "root\\Microsoft\\Windows\\Nfs";

	private const string EventSharePropertyName = "Share";

	private const string WmiClass = "MSFT_NfsShare";

	private const string IsClusteredPropertyName = "IsClustered";

	private const string NetworkNamePropertyName = "NetworkName";

	private const string EventTypePropertyName = "Event";

	public NfsAdapter()
		: base(FileShareProtocol.Nfs)
	{
	}

	public override IObservable<IFileShareDataItem> GetSharesObservable(string nodeFqdn, string scopeName)
	{
		try
		{
			CimSession cimSession = ServiceContainer.Container.Resolve<ICimUtilities>(Array.Empty<object>()).GetCimSession(nodeFqdn);
			CimObservableErrorType cimObservableErrorType = ServiceContainer.Container.Resolve<ICimUtilities>(Array.Empty<object>()).CanConnectToCim(cimSession, "root\\Microsoft\\Windows\\Nfs", "MSFT_NfsShare");
			if (cimObservableErrorType != 0)
			{
				return Observable.Return(new FileShareErrorItem(cimObservableErrorType, base.SupportedProtocol, "MSFT_NfsShare", scopeName));
			}
			return ((IObservable<IFileShareDataItem>)(from result in cimSession.QueryInstancesAsync("root\\Microsoft\\Windows\\Nfs", "WQL", "select Name, Path, NetworkName, IsClustered from msft_NfsShare where networkname='{0}'".FormatCurrentCulture(scopeName), new CimOperationOptions
				{
					EnableMethodResultStreaming = true
				}).OfType<CimInstance>()
				select TransformCimInstance(result, ShareEventType.None, nodeFqdn))).Catch((Func<CimException, IObservable<IFileShareDataItem>>)delegate(CimException ex)
			{
				ClusterLog.LogException(ex, "NFS share enumeration observable was terminated with an exception");
				return Observable.Return(new FileShareErrorItem(CimObservableErrorType.ExceptionObserved, base.SupportedProtocol, "MSFT_NfsShare", scopeName, ex));
			});
		}
		catch (CimException exception)
		{
			ClusterLog.LogException(exception);
			return Observable.Return(new FileShareErrorItem(CimObservableErrorType.SubscriptionFailure, base.SupportedProtocol, "MSFT_NfsShare", scopeName, exception));
		}
	}

	public override void SetShare(FileShare fileShare)
	{
		throw new NotSupportedException();
	}

	public override IObservable<IFileShareDataItem> GetSubscriptionObservable(string nodeFqdn, string serverName)
	{
		return GetShareEventObservable(nodeFqdn, "root\\Microsoft\\Windows\\Nfs", "MSFT_NfsShare", "SELECT * FROM MSFT_NfsShareEvent WHERE Share.NetworkName='{0}'".FormatInvariantCulture(serverName), "Share", "Event", TransformCimInstance);
	}

	protected override FileShare TransformCimInstance(CimInstance instance, ShareEventType eventType, string connectionFqdn)
	{
		CimKeyedCollection<CimProperty> cimInstanceProperties = instance.CimInstanceProperties;
		FileShare fileShare = new FileShare();
		object cimPropertyValue = ServiceContainer.Container.Resolve<ICimUtilities>(Array.Empty<object>()).GetCimPropertyValue(cimInstanceProperties, "Name");
		if (cimPropertyValue != null)
		{
			fileShare.Name = cimPropertyValue.ToString();
		}
		fileShare.Path = cimInstanceProperties["Path"].Value.ToString();
		object cimPropertyValue2 = ServiceContainer.Container.Resolve<ICimUtilities>(Array.Empty<object>()).GetCimPropertyValue(cimInstanceProperties, "NetworkName");
		if (cimPropertyValue2 != null)
		{
			fileShare.ServerName = cimPropertyValue2.ToString();
		}
		object cimPropertyValue3 = ServiceContainer.Container.Resolve<ICimUtilities>(Array.Empty<object>()).GetCimPropertyValue(cimInstanceProperties, "IsClustered");
		if (cimPropertyValue3 != null)
		{
			fileShare.ContinuousAvailability = Convert.ToBoolean(cimPropertyValue3, CultureInfo.InvariantCulture);
		}
		fileShare.EventType = eventType;
		fileShare.Protocol = FileShareProtocol.Nfs;
		fileShare.ConnectionFqdn = connectionFqdn;
		return fileShare;
	}

	protected override CimInstance GetCimInstance(CimSession session, string serverName, string shareName, string sharePath)
	{
		List<CimProperty> list = new List<CimProperty>();
		if (!string.IsNullOrWhiteSpace(sharePath))
		{
			list.Add(CimProperty.Create("Path", sharePath, CimFlags.Key));
		}
		else
		{
			list.Add(CimProperty.Create("Name", shareName, CimFlags.Property));
		}
		return FileShareDataSourceBase.GetCimInstance(session, "MSFT_NfsShare", "root\\Microsoft\\Windows\\Nfs", list);
	}
}
