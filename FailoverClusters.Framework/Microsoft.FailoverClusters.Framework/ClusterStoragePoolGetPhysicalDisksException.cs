using System;
using System.Runtime.Serialization;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterStoragePoolGetPhysicalDisksException : ClusterException
{
	public ClusterStoragePoolGetPhysicalDisksException()
	{
	}

	public ClusterStoragePoolGetPhysicalDisksException(string message)
		: base(message)
	{
	}

	public ClusterStoragePoolGetPhysicalDisksException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected ClusterStoragePoolGetPhysicalDisksException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

