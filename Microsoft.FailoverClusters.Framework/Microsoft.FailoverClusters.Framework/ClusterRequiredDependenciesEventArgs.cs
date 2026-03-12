using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterRequiredDependenciesEventArgs : ClusterEventArgs
{
	public RequiredDependencies RequiredDependencies { get; internal set; }

	public ClusterRequiredDependenciesEventArgs(Guid id, RequiredDependencies requiredDependencies, ClusterException exception)
		: base(id, exception)
	{
		RequiredDependencies = requiredDependencies;
	}
}
