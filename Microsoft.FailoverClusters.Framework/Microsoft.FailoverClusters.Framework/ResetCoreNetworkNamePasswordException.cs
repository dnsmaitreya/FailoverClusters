using System;
using System.Runtime.Serialization;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ResetCoreNetworkNamePasswordException : ClusterException
{
	public ResetCoreNetworkNamePasswordException()
	{
	}

	public ResetCoreNetworkNamePasswordException(string message)
		: base(message)
	{
	}

	public ResetCoreNetworkNamePasswordException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected ResetCoreNetworkNamePasswordException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
