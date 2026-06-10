using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PIScsiNameServiceResource : PResource
{
	public PIScsiNameServiceResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.IScsiNameService))
	{
	}
}

