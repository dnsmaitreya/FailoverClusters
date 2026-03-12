using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineSetOfflineActionException : ClusterException
{
	public ClusterVirtualMachineSetOfflineActionException()
		: this(string.Empty)
	{
	}

	public ClusterVirtualMachineSetOfflineActionException(string virtualMachineName)
		: base(ExceptionResources.VirtualMachineSetOfflineAction_Default.FormatCurrentCulture(virtualMachineName), null)
	{
	}

	public ClusterVirtualMachineSetOfflineActionException(string message, string virtualMachineName)
		: base(message.FormatCurrentCulture(virtualMachineName), null)
	{
	}

	public ClusterVirtualMachineSetOfflineActionException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachineSetOfflineAction_Default.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	public ClusterVirtualMachineSetOfflineActionException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	protected ClusterVirtualMachineSetOfflineActionException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
