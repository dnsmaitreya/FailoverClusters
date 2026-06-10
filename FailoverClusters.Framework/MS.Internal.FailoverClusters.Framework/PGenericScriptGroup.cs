using System;
using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PGenericScriptGroup : PGroup
{
	public PGenericScriptGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.GenericScript)
	{
	}
}

