using System;
using System.Runtime.Serialization;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public abstract class ClusterException : Exception
{
	public override string StackTrace => base.StackTrace;

	protected ClusterException()
		: this(string.Empty)
	{
	}

	protected ClusterException(string message)
		: base(message)
	{
	}

	protected ClusterException(string message, Exception innerException)
		: base(message, (innerException != null && innerException is ClusterDefaultException) ? innerException.InnerException : innerException)
	{
	}

	protected ClusterException(Exception innerException)
		: base(string.Empty, (innerException != null && innerException is ClusterDefaultException) ? innerException.InnerException : innerException)
	{
	}

	protected ClusterException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
