using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterCimException : ClusterException
{
	public ClusterCimException()
		: this(ExceptionResources.CimException_Text)
	{
	}

	public ClusterCimException(string message)
		: this(message, null)
	{
	}

	public ClusterCimException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterCimException(Exception innerException)
		: this(ExceptionResources.ClusterGCMemoryCleanUp, innerException)
	{
	}

	protected ClusterCimException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}

