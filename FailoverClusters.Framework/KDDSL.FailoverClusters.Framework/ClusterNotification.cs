using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class ClusterNotification : Notification
{
	public ClusterNotification(ClusterEventArgs payload)
		: base(payload)
	{
	}
}

