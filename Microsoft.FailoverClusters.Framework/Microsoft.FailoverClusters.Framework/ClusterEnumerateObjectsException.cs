using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterEnumerateObjectsException : ClusterException
{
	public ClusterEnumerateObjectsException()
		: this((Exception)null)
	{
	}

	public ClusterEnumerateObjectsException(string message)
		: base(message)
	{
	}

	public ClusterEnumerateObjectsException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterEnumerateObjectsException(Exception innerException)
		: base(ExceptionResources.ClusterEnumerateObjects_Default, innerException)
	{
	}

	protected ClusterEnumerateObjectsException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
