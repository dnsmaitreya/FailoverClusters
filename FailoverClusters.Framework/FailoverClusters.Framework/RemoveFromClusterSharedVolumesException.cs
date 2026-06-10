using System;
using System.Runtime.Serialization;

namespace FailoverClusters.Framework;

[Serializable]
public class RemoveFromClusterSharedVolumesException : ClusterException
{
	public RemoveFromClusterSharedVolumesException()
	{
	}

	public RemoveFromClusterSharedVolumesException(string message)
		: base(message)
	{
	}

	public RemoveFromClusterSharedVolumesException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected RemoveFromClusterSharedVolumesException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

