using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterCreateNotifyPortException : ClusterException
{
	public ClusterCreateNotifyPortException()
		: this((Exception)null)
	{
	}

	public ClusterCreateNotifyPortException(string message)
		: base(message)
	{
	}

	public ClusterCreateNotifyPortException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterCreateNotifyPortException(Exception innerException)
		: base(ExceptionResources.ClusterCreateNotifyPort_Default, innerException)
	{
	}

	protected ClusterCreateNotifyPortException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
