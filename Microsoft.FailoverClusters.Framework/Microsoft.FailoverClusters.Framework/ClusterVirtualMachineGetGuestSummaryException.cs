using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineGetGuestSummaryException : ClusterException
{
	public ClusterVirtualMachineGetGuestSummaryException()
		: this(string.Empty)
	{
	}

	public ClusterVirtualMachineGetGuestSummaryException(string virtualMachineName)
		: base(ExceptionResources.VirtualMachineGetGuestStatus_Default.FormatCurrentCulture(virtualMachineName), null)
	{
	}

	public ClusterVirtualMachineGetGuestSummaryException(string message, string virtualMachineName)
		: base(message.FormatCurrentCulture(virtualMachineName), null)
	{
	}

	public ClusterVirtualMachineGetGuestSummaryException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachineGetGuestStatus_Default.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	public ClusterVirtualMachineGetGuestSummaryException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	protected ClusterVirtualMachineGetGuestSummaryException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
