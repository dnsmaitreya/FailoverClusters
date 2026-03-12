using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachinePauseException : ClusterDialogException
{
	public ClusterVirtualMachinePauseException()
		: this(string.Empty)
	{
	}

	public ClusterVirtualMachinePauseException(string virtualMachineName)
		: base(ExceptionResources.VirtualMachinePause_Default.FormatCurrentCulture(virtualMachineName), null)
	{
		base.Header = ExceptionResources.VirtualMachinePause_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachinePauseException(string virtualMachineName, int errorCode)
		: base(ExceptionResources.VirtualMachinePause_ErrorCode.FormatCurrentCulture(errorCode, virtualMachineName), null)
	{
		base.Header = ExceptionResources.VirtualMachinePause_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachinePauseException(string message, string virtualMachineName)
		: base(message.FormatCurrentCulture(virtualMachineName), null)
	{
		base.Header = ExceptionResources.VirtualMachinePause_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachinePauseException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachinePause_Default.FormatCurrentCulture(virtualMachineName), innerException)
	{
		base.Header = ExceptionResources.VirtualMachinePause_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachinePauseException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
		base.Header = ExceptionResources.VirtualMachinePause_Header.FormatCurrentCulture(virtualMachineName);
	}

	protected ClusterVirtualMachinePauseException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
