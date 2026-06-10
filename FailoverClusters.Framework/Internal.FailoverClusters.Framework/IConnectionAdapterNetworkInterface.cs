using System;
using System.Collections.Generic;
using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal interface IConnectionAdapterNetworkInterface
{
	PNetworkInterface Open(Guid id);

	PNetworkInterface Open(string networkInterfaceName);

	void Load(PNetworkInterface networkInterface, NetworkInterfaceLoadSelection networkSelection);

	List<string> GetNodeDnsSuffixes(string nodeName);
}

