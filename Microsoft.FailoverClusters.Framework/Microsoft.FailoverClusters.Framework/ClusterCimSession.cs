using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.FailoverClusters.UI.Common.Reactive.Linq;
using Microsoft.FailoverClusters.UI.Common.Services;
using Microsoft.Management.Infrastructure;

namespace Microsoft.FailoverClusters.Framework;

internal class ClusterCimSession
{
	private const string ClusterClass = "MSFTCluster_Cluster";

	private const string NodeClass = "MSFTCluster_Node";

	private const string ClusterNamespace = "root/microsoft/windows/cluster";

	private readonly List<string> nodeNames;

	private int currentNodeIndex;

	private CimSession currentSession;

	public IList<string> Nodes => nodeNames.AsReadOnly();

	protected ClusterCimSession(IEnumerable<string> nodeFqdns)
	{
		nodeNames = nodeFqdns.ToList();
	}

	public static ClusterCimSession Create(string serverName)
	{
		ICimUtilities cimUtilities = ServiceContainer.Container.Resolve<ICimUtilities>(Array.Empty<object>());
		CimSession cimSession = GetCimSession(serverName, "root/microsoft/windows/cluster", "MSFTCluster_Cluster");
		if (cimSession == null)
		{
			throw new ClusterCimException();
		}
		IEnumerable<string> enumerable = from n in cimUtilities.EnumerateInstances(cimSession, "root/microsoft/windows/cluster", "MSFTCluster_Node")
			select cimUtilities.GetCimPropertyValue(n.CimInstanceProperties, "Fqdn").ToString();
		if (!enumerable.Any())
		{
			throw new ClusterCimException();
		}
		return new ClusterCimSession(enumerable);
	}

	public IObservable<ClusterSubscriptionResult> SubscribeAsync(string namespaceName, string className, string query)
	{
		return (from csr in ServiceContainer.Container.Resolve<ICimUtilities>(Array.Empty<object>()).SubscribeAsync(GetLiveSession(namespaceName, className), namespaceName, query)
			select new ClusterSubscriptionResult(ClusterSubscriptionResultType.NormalResult, csr)).Catch((CimException ex) => Observable.Return(new ClusterSubscriptionResult(ClusterSubscriptionResultType.Reconnected, null)).Concat(SubscribeAsync(namespaceName, className, query)));
	}

	public CimSession GetLiveSession(string namespaceName, string className)
	{
		ICimUtilities cimUtilities = ServiceContainer.Container.Resolve<ICimUtilities>(Array.Empty<object>());
		if (currentSession == null || !cimUtilities.TestConnection(currentSession))
		{
			int num = (currentNodeIndex + 1) % nodeNames.Count;
			int num2 = num;
			currentSession = null;
			do
			{
				CimSession cimSession = cimUtilities.GetCimSession(nodeNames[num]);
				if (cimSession != null)
				{
					currentNodeIndex = num;
					currentSession = cimSession;
					return cimSession;
				}
				num = (num + 1) % nodeNames.Count;
			}
			while (num != num2);
		}
		if (currentSession == null)
		{
			throw new ClusterCimException();
		}
		return currentSession;
	}

	private static CimSession GetCimSession(string serverName, string namespaceName, string className)
	{
		ICimUtilities cimUtilities = ServiceContainer.Container.Resolve<ICimUtilities>(Array.Empty<object>());
		CimSession cimSession = cimUtilities.GetCimSession(serverName);
		if (cimUtilities.CanConnectToCim(cimSession, namespaceName, className) != 0)
		{
			return null;
		}
		return cimSession;
	}
}
