using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineDeleteCheckpointTreeException : ClusterDialogException
{
	public ClusterVirtualMachineDeleteCheckpointTreeException()
		: this(string.Empty, string.Empty)
	{
	}

	public ClusterVirtualMachineDeleteCheckpointTreeException(string virtualMachineName, string checkpointName)
		: base(ExceptionResources.VirtualMachineDeleteCheckpointTree_Default.FormatCurrentCulture(virtualMachineName, checkpointName), null)
	{
		base.Header = ExceptionResources.VirtualMachineDeleteCheckpointTree_Header;
	}

	public ClusterVirtualMachineDeleteCheckpointTreeException(string virtualMachineName, string checkpointName, string errorMessage)
		: base(ExceptionResources.VirtualMachineDeleteCheckpointTree_ErrorCode.FormatCurrentCulture(errorMessage, virtualMachineName, checkpointName), null)
	{
		base.Header = ExceptionResources.VirtualMachineDeleteCheckpointTree_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineDeleteCheckpointTreeException(string virtualMachineName, string checkpointName, int errorCode)
		: base(ExceptionResources.VirtualMachineDeleteCheckpointTree_ErrorCode.FormatCurrentCulture(errorCode, virtualMachineName, checkpointName), null)
	{
		base.Header = ExceptionResources.VirtualMachineDeleteCheckpointTree_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineDeleteCheckpointTreeException(string virtualMachineName, string checkpointName, Exception innerException)
		: base(ExceptionResources.VirtualMachineDeleteCheckpointTree_Default.FormatCurrentCulture(virtualMachineName, checkpointName), innerException)
	{
		base.Header = ExceptionResources.VirtualMachineDeleteCheckpointTree_Header;
	}

	public ClusterVirtualMachineDeleteCheckpointTreeException(string errorMessage)
		: base(errorMessage)
	{
		base.Header = ExceptionResources.VirtualMachineDeleteCheckpointTree_Header;
	}

	public ClusterVirtualMachineDeleteCheckpointTreeException(string errorMessage, Exception innerException)
		: base(errorMessage, innerException)
	{
		base.Header = ExceptionResources.VirtualMachineDeleteCheckpointTree_Header;
	}

	public ClusterVirtualMachineDeleteCheckpointTreeException(string message, string virtualMachineName, string checkpointName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName, checkpointName), innerException)
	{
		base.Header = ExceptionResources.VirtualMachineDeleteCheckpointTree_Header;
	}

	protected ClusterVirtualMachineDeleteCheckpointTreeException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}

