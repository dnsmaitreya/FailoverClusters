using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineStartException : ClusterException
{
	public ClusterVirtualMachineStartException()
		: this(string.Empty)
	{
	}

	public ClusterVirtualMachineStartException(string virtualMachineName)
		: base(ExceptionResources.VirtualMachineStart_Default.FormatCurrentCulture(virtualMachineName), null)
	{
	}

	public ClusterVirtualMachineStartException(string message, string virtualMachineName)
		: base(message.FormatCurrentCulture(virtualMachineName), null)
	{
	}

	public ClusterVirtualMachineStartException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachineStart_Default.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	public ClusterVirtualMachineStartException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	protected ClusterVirtualMachineStartException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

