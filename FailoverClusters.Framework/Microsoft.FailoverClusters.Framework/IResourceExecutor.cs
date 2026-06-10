using System;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

internal interface IResourceExecutor
{
	void ExecuteOnResource(SafeClusterHandle clusterHandle, ClusterAccessRights clusterAccessRights, Guid id, string name, Action<SafeClusterResourceHandle> actionOnResource);

	Guid GetResourceId(SafeClusterResourceHandle resourceHandle, string resourceName);
}

