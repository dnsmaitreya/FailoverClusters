using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class NetworkInterfaceNotification : Notification
{
	public NetworkInterfaceNotification(ClusterEventArgs payload)
		: base(payload)
	{
	}
}

