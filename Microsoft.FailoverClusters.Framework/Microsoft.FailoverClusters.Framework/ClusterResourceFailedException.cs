using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterResourceFailedException : ClusterException
{
	public ClusterResourceFailedException()
		: this((Exception)null)
	{
	}

	public ClusterResourceFailedException(string message)
		: base(message)
	{
	}

	public ClusterResourceFailedException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterResourceFailedException(Exception innerException)
		: base(ExceptionResources.ClusterResourceFailed_Default, innerException)
	{
	}

	protected ClusterResourceFailedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
