using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class GroupNotification : Notification
{
	public GroupNotification(ClusterEventArgs payload)
		: base(payload)
	{
	}
}

