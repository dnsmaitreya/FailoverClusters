using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineUnableToDetermineComputerNameException : ClusterException
{
	public ClusterVirtualMachineUnableToDetermineComputerNameException()
	{
	}

	public ClusterVirtualMachineUnableToDetermineComputerNameException(string virtualMachineName)
		: this(virtualMachineName, (Exception)null)
	{
	}

	public ClusterVirtualMachineUnableToDetermineComputerNameException(string message, string virtualMachineName)
		: this(message, virtualMachineName, null)
	{
	}

	public ClusterVirtualMachineUnableToDetermineComputerNameException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.UnableToDetermineVMComputerName_Default.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	public ClusterVirtualMachineUnableToDetermineComputerNameException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	protected ClusterVirtualMachineUnableToDetermineComputerNameException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}

