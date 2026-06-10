using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class NetworkNotification : Notification
{
	public NetworkNotification(ClusterEventArgs payload)
		: base(payload)
	{
	}
}

