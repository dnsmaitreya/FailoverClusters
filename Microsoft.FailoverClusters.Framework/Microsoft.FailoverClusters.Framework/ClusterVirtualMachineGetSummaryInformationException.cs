using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineGetSummaryInformationException : ClusterDialogException
{
	public ClusterVirtualMachineGetSummaryInformationException()
		: this(string.Empty)
	{
	}

	public ClusterVirtualMachineGetSummaryInformationException(string virtualMachineName)
		: base(ExceptionResources.VirtualMachineGetSummaryInformation_Default.FormatCurrentCulture(virtualMachineName), null)
	{
		base.Header = ExceptionResources.VirtualMachineGetSummaryInformation_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineGetSummaryInformationException(string virtualMachineName, int errorCode)
		: base(ExceptionResources.VirtualMachineGetSummaryInformation_ErrorCode.FormatCurrentCulture(errorCode, virtualMachineName), null)
	{
		base.Header = ExceptionResources.VirtualMachineGetSummaryInformation_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineGetSummaryInformationException(string message, string virtualMachineName)
		: base(message.FormatCurrentCulture(virtualMachineName), null)
	{
		base.Header = ExceptionResources.VirtualMachineGetSummaryInformation_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineGetSummaryInformationException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachineGetSummaryInformation_Default.FormatCurrentCulture(virtualMachineName), innerException)
	{
		base.Header = ExceptionResources.VirtualMachineGetSummaryInformation_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineGetSummaryInformationException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
		base.Header = ExceptionResources.VirtualMachineGetSummaryInformation_Header.FormatCurrentCulture(virtualMachineName);
	}

	protected ClusterVirtualMachineGetSummaryInformationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
