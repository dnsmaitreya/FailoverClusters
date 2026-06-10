using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterEnumerateResourcesException : ClusterException
{
	public ClusterEnumerateResourcesException()
		: this((Exception)null)
	{
	}

	public ClusterEnumerateResourcesException(string message)
		: base(message)
	{
	}

	public ClusterEnumerateResourcesException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterEnumerateResourcesException(Exception innerException)
		: base(ExceptionResources.ClusterEnumerateResources_Default, innerException)
	{
	}

	protected ClusterEnumerateResourcesException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

