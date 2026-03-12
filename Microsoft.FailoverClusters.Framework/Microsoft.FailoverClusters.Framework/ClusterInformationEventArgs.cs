using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterInformationEventArgs : ClusterEventArgs
{
	public string Information { get; internal set; }

	public ClusterInformationEventArgs(Guid id)
		: this(id, string.Empty)
	{
	}

	public ClusterInformationEventArgs(Guid id, string information)
		: base(id, null)
	{
		Information = information;
	}
}
