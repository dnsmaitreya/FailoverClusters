using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public static class DnsSupport
{
	public static bool IsNetworkNameReady(string serverName)
	{
		if (!NetworkHelper.CanPing(serverName))
		{
			return false;
		}
		switch (NetworkHelper.TryNetServerTransportEnum(serverName))
		{
		default:
			throw new NetworkNameNotReadyException(serverName);
		case NetApiStatus.ErrorAccessDenied:
		case NetApiStatus.ErrorBadNetPath:
			return false;
		case NetApiStatus.Success:
			return true;
		}
	}
}
