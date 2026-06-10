using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class NodeNotification : Notification
{
	public NodeNotification(ClusterEventArgs payload)
		: base(payload)
	{
	}
}

