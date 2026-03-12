using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineNoIntegrationServiceException : ClusterException
{
	public ClusterVirtualMachineNoIntegrationServiceException()
	{
	}

	public ClusterVirtualMachineNoIntegrationServiceException(string virtualMachineName)
		: this(virtualMachineName, (Exception)null)
	{
	}

	public ClusterVirtualMachineNoIntegrationServiceException(string message, string virtualMachineName)
		: this(message, virtualMachineName, null)
	{
	}

	public ClusterVirtualMachineNoIntegrationServiceException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VmNoIntegrationComponent_Default.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	public ClusterVirtualMachineNoIntegrationServiceException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	protected ClusterVirtualMachineNoIntegrationServiceException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
