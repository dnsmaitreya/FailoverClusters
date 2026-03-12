using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterGCException : ClusterException
{
	public ClusterGCException()
		: this(ExceptionResources.ClusterGCMemoryCleanUp)
	{
	}

	public ClusterGCException(string message)
		: this(message, null)
	{
	}

	public ClusterGCException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterGCException(Exception innerException)
		: this(ExceptionResources.ClusterGCMemoryCleanUp, innerException)
	{
	}

	protected ClusterGCException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
