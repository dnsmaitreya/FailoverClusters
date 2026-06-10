using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterNotificationNotStartedException : ClusterException
{
	public ClusterNotificationNotStartedException()
		: base(ExceptionResources.ClusterNotificationNotStarted_Default)
	{
	}

	public ClusterNotificationNotStartedException(string message)
		: base(message)
	{
	}

	public ClusterNotificationNotStartedException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected ClusterNotificationNotStartedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

