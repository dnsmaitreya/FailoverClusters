using System;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PCrossClusterDependencyOrchestratorResource : PResource
{
	public PCrossClusterDependencyOrchestratorResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.CrossClusterOrchestrator))
	{
	}
}
