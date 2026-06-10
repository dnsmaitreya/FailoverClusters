using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterNotificationException : ClusterException
{
	public ClusterNotificationException()
		: this(0)
	{
	}

	public ClusterNotificationException(int errorCode)
		: this(errorCode, null)
	{
	}

	public ClusterNotificationException(int errorCode, Exception innerException)
		: this(ExceptionResources.GetClusterNotifyInvalidError_Default.FormatCurrentCulture(errorCode), innerException)
	{
	}

	public ClusterNotificationException(string message)
		: this(message, null)
	{
	}

	public ClusterNotificationException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected ClusterNotificationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

