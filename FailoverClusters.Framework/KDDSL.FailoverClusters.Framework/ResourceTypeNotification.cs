using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class ResourceTypeNotification : Notification
{
	public ResourceTypeNotification(ClusterEventArgs payload)
		: base(payload)
	{
	}

	public override string ToString()
	{
		return base.Payload.ToString();
	}
}

