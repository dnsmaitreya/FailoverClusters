using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineGetReplicationStatusException : ClusterException
{
	public ClusterVirtualMachineGetReplicationStatusException()
		: this(string.Empty)
	{
	}

	public ClusterVirtualMachineGetReplicationStatusException(string virtualMachineName)
		: base(ExceptionResources.VirtualMachineGetReplicationStatus_Default.FormatCurrentCulture(virtualMachineName), null)
	{
	}

	public ClusterVirtualMachineGetReplicationStatusException(string message, string virtualMachineName)
		: base(message.FormatCurrentCulture(virtualMachineName), null)
	{
	}

	public ClusterVirtualMachineGetReplicationStatusException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachineGetReplicationStatus_Default.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	public ClusterVirtualMachineGetReplicationStatusException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	protected ClusterVirtualMachineGetReplicationStatusException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
