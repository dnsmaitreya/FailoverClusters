using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterControlCodeException : ClusterException
{
	public int ControlCode { get; set; }

	public ClusterControlCodeException(int controlCode)
		: base(ExceptionResources.FailedToExecuteControlCode_Default.FormatCurrentCulture(controlCode))
	{
		ControlCode = controlCode;
	}

	public ClusterControlCodeException()
		: this(0)
	{
	}

	public ClusterControlCodeException(string message)
		: base(message)
	{
	}

	public ClusterControlCodeException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterControlCodeException(int controlCode, Exception innerException)
		: base(ExceptionResources.FailedToExecuteControlCode_Default.FormatCurrentCulture(controlCode), innerException)
	{
		ControlCode = controlCode;
	}

	protected ClusterControlCodeException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info?.AddValue("ControlCode", ControlCode);
	}
}
