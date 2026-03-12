using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterDependencyRelationshipEventArgs : ClusterEventArgs
{
	public string DependencyRelationship { get; internal set; }

	public ClusterDependencyRelationshipEventArgs(Guid id, string relationship, ClusterException exception)
		: base(id, exception)
	{
		DependencyRelationship = relationship;
	}
}
