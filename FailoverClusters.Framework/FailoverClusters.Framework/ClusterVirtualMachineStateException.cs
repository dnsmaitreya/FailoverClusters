using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineStateException : ClusterDialogException
{
	public ClusterVirtualMachineStateException()
		: this(string.Empty)
	{
	}

	public ClusterVirtualMachineStateException(string virtualMachineName)
		: base(ExceptionResources.VirtualMachineState_Default.FormatCurrentCulture(virtualMachineName), null)
	{
		base.Header = ExceptionResources.VirtualMachineState_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineStateException(string virtualMachineName, int errorCode)
		: base(ExceptionResources.VirtualMachineState_ErrorCode.FormatCurrentCulture(errorCode, virtualMachineName), null)
	{
		base.Header = ExceptionResources.VirtualMachineState_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineStateException(string virtualMachineName, string errorMessage)
		: base(ExceptionResources.VirtualMachineState_ErrorCode.FormatCurrentCulture(errorMessage, virtualMachineName), null)
	{
		base.Header = ExceptionResources.VirtualMachineState_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineStateException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachineState_Default.FormatCurrentCulture(virtualMachineName), innerException)
	{
		base.Header = ExceptionResources.VirtualMachineState_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineStateException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
		base.Header = ExceptionResources.VirtualMachineState_Header.FormatCurrentCulture(virtualMachineName);
	}

	protected ClusterVirtualMachineStateException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

