using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterIterateResourcesException : ClusterException
{
	public ClusterIterateResourcesException()
		: this((Exception)null)
	{
	}

	public ClusterIterateResourcesException(string message)
		: base(message)
	{
	}

	public ClusterIterateResourcesException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterIterateResourcesException(Exception innerException)
		: base(ExceptionResources.ClusterIterateResources_Default, innerException)
	{
	}

	protected ClusterIterateResourcesException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

