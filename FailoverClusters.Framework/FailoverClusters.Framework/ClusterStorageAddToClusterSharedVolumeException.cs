using System;
using System.Runtime.Serialization;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterStorageAddToClusterSharedVolumeException : ClusterException
{
	public ClusterStorageAddToClusterSharedVolumeException()
	{
	}

	public ClusterStorageAddToClusterSharedVolumeException(string message)
		: base(message)
	{
	}

	public ClusterStorageAddToClusterSharedVolumeException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected ClusterStorageAddToClusterSharedVolumeException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

