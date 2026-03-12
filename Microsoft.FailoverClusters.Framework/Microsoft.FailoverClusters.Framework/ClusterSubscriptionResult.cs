using Microsoft.Management.Infrastructure;

namespace Microsoft.FailoverClusters.Framework;

internal class ClusterSubscriptionResult
{
	public ClusterSubscriptionResultType ResultType { get; private set; }

	public CimSubscriptionResult CimSubscriptionResult { get; private set; }

	public ClusterSubscriptionResult(ClusterSubscriptionResultType resultType, CimSubscriptionResult result)
	{
		CimSubscriptionResult = result;
		ResultType = resultType;
	}
}
