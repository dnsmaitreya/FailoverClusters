using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineWmiQueryFailedException : ClusterException
{
	public ClusterVirtualMachineWmiQueryFailedException()
	{
	}

	public ClusterVirtualMachineWmiQueryFailedException(string virtualMachineName)
		: this(virtualMachineName, (Exception)null)
	{
	}

	public ClusterVirtualMachineWmiQueryFailedException(string message, string virtualMachineName)
		: this(message, virtualMachineName, null)
	{
	}

	public ClusterVirtualMachineWmiQueryFailedException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachineWmiQueryFailed_Default.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	public ClusterVirtualMachineWmiQueryFailedException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	protected ClusterVirtualMachineWmiQueryFailedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}

