using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterAlreadyOpenException : ClusterDialogException
{
	public ClusterAlreadyOpenException()
		: base(ExceptionResources.AlreadyOpen_Default)
	{
		base.Header = ExceptionResources.AlreadyOpen_Header;
	}

	public ClusterAlreadyOpenException(string message)
		: base(message)
	{
		base.Header = ExceptionResources.AlreadyOpen_Header;
	}

	public ClusterAlreadyOpenException(string message, Exception innerException)
		: base(message, innerException)
	{
		base.Header = ExceptionResources.AlreadyOpen_Header;
	}

	protected ClusterAlreadyOpenException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
