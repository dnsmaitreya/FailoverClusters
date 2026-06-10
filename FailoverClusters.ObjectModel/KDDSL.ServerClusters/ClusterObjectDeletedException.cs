using System;
using System.Runtime.Serialization;

namespace KDDSL.ServerClusters;

[Serializable]
public class ClusterObjectDeletedException : ClusterBaseException
{
	protected ClusterObjectDeletedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	protected ClusterObjectDeletedException()
	{
	}

	public ClusterObjectDeletedException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterObjectDeletedException(string message)
		: base(message)
	{
	}
}
