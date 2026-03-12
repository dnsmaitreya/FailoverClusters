using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterVMMonitoringResetException : ClusterDialogException
{
	public ClusterVMMonitoringResetException()
		: base(ExceptionResources.ClusterVMMonitoringResetException_Default)
	{
	}

	public ClusterVMMonitoringResetException(string message)
		: base(message)
	{
	}

	public ClusterVMMonitoringResetException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected ClusterVMMonitoringResetException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
