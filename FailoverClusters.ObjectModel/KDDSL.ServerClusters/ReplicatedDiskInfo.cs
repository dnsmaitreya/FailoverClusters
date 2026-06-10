using System;

namespace KDDSL.ServerClusters;

public class ReplicatedDiskInfo
{
	private ReplicatedDiskType diskType;

	private Guid clusterDiskResourceGuid;

	private Guid replicationGroupId;

	private string replicationGroupName;

	public string ReplicationGroupName => replicationGroupName;

	public Guid ReplicationGroupId => replicationGroupId;

	public Guid ClusterDiskResourceGuid => clusterDiskResourceGuid;

	public ReplicatedDiskType DiskType => diskType;

	public ReplicatedDiskInfo(ReplicatedDiskType diskType, Guid clusterDiskResourceGuid, Guid replicationGroupId, string replicationGroupName)
	{
		this.diskType = diskType;
		this.clusterDiskResourceGuid = clusterDiskResourceGuid;
		this.replicationGroupId = replicationGroupId;
		this.replicationGroupName = replicationGroupName;
	}
}
