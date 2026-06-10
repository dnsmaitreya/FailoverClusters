using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineRevertCheckpointException : ClusterDialogException
{
	public ClusterVirtualMachineRevertCheckpointException()
		: this(string.Empty, string.Empty)
	{
	}

	public ClusterVirtualMachineRevertCheckpointException(string virtualMachineName)
		: base(ExceptionResources.VirtualMachineRevertCheckpoint_Default.FormatCurrentCulture(virtualMachineName), null)
	{
		base.Header = ExceptionResources.VirtualMachineTakeCheckpoint_Header;
	}

	public ClusterVirtualMachineRevertCheckpointException(string virtualMachineName, string errorMessage)
		: base(ExceptionResources.VirtualMachineRevertCheckpoint_ErrorCode.FormatCurrentCulture(errorMessage, virtualMachineName), null)
	{
		base.Header = ExceptionResources.VirtualMachineRevertCheckpoint_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineRevertCheckpointException(string virtualMachineName, int errorCode)
		: base(ExceptionResources.VirtualMachineRevertCheckpoint_ErrorCode.FormatCurrentCulture(errorCode, virtualMachineName), null)
	{
		base.Header = ExceptionResources.VirtualMachineTakeCheckpoint_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineRevertCheckpointException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachineRevertCheckpoint_Default.FormatCurrentCulture(virtualMachineName), innerException)
	{
		base.Header = ExceptionResources.VirtualMachineRevertCheckpoint_Header;
	}

	public ClusterVirtualMachineRevertCheckpointException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
		base.Header = ExceptionResources.VirtualMachineTakeCheckpoint_Header;
	}

	protected ClusterVirtualMachineRevertCheckpointException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}

