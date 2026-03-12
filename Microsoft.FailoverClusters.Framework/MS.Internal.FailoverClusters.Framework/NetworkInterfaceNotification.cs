using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class NetworkInterfaceNotification : Notification
{
	public NetworkInterfaceNotification(ClusterEventArgs payload)
		: base(payload)
	{
	}
}
