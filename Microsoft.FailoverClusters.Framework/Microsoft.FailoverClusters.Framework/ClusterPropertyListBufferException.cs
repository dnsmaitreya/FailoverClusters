using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterPropertyListBufferException : ClusterException
{
	public ClusterPropertyListBufferException()
		: base(ExceptionResources.ErrorGettingPropertyListBuffer_Default)
	{
	}

	public ClusterPropertyListBufferException(string message)
		: base(message)
	{
	}

	public ClusterPropertyListBufferException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterPropertyListBufferException(Exception innerException)
		: base(ExceptionResources.ErrorGettingPropertyListBuffer_Default, innerException)
	{
	}

	protected ClusterPropertyListBufferException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
