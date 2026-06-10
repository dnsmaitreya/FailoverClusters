using System;

namespace FailoverClusters.Framework;

internal class ClusterQuorumChangedEventArgs : ClusterEventArgs
{
	public QuorumConfigurationPrivate QuorumConfiguration { get; private set; }

	public ClusterQuorumChangedEventArgs(Guid id, QuorumConfigurationPrivate quorumConfiguration)
		: base(id, null)
	{
		QuorumConfiguration = quorumConfiguration;
	}
}

