using System;

namespace FailoverClusters.Framework;

public class ClusterNodeServerInformationEventArgs : ClusterEventArgs
{
	public ServerInformation ServerInformation { get; internal set; }

	public ClusterNodeServerInformationEventArgs(Guid id, ServerInformation serverInformation, ClusterException exception)
		: base(id, exception)
	{
		ServerInformation = serverInformation;
	}
}

