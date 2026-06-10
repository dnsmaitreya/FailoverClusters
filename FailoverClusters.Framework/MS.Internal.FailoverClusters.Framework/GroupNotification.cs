using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class GroupNotification : Notification
{
	public GroupNotification(ClusterEventArgs payload)
		: base(payload)
	{
	}
}

