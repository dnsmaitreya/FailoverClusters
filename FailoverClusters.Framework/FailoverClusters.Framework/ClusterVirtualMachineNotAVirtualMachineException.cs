using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineNotAVirtualMachineException : ClusterException
{
	public ClusterVirtualMachineNotAVirtualMachineException()
	{
	}

	public ClusterVirtualMachineNotAVirtualMachineException(string virtualMachineName)
		: this(virtualMachineName, (Exception)null)
	{
	}

	public ClusterVirtualMachineNotAVirtualMachineException(string message, string virtualMachineName)
		: this(message, virtualMachineName, null)
	{
	}

	public ClusterVirtualMachineNotAVirtualMachineException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachineNotRunning_Default.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	public ClusterVirtualMachineNotAVirtualMachineException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	protected ClusterVirtualMachineNotAVirtualMachineException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}

