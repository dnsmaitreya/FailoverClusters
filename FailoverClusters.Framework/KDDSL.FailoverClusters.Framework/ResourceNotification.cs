using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class ResourceNotification : Notification
{
	public ResourceNotification(ClusterEventArgs payload)
		: base(payload)
	{
	}
}

