using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterClosedException : ClusterException
{
	public ClusterClosedException()
		: base(ExceptionResources.ClusterClosed_Default)
	{
	}

	public ClusterClosedException(string message)
		: base(message)
	{
	}

	public ClusterClosedException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected ClusterClosedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
