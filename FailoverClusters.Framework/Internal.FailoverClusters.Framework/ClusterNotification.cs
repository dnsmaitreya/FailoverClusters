using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class ClusterNotification : Notification
{
	public ClusterNotification(ClusterEventArgs payload)
		: base(payload)
	{
	}
}

