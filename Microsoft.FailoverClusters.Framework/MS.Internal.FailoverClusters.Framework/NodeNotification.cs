using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class NodeNotification : Notification
{
	public NodeNotification(ClusterEventArgs payload)
		: base(payload)
	{
	}
}
