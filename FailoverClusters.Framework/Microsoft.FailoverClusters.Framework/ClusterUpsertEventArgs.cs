using System;
using System.Diagnostics;

namespace FailoverClusters.Framework;

public class ClusterUpsertEventArgs : ClusterAddedEventArgs
{
	[DebuggerNonUserCode]
	public ClusterUpsertEventArgs(Guid id, string name, int? objectType, string objectTypeName, Guid parentId, ClusterException exception)
		: base(id, name, objectType, objectTypeName, parentId, exception)
	{
	}
}

