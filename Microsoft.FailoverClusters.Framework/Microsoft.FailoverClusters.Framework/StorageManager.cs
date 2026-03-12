using System;
using System.Collections.Generic;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

internal class StorageManager
{
	private readonly PCluster cluster;

	public IEnumerable<ClusterDisk> AvailableDisks { get; private set; }

	public StorageManager(PCluster cluster)
	{
		this.cluster = cluster;
	}

	public void DetermineClusterableDisks()
	{
		DetermineClusterableDisks(Guid.Empty, all: false);
	}

	public void DetermineClusterableDisks(bool all)
	{
		DetermineClusterableDisks(Guid.Empty, all);
	}

	public void DetermineClusterableDisks(Guid poolId)
	{
		DetermineClusterableDisks(poolId, all: false);
	}

	public void DetermineClusterableDisks(Guid poolId, bool all)
	{
		AvailableDisks = cluster.GetAvailableDisks(poolId, all);
	}

	public IEnumerable<PResource> CreateDiskResources(IEnumerable<ClusterDisk> clusterableDisks)
	{
		return cluster.CreateDiskResources(clusterableDisks);
	}
}
