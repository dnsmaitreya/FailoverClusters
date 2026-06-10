using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterIterateNetworkInterfacesException : ClusterException
{
	public ClusterIterateNetworkInterfacesException()
		: this((Exception)null)
	{
	}

	public ClusterIterateNetworkInterfacesException(string message)
		: base(message)
	{
	}

	public ClusterIterateNetworkInterfacesException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterIterateNetworkInterfacesException(Exception innerException)
		: base(ExceptionResources.ClusterIterateNetworks_Default, innerException)
	{
	}

	protected ClusterIterateNetworkInterfacesException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

