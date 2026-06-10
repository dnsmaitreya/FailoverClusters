using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class ResourceNotification : Notification
{
	public ResourceNotification(ClusterEventArgs payload)
		: base(payload)
	{
	}
}

