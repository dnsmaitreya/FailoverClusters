using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineGetCheckpointException : ClusterException
{
	public ClusterVirtualMachineGetCheckpointException()
		: this("<Unknown>")
	{
	}

	public ClusterVirtualMachineGetCheckpointException(string virtualMachineName)
		: base(ExceptionResources.VirtualMachineGetCheckpoint_Default.FormatCurrentCulture(virtualMachineName), null)
	{
	}

	public ClusterVirtualMachineGetCheckpointException(string message, string virtualMachineName)
		: base(message.FormatCurrentCulture(virtualMachineName), null)
	{
	}

	public ClusterVirtualMachineGetCheckpointException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachineGetCheckpoint_Default.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	public ClusterVirtualMachineGetCheckpointException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	protected ClusterVirtualMachineGetCheckpointException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}

