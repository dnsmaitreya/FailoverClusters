using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterEnumerateNetworkException : ClusterException
{
	public ClusterEnumerateNetworkException()
		: this((Exception)null)
	{
	}

	public ClusterEnumerateNetworkException(string message)
		: base(message)
	{
	}

	public ClusterEnumerateNetworkException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterEnumerateNetworkException(Exception innerException)
		: base(ExceptionResources.ClusterEnumerateNetworks_Default, innerException)
	{
	}

	protected ClusterEnumerateNetworkException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

