using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineDeleteStateException : ClusterDialogException
{
	public ClusterVirtualMachineDeleteStateException()
		: this(string.Empty)
	{
	}

	public ClusterVirtualMachineDeleteStateException(string virtualMachineName)
		: base(ExceptionResources.VirtualMachineDeleteSavedState_Default.FormatCurrentCulture(virtualMachineName), null)
	{
		base.Header = ExceptionResources.VirtualMachineDeleteSavedState_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineDeleteStateException(string virtualMachineName, int errorCode)
		: base(ExceptionResources.VirtualMachineDeleteSavedState_ErrorCode.FormatCurrentCulture(errorCode, virtualMachineName), null)
	{
		base.Header = ExceptionResources.VirtualMachineDeleteSavedState_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineDeleteStateException(string message, string virtualMachineName)
		: base(message.FormatCurrentCulture(virtualMachineName), null)
	{
		base.Header = ExceptionResources.VirtualMachineDeleteSavedState_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineDeleteStateException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachineDeleteSavedState_Default.FormatCurrentCulture(virtualMachineName), innerException)
	{
		base.Header = ExceptionResources.VirtualMachineDeleteSavedState_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineDeleteStateException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
		base.Header = ExceptionResources.VirtualMachineDeleteSavedState_Header.FormatCurrentCulture(virtualMachineName);
	}

	protected ClusterVirtualMachineDeleteStateException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

