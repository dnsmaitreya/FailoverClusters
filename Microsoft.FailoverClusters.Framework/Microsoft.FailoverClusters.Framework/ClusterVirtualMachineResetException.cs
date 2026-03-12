using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineResetException : ClusterDialogException
{
	public ClusterVirtualMachineResetException(string virtualMachineName)
		: base(ExceptionResources.VirtualMachineReset_Default.FormatCurrentCulture(virtualMachineName), null)
	{
		base.Header = ExceptionResources.VirtualMachineReset_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineResetException(string virtualMachineName, int errorCode)
		: base(ExceptionResources.VirtualMachineReset_ErrorCode.FormatCurrentCulture(errorCode, virtualMachineName), null)
	{
		base.Header = ExceptionResources.VirtualMachineReset_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineResetException(string message, string virtualMachineName)
		: base(message.FormatCurrentCulture(virtualMachineName), null)
	{
		base.Header = ExceptionResources.VirtualMachineReset_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineResetException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachineReset_Default.FormatCurrentCulture(virtualMachineName), innerException)
	{
		base.Header = ExceptionResources.VirtualMachineReset_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineResetException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
		base.Header = ExceptionResources.VirtualMachineReset_Header.FormatCurrentCulture(virtualMachineName);
	}

	protected ClusterVirtualMachineResetException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
