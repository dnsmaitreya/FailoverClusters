using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterEnumerateNetworkInterfaceException : ClusterException
{
	public ClusterEnumerateNetworkInterfaceException()
		: this((Exception)null)
	{
	}

	public ClusterEnumerateNetworkInterfaceException(string message)
		: base(message)
	{
	}

	public ClusterEnumerateNetworkInterfaceException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterEnumerateNetworkInterfaceException(Exception innerException)
		: base(ExceptionResources.ClusterEnumerateNetworkInterfaces_Default, innerException)
	{
	}

	protected ClusterEnumerateNetworkInterfaceException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

