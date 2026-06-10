using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PCrossClusterDependencyOrchestratorResource : PResource
{
	public PCrossClusterDependencyOrchestratorResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.CrossClusterOrchestrator))
	{
	}
}

