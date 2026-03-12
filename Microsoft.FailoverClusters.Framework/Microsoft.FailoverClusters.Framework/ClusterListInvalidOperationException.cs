using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterListInvalidOperationException : ClusterException
{
	public ClusterListInvalidOperationException()
		: base(ExceptionResources.RTCAddNotAllowed_Default)
	{
	}

	public ClusterListInvalidOperationException(string message)
		: base(message)
	{
	}

	public ClusterListInvalidOperationException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected ClusterListInvalidOperationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
