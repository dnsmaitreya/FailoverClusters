using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterForwardedEventArgs : ClusterEventArgs
{
	public const int None = 0;

	public const int VMCsvDependentChanged = 1;

	public const int VMPrivateProperties = 2;

	public const int VMCommonProperties = 3;

	public const int FileShareErrors = 4;

	public ClusterEventArgs ForwardedPayload { get; internal set; }

	public object ClusterElementType { get; internal set; }

	public int Key { get; internal set; }

	public ClusterForwardedEventArgs(Guid id, object clusterElementType, ClusterEventArgs forwardedPayload)
		: this(id, clusterElementType, forwardedPayload, 0)
	{
	}

	public ClusterForwardedEventArgs(Guid id, object clusterElementType, ClusterEventArgs forwardedPayload, int key)
		: base(id, null)
	{
		ForwardedPayload = forwardedPayload;
		Key = key;
		ClusterElementType = clusterElementType;
	}
}
