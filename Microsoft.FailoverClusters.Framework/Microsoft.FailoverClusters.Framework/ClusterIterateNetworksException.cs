using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterIterateNetworksException : ClusterException
{
	public ClusterIterateNetworksException()
		: this((Exception)null)
	{
	}

	public ClusterIterateNetworksException(string message)
		: base(message)
	{
	}

	public ClusterIterateNetworksException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterIterateNetworksException(Exception innerException)
		: base(ExceptionResources.ClusterIterateNetworks_Default, innerException)
	{
	}

	protected ClusterIterateNetworksException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
