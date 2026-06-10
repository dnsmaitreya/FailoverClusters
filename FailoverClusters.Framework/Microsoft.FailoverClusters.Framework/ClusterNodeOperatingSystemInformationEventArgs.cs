using System;

namespace FailoverClusters.Framework;

public class ClusterNodeOperatingSystemInformationEventArgs : ClusterEventArgs
{
	public NodeOperatingSystemInformation OperatingSystemInformation { get; internal set; }

	public ClusterNodeOperatingSystemInformationEventArgs(Guid id, NodeOperatingSystemInformation operatingSystemInformation, ClusterException exception)
		: base(id, exception)
	{
		OperatingSystemInformation = operatingSystemInformation;
	}
}

