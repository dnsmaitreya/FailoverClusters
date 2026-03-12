using System;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PIScsiTargetGroup : PGroup
{
	public PIScsiTargetGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.IScsiTarget)
	{
	}
}
