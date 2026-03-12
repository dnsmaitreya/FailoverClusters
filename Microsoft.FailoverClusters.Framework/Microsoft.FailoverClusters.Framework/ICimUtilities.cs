using System;
using System.Collections.Generic;
using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Generic;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

public interface ICimUtilities
{
	CimSession GetCimSession(string serverName);

	object GetCimPropertyValue(CimKeyedCollection<CimProperty> propertyCollection, string propertyName);

	CimObservableErrorType CanConnectToCim(CimSession session, string wmiNamespace, string wmiClass);

	CimObservableErrorType CanConnectToCim(CimSession session, string wmiNamespace, string wmiClass, int attempts, TimeSpan retryFrequency);

	IEnumerable<CimInstance> EnumerateInstances(CimSession session, string namespaceName, string className);

	IObservable<CimSubscriptionResult> SubscribeAsync(CimSession cimSession, string namespaceName, string query);

	bool TestConnection(CimSession cimSession);
}
