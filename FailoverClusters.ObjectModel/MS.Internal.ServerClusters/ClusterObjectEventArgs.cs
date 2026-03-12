using System;

namespace MS.Internal.ServerClusters;

public class ClusterObjectEventArgs : EventArgs
{
	private string clusterObject;

	private Guid clusterObjectId;

	private ClusterObjectEventType type;

	public ClusterObjectEventType EventType => type;

	public Guid ClusterObjectId => clusterObjectId;

	public string ClusterObject => clusterObject;

	public ClusterObjectEventArgs(string clusterObject, Guid clusterObjectId, ClusterObjectEventType type)
	{
		this.clusterObject = clusterObject;
		this.clusterObjectId = clusterObjectId;
		this.type = type;
	}
}
