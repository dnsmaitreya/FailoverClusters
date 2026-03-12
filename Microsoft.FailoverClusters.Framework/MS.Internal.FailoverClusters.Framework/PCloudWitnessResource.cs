using System;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PCloudWitnessResource : PResource
{
	public PCloudWitnessResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.CloudWitness))
	{
	}
}
