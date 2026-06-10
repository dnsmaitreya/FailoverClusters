using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterObjectDeletingException : ClusterException
{
	public ClusterObjectDeletingException()
		: this((Exception)null)
	{
	}

	public ClusterObjectDeletingException(string message)
		: base(message)
	{
	}

	public ClusterObjectDeletingException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterObjectDeletingException(Exception innerException)
		: base(ExceptionResources.ClusterObjectDeleting_Default, innerException)
	{
	}

	protected ClusterObjectDeletingException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}

