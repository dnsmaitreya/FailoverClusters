using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterEnumerateNodeException : ClusterException
{
	public ClusterEnumerateNodeException()
		: this((Exception)null)
	{
	}

	public ClusterEnumerateNodeException(string message)
		: base(message)
	{
	}

	public ClusterEnumerateNodeException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterEnumerateNodeException(Exception innerException)
		: base(ExceptionResources.ClusterEnumerateNodes_Default, innerException)
	{
	}

	protected ClusterEnumerateNodeException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
