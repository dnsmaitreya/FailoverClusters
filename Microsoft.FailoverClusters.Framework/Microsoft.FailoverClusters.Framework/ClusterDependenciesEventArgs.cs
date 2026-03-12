using System;
using System.Collections.Generic;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterDependenciesEventArgs : ClusterEventArgs
{
	public IEnumerable<Guid> Dependencies { get; internal set; }

	public ClusterDependenciesEventArgs(Guid id, IEnumerable<Guid> dependencies, ClusterException exception)
		: base(id, exception)
	{
		Dependencies = dependencies;
	}
}
