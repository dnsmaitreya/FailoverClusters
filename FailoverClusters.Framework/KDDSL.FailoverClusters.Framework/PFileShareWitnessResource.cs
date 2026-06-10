using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PFileShareWitnessResource : PResource
{
	public PFileShareWitnessResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.FileShareWitness))
	{
	}
}

