using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class NetworkNotification : Notification
{
	public NetworkNotification(ClusterEventArgs payload)
		: base(payload)
	{
	}
}

