using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineTakeCheckpointException : ClusterDialogException
{
	public ClusterVirtualMachineTakeCheckpointException()
		: this(string.Empty, string.Empty)
	{
	}

	public ClusterVirtualMachineTakeCheckpointException(string virtualMachineName)
		: base(ExceptionResources.VirtualMachineTakeCheckpoint_Default.FormatCurrentCulture(virtualMachineName), null)
	{
		base.Header = ExceptionResources.VirtualMachineTakeCheckpoint_Header;
	}

	public ClusterVirtualMachineTakeCheckpointException(string virtualMachineName, string errorMessage)
		: base(ExceptionResources.VirtualMachineTakeCheckpoint_ErrorCode.FormatCurrentCulture(errorMessage, virtualMachineName), null)
	{
		base.Header = ExceptionResources.VirtualMachineTakeCheckpoint_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineTakeCheckpointException(string virtualMachineName, int errorCode)
		: base(ExceptionResources.VirtualMachineTakeCheckpoint_ErrorCode.FormatCurrentCulture(errorCode, virtualMachineName), null)
	{
		base.Header = ExceptionResources.VirtualMachineTakeCheckpoint_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineTakeCheckpointException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachineTakeCheckpoint_Default.FormatCurrentCulture(virtualMachineName), innerException)
	{
		base.Header = ExceptionResources.VirtualMachineTakeCheckpoint_Header;
	}

	public ClusterVirtualMachineTakeCheckpointException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
		base.Header = ExceptionResources.VirtualMachineTakeCheckpoint_Header;
	}

	protected ClusterVirtualMachineTakeCheckpointException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}

