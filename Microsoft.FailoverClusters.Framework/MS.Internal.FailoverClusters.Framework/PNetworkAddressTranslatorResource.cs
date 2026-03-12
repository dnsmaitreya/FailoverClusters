using System;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PNetworkAddressTranslatorResource : PResource
{
	public PNetworkAddressTranslatorResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.NetworkAddressTranslator))
	{
	}
}
