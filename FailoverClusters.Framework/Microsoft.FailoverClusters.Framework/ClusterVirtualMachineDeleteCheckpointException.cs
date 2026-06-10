using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineDeleteCheckpointException : ClusterDialogException
{
	public ClusterVirtualMachineDeleteCheckpointException()
		: this(string.Empty, string.Empty)
	{
	}

	public ClusterVirtualMachineDeleteCheckpointException(string virtualMachineName, string checkpointName)
		: base(ExceptionResources.VirtualMachineDeleteCheckpoint_Default.FormatCurrentCulture(virtualMachineName, checkpointName), null)
	{
		base.Header = ExceptionResources.VirtualMachineDeleteCheckpoint_Header;
	}

	public ClusterVirtualMachineDeleteCheckpointException(string virtualMachineName, string checkpointName, string errorMessage)
		: base(ExceptionResources.VirtualMachineDeleteCheckpoint_ErrorCode.FormatCurrentCulture(errorMessage, virtualMachineName, checkpointName), null)
	{
		base.Header = ExceptionResources.VirtualMachineDeleteCheckpoint_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineDeleteCheckpointException(string virtualMachineName, string checkpointName, int errorCode)
		: base(ExceptionResources.VirtualMachineDeleteCheckpoint_ErrorCode.FormatCurrentCulture(errorCode, virtualMachineName, checkpointName), null)
	{
		base.Header = ExceptionResources.VirtualMachineDeleteCheckpoint_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineDeleteCheckpointException(string virtualMachineName, string checkpointName, Exception innerException)
		: base(ExceptionResources.VirtualMachineDeleteCheckpoint_Default.FormatCurrentCulture(virtualMachineName, checkpointName), innerException)
	{
		base.Header = ExceptionResources.VirtualMachineDeleteCheckpoint_Header;
	}

	public ClusterVirtualMachineDeleteCheckpointException(string errorMessage)
		: base(errorMessage)
	{
		base.Header = ExceptionResources.VirtualMachineDeleteCheckpoint_Header;
	}

	public ClusterVirtualMachineDeleteCheckpointException(string errorMessage, Exception innerException)
		: base(errorMessage, innerException)
	{
		base.Header = ExceptionResources.VirtualMachineDeleteCheckpoint_Header;
	}

	public ClusterVirtualMachineDeleteCheckpointException(string message, string virtualMachineName, string checkpointName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName, checkpointName), innerException)
	{
		base.Header = ExceptionResources.VirtualMachineDeleteCheckpoint_Header;
	}

	protected ClusterVirtualMachineDeleteCheckpointException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}

