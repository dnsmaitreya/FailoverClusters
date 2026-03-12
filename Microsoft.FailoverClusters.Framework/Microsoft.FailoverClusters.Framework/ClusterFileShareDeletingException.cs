using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterFileShareDeletingException : ClusterDialogException
{
	public ClusterFileShareDeletingException()
		: this(ExceptionResources.ClusterFileShareDeleting_Default, null)
	{
	}

	public ClusterFileShareDeletingException(string message)
		: this(message, null)
	{
	}

	public ClusterFileShareDeletingException(Exception innerException)
		: this(ExceptionResources.ClusterFileShareDeleting_Default, innerException)
	{
	}

	public ClusterFileShareDeletingException(string message, Exception innerException)
		: base(message, innerException)
	{
		base.Header = ExceptionResources.ClusterFileShareDeleting_Header;
	}

	protected ClusterFileShareDeletingException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
