using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal abstract class Notification
{
	private ClusterEventArgs payload;

	public ClusterEventArgs Payload => payload;

	public Notification(ClusterEventArgs payload)
	{
		this.payload = payload;
	}

	public override string ToString()
	{
		return Payload.GetType().ToString();
	}
}

