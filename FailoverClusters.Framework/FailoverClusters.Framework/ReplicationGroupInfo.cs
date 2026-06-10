using System;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class ReplicationGroupInfo
{
	public Guid ClusterGroupId { get; private set; }

	public Guid ReplicationGroupId { get; private set; }

	public ReplicationGroupRole Role { get; private set; }

	internal ReplicationGroupInfo(Guid clusterGroupId, Guid replicationGroupId, ReplicationGroupRole role)
	{
		ClusterGroupId = clusterGroupId;
		ReplicationGroupId = replicationGroupId;
		Role = role;
	}
}

