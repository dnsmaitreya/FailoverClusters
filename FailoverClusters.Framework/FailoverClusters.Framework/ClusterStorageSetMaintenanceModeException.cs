using System;
using System.Runtime.Serialization;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterStorageSetMaintenanceModeException : ClusterException
{
	public ClusterStorageSetMaintenanceModeException()
	{
	}

	public ClusterStorageSetMaintenanceModeException(string message)
		: base(message)
	{
	}

	public ClusterStorageSetMaintenanceModeException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected ClusterStorageSetMaintenanceModeException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

