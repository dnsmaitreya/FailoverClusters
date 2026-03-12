using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineNotRunningException : ClusterException
{
	public ClusterVirtualMachineNotRunningException()
		: this(string.Empty)
	{
	}

	public ClusterVirtualMachineNotRunningException(string virtualMachineName)
		: this(virtualMachineName, (Exception)null)
	{
	}

	public ClusterVirtualMachineNotRunningException(string message, string virtualMachineName)
		: this(message, virtualMachineName, null)
	{
	}

	public ClusterVirtualMachineNotRunningException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachineNotRunning_Default.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	public ClusterVirtualMachineNotRunningException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	protected ClusterVirtualMachineNotRunningException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
