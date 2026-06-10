using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PVolumeShadowCopyServiceTaskResource : PResource
{
	public PVolumeShadowCopyServiceTaskResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.VolumeShadowCopyServiceTask))
	{
	}
}

