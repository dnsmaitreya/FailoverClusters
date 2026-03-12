using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterInvalidDispatcherOperationException : ClusterException
{
	public ClusterInvalidDispatcherOperationException()
		: base(ExceptionResources.InvalidDispatcherOperation_Default, null)
	{
	}

	public ClusterInvalidDispatcherOperationException(string message)
		: base(message)
	{
	}

	public ClusterInvalidDispatcherOperationException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterInvalidDispatcherOperationException(Exception innerException)
		: base(ExceptionResources.InvalidDispatcherOperation_Default, innerException)
	{
	}

	protected ClusterInvalidDispatcherOperationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
