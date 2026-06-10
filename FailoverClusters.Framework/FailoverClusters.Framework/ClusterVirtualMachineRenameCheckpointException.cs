using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineRenameCheckpointException : ClusterDialogException
{
	public ClusterVirtualMachineRenameCheckpointException()
		: this(string.Empty, string.Empty)
	{
	}

	public ClusterVirtualMachineRenameCheckpointException(string virtualMachineName, string checkpointName)
		: base(ExceptionResources.VirtualMachineRenameCheckpoint_Default.FormatCurrentCulture(virtualMachineName, checkpointName), null)
	{
		base.Header = ExceptionResources.VirtualMachineRenameCheckpoint_Header;
	}

	public ClusterVirtualMachineRenameCheckpointException(string virtualMachineName, string checkpointName, string errorMessage)
		: base(ExceptionResources.VirtualMachineRenameCheckpoint_ErrorCode.FormatCurrentCulture(errorMessage, virtualMachineName, checkpointName), null)
	{
		base.Header = ExceptionResources.VirtualMachineRenameCheckpoint_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineRenameCheckpointException(string virtualMachineName, string checkpointName, int errorCode)
		: base(ExceptionResources.VirtualMachineRenameCheckpoint_ErrorCode.FormatCurrentCulture(errorCode, virtualMachineName, checkpointName), null)
	{
		base.Header = ExceptionResources.VirtualMachineRenameCheckpoint_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineRenameCheckpointException(string virtualMachineName, string checkpointName, Exception innerException)
		: base(ExceptionResources.VirtualMachineRenameCheckpoint_Default.FormatCurrentCulture(virtualMachineName, checkpointName), innerException)
	{
		base.Header = ExceptionResources.VirtualMachineRenameCheckpoint_Header;
	}

	public ClusterVirtualMachineRenameCheckpointException(string errorMessage)
		: base(errorMessage)
	{
		base.Header = ExceptionResources.VirtualMachineRenameCheckpoint_Header;
	}

	public ClusterVirtualMachineRenameCheckpointException(string errorMessage, Exception innerException)
		: base(errorMessage, innerException)
	{
		base.Header = ExceptionResources.VirtualMachineRenameCheckpoint_Header;
	}

	public ClusterVirtualMachineRenameCheckpointException(string message, string virtualMachineName, string checkpointName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName, checkpointName), innerException)
	{
		base.Header = ExceptionResources.VirtualMachineRenameCheckpoint_Header;
	}

	protected ClusterVirtualMachineRenameCheckpointException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}

