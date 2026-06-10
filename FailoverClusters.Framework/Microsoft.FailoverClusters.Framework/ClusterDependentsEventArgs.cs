using System;
using System.Collections.Generic;

namespace FailoverClusters.Framework;

public class ClusterDependentsEventArgs : ClusterEventArgs
{
	public IEnumerable<Guid> Dependents { get; internal set; }

	public bool IsChild { get; internal set; }

	public ClusterDependentsEventArgs(Guid id, IEnumerable<Guid> dependents, ClusterException exception, bool isChild)
		: base(id, exception)
	{
		Dependents = dependents;
		IsChild = isChild;
	}

	internal ClusterDependentsEventArgs(Guid id, IEnumerable<Guid> dependants, ClusterException exception)
		: base(id, exception)
	{
		Dependents = dependants;
	}
}

