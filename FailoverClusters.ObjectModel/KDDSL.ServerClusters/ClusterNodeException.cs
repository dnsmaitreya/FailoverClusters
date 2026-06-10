using System;
using System.Runtime.Serialization;

namespace KDDSL.ServerClusters;

[Serializable]
public abstract class ClusterNodeException : ClusterBaseException
{
	protected ClusterNodeException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	protected ClusterNodeException(string message, Exception inner)
		: base(message, inner)
	{
	}

	protected ClusterNodeException(string message)
		: base(message)
	{
	}

	protected ClusterNodeException()
	{
	}
}
