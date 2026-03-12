using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineGetGuestStatusException : ClusterException
{
	public ClusterVirtualMachineGetGuestStatusException()
		: this(string.Empty)
	{
	}

	public ClusterVirtualMachineGetGuestStatusException(string virtualMachineName)
		: base(ExceptionResources.VirtualMachineGetGuestStatus_Default.FormatCurrentCulture(virtualMachineName), null)
	{
	}

	public ClusterVirtualMachineGetGuestStatusException(string message, string virtualMachineName)
		: base(message.FormatCurrentCulture(virtualMachineName), null)
	{
	}

	public ClusterVirtualMachineGetGuestStatusException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachineGetGuestStatus_Default.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	public ClusterVirtualMachineGetGuestStatusException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	protected ClusterVirtualMachineGetGuestStatusException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
