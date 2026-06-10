using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterRegistryException : ClusterException
{
	public ClusterRegistryException()
		: this((Exception)null)
	{
	}

	public ClusterRegistryException(string message)
		: base(message)
	{
	}

	public ClusterRegistryException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterRegistryException(Exception innerException)
		: base(ExceptionResources.ClusterRegistry_Default, innerException)
	{
	}

	protected ClusterRegistryException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

