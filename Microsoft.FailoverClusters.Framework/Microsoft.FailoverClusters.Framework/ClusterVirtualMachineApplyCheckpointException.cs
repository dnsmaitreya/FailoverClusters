using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineApplyCheckpointException : ClusterDialogException
{
	public ClusterVirtualMachineApplyCheckpointException()
		: this(string.Empty, string.Empty)
	{
	}

	public ClusterVirtualMachineApplyCheckpointException(string virtualMachineName, string checkpointName)
		: base(ExceptionResources.VirtualMachineApplyCheckpoint_Default.FormatCurrentCulture(virtualMachineName, checkpointName), null)
	{
		base.Header = ExceptionResources.VirtualMachineApplyCheckpoint_Header;
	}

	public ClusterVirtualMachineApplyCheckpointException(string virtualMachineName, string checkpointName, string errorMessage)
		: base(ExceptionResources.VirtualMachineApplyCheckpoint_ErrorCode.FormatCurrentCulture(errorMessage, virtualMachineName, checkpointName), null)
	{
		base.Header = ExceptionResources.VirtualMachineApplyCheckpoint_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineApplyCheckpointException(string virtualMachineName, string checkpointName, int errorCode)
		: base(ExceptionResources.VirtualMachineApplyCheckpoint_ErrorCode.FormatCurrentCulture(errorCode, virtualMachineName, checkpointName), null)
	{
		base.Header = ExceptionResources.VirtualMachineApplyCheckpoint_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineApplyCheckpointException(string virtualMachineName, string checkpointName, Exception innerException)
		: base(ExceptionResources.VirtualMachineApplyCheckpoint_Default.FormatCurrentCulture(virtualMachineName, checkpointName), innerException)
	{
		base.Header = ExceptionResources.VirtualMachineApplyCheckpoint_Header;
	}

	public ClusterVirtualMachineApplyCheckpointException(string errorMessage)
		: base(errorMessage)
	{
		base.Header = ExceptionResources.VirtualMachineApplyCheckpoint_Header;
	}

	public ClusterVirtualMachineApplyCheckpointException(string errorMessage, Exception innerException)
		: base(errorMessage, innerException)
	{
		base.Header = ExceptionResources.VirtualMachineApplyCheckpoint_Header;
	}

	public ClusterVirtualMachineApplyCheckpointException(string message, string virtualMachineName, string checkpointName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName, checkpointName), innerException)
	{
		base.Header = ExceptionResources.VirtualMachineApplyCheckpoint_Header;
	}

	protected ClusterVirtualMachineApplyCheckpointException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
