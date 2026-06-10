using System;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PNetworkAddressTranslatorResource : PResource
{
	public PNetworkAddressTranslatorResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.NetworkAddressTranslator))
	{
	}
}

