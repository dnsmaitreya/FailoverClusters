using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.FailoverClusters.UI.Common;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

public class ReplicationInfo
{
	private readonly PCluster cluster;

	private long logSize;

	private IList<StorageResource> replicationStorageResources;

	public IList<Guid> ReplicationPrivateStorageResources;

	public ReplicationType ReplicationType { get; private set; }

	public ReplicationStatus ReplicationStatus { get; private set; }

	public string ReplicationGroupName { get; private set; }

	public StorageResource StorageResource { get; internal set; }

	public long LogSize
	{
		get
		{
			return logSize;
		}
		set
		{
			if (logSize == value || !(StorageResource != null))
			{
				return;
			}
			StorageResource.ExecuteMethod(delegate(ILockable lockObject)
			{
				if (lockObject != null)
				{
					((PStorageResource)lockObject.Owner).SetReplicationLogSize(value);
				}
			}, null, LockAccess.Reader);
		}
	}

	public long ContainerSize { get; internal set; }

	public int MultiplicationFactor { get; internal set; }

	public long MinLogSize { get; internal set; }

	public bool IsConsistencyEnabled { get; internal set; }

	public Guid ReplicationGroupId => cluster.CacheManager.GetReplicationGroupIdFromName(ReplicationGroupName);

	public List<Guid> ReplicationGroupIds
	{
		get
		{
			List<Guid> list = new List<Guid> { cluster.CacheManager.GetReplicationGroupIdFromName(ReplicationGroupName) };
			foreach (StorageResource replicationStorageResource in ReplicationStorageResources)
			{
				Guid replicationGroupIdFromResourceId = cluster.CacheManager.GetReplicationGroupIdFromResourceId(replicationStorageResource.Id);
				if (replicationGroupIdFromResourceId != Guid.Empty && !list.Contains(replicationGroupIdFromResourceId))
				{
					list.Add(replicationGroupIdFromResourceId);
				}
			}
			return list;
		}
	}

	public string Description { get; private set; }

	public IList<StorageResource> ReplicationStorageResources
	{
		get
		{
			if (replicationStorageResources == null && ReplicationPrivateStorageResources != null)
			{
				replicationStorageResources = new List<StorageResource>(from storageResource in ReplicationPrivateStorageResources.Select(delegate(Guid privateResource)
					{
						using ClusterLock clusterLock = cluster.CacheManager.Get(privateResource, ClusterIdentityType.Resource, LockAccess.Reader);
						if (clusterLock != null && clusterLock.Owner is PStorageResource)
						{
							return (StorageResource)((PResource)clusterLock.Owner).GetProxy();
						}
						return null;
					})
					where storageResource != null
					select storageResource);
			}
			return replicationStorageResources;
		}
	}

	internal ReplicationInfo(PCluster cluster, string replicationGroupName, string description, ReplicationType replicationType, ReplicationStatus replicationStatus, IList<Guid> replicatedResources, long logSize, long containerSize, int multiplicationFactor, long minLogSize, bool isConsistencyEnabled)
	{
		this.cluster = cluster;
		ReplicationType = replicationType;
		ReplicationGroupName = replicationGroupName;
		ReplicationStatus = replicationStatus;
		Description = description;
		ReplicationPrivateStorageResources = replicatedResources;
		this.logSize = logSize;
		ContainerSize = containerSize;
		MultiplicationFactor = multiplicationFactor;
		MinLogSize = minLogSize;
		IsConsistencyEnabled = isConsistencyEnabled;
	}

	public ReplicationInfo Clone()
	{
		return new ReplicationInfo(cluster, ReplicationGroupName, Description, ReplicationType, ReplicationStatus, new List<Guid>(ReplicationPrivateStorageResources), LogSize, ContainerSize, MultiplicationFactor, MinLogSize, IsConsistencyEnabled);
	}
}
