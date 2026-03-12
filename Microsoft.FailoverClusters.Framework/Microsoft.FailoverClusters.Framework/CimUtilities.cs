using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Generic;
using Microsoft.Management.Infrastructure.Options;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

public class CimUtilities : ICimUtilities
{
	private const string QueryDialect = "WQL";

	public CimSession GetCimSession(string serverName)
	{
		CimSessionOptions cimSessionOptions = new CimSessionOptions();
		cimSessionOptions.AddDestinationCredentials(new CimCredential(ImpersonatedAuthenticationMechanism.Negotiate));
		return CimSession.Create(serverName, cimSessionOptions);
	}

	public object GetCimPropertyValue(CimKeyedCollection<CimProperty> propertyCollection, string propertyName)
	{
		Exceptions.ThrowIfNull(propertyCollection, "propertyCollection");
		CimProperty cimProperty = propertyCollection[propertyName];
		if (cimProperty != null && cimProperty.Value != null)
		{
			return cimProperty.Value;
		}
		return null;
	}

	public CimObservableErrorType CanConnectToCim(CimSession session, string wmiNamespace, string wmiClass)
	{
		return CanConnectToCim(session, wmiNamespace, wmiClass, 3, TimeSpan.FromSeconds(2.0));
	}

	public CimObservableErrorType CanConnectToCim(CimSession session, string wmiNamespace, string wmiClass, int attempts, TimeSpan retryFrequency)
	{
		Exceptions.ThrowIfNull(session, "session");
		Exceptions.ThrowIfNullOrEmpty(wmiNamespace, "wmiNamespace");
		Exceptions.ThrowIfNullOrEmpty(wmiClass, "wmiClass");
		if (!NetworkHelper.CanPing(session.ComputerName))
		{
			return CimObservableErrorType.ConnectionFailure;
		}
		CimObservableErrorType cimObservableErrorType = CimObservableErrorType.Unknown;
		for (int i = 0; i < attempts; i++)
		{
			cimObservableErrorType = ((!session.TestConnection(out var _, out var exception)) ? CimObservableErrorType.ConnectionFailure : CimObservableErrorType.None);
			if (cimObservableErrorType == CimObservableErrorType.None)
			{
				break;
			}
			if (exception != null)
			{
				ClusterLog.LogException(exception, "Connection attempt: {0} to namespace '{1}'.", i, wmiNamespace);
			}
			Thread.Sleep(retryFrequency);
		}
		if (cimObservableErrorType == CimObservableErrorType.None)
		{
			try
			{
				session.GetClass(wmiNamespace, wmiClass);
			}
			catch (CimException ex)
			{
				if (ex.NativeErrorCode == NativeErrorCode.ServerLimitsExceeded)
				{
					cimObservableErrorType = CimObservableErrorType.ServerQuotaReached;
				}
				else if (ex.NativeErrorCode == NativeErrorCode.InvalidClass)
				{
					cimObservableErrorType = CimObservableErrorType.ClassNotFound;
				}
			}
		}
		return cimObservableErrorType;
	}

	public IEnumerable<CimInstance> EnumerateInstances(CimSession session, string namespaceName, string className)
	{
		Exceptions.ThrowIfNull(session, "session");
		return session.EnumerateInstances(namespaceName, className);
	}

	public IObservable<CimSubscriptionResult> SubscribeAsync(CimSession cimSession, string namespaceName, string query)
	{
		Exceptions.ThrowIfNull(cimSession, "cimSession");
		return cimSession.SubscribeAsync(namespaceName, "WQL", query, new CimSubscriptionDeliveryOptions(CimSubscriptionDeliveryType.Pull));
	}

	public bool TestConnection(CimSession cimSession)
	{
		Exceptions.ThrowIfNull(cimSession, "cimSession");
		return cimSession.TestConnection();
	}
}
