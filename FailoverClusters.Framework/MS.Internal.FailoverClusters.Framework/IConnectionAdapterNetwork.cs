using System;
using System.Collections.Generic;
using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal interface IConnectionAdapterNetwork
{
	PNetwork Open(Guid id);

	PNetwork Open(string networkName);

	void Rename(Guid id, string newName);

	void Load(PNetwork network, NetworkLoadSelection networkSelection);

	IEnumerable<PNetwork> GetAll(bool nullElementOnError);
}

