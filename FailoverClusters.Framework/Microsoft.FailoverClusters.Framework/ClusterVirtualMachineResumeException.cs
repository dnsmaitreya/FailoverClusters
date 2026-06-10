using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineResumeException : ClusterDialogException
{
	public ClusterVirtualMachineResumeException()
		: this(string.Empty)
	{
	}

	public ClusterVirtualMachineResumeException(string virtualMachineName)
		: base(ExceptionResources.VirtualMachineResume_Default.FormatCurrentCulture(virtualMachineName), null)
	{
		base.Header = ExceptionResources.VirtualMachineResume_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineResumeException(string virtualMachineName, int errorCode)
		: base(ExceptionResources.VirtualMachineResume_ErrorCode.FormatCurrentCulture(errorCode, virtualMachineName), null)
	{
		base.Header = ExceptionResources.VirtualMachineResume_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineResumeException(string message, string virtualMachineName)
		: base(message.FormatCurrentCulture(virtualMachineName), null)
	{
		base.Header = ExceptionResources.VirtualMachineResume_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineResumeException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachineResume_Default.FormatCurrentCulture(virtualMachineName), innerException)
	{
		base.Header = ExceptionResources.VirtualMachineResume_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineResumeException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
		base.Header = ExceptionResources.VirtualMachineResume_Header.FormatCurrentCulture(virtualMachineName);
	}

	protected ClusterVirtualMachineResumeException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

